 // ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of the given <see cref="Type"/>.
    /// </summary>
    public class Accessor
    {
        /// <summary>
        /// Builds an <see cref="Accessor"/> which provides easy access to all of 
        /// the <see cref="PropertyInfo"/> of the given <paramref name="type"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Accessor Build(Type type, bool ignoreCase = false, bool includeNonPublic = false)
        {
            Ensure.NotNull(type, nameof(type));
            return new Accessor(type, ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// Builds an <see cref="Accessor{TInstance}"/> which provides easy access to all of 
        /// the <see cref="PropertyInfo"/> of the given <typeparamref name="TInstance"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Accessor<TInstance> Build<TInstance>(bool ignoreCase = false, bool includeNonPublic = false) where TInstance : class
        {
            return new Accessor<TInstance>(ignoreCase, includeNonPublic);
        }
        
        /// <summary>
        /// Gets the <see cref="StringComparer"/> used by the <see cref="Accessor"/> to find 
        /// the properties on the given instance. 
        /// </summary>
        protected StringComparer Comparer;

        private readonly Dictionary<string, Func<object, object>> _objectGettersCache;
        private readonly Dictionary<string, Action<object, object>> _objectSettersCache;

        [DebuggerStepThrough]
        internal Accessor(IReflect type, bool ignoreCase, bool includeNonPublic)
        {
            Type = type;
            IgnoreCase = ignoreCase;
            IncludesNonPublic = includeNonPublic;

            Comparer = IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (IncludesNonPublic)
            {
                flags = flags | BindingFlags.NonPublic;
            }

            Properties = Type.GetProperties(flags);

            _objectGettersCache = new Dictionary<string, Func<object, object>>(Properties.Length, Comparer);
            _objectSettersCache = new Dictionary<string, Action<object, object>>(Properties.Length, Comparer);

            foreach (var prop in Properties)
            {
                var propName = prop.Name;
             
                if (prop.CanRead)
                {
                    _objectGettersCache[propName] = AccessorBuilder.BuildGetter(prop, IncludesNonPublic);
                }
             
                if (prop.CanWrite)
                {
                    _objectSettersCache[propName] = AccessorBuilder.BuildSetter(prop, IncludesNonPublic);
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the given <paramref name="propertyName"/> for 
        /// the given <paramref name="instance"/>.
        /// </summary>
        public object this[object instance, string propertyName]
        {
            get
            {
                if (!_objectGettersCache.TryGetValue(propertyName, out Func<object, object> getter))
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports reading.");
                }

                return getter(instance);
            }

            set
            {
                if (!_objectSettersCache.TryGetValue(propertyName, out Action<object, object> setter))
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports writing.");
                }

                setter(instance, value);
            }
        }

        /// <summary>
        /// Gets the type of the object this instance supports.
        /// </summary>
        public IReflect Type { get; }

        /// <summary>
        /// Gets the flag indicating whether property names should be treated in a case sensitive manner.
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// Gets the flag indicating whether non-public properties should be supported by this instance.
        /// </summary>
        public bool IncludesNonPublic { get; }

        /// <summary>
        /// Gets the properties to which this instance can provide access to.
        /// </summary>
        public PropertyInfo[] Properties { get; }
    }

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of 
    /// the given <typeparamref name="TInstance"/>.
    /// </summary>
    public sealed class Accessor<TInstance> : Accessor where TInstance : class
    {
        private readonly ConcurrentDictionary<string, object> _genericInstanceGettersCache, _genericInstanceSettersCache;
        private readonly Dictionary<string, Func<TInstance, object>> _genericPropertiesGettersCache;
        private readonly Dictionary<string, Action<TInstance, object>> _genericPropertiesSettersCache;

        internal Accessor(bool ignoreCase, bool includeNonPublic) : base(typeof(TInstance), ignoreCase, includeNonPublic)
        {
            _genericPropertiesGettersCache = new Dictionary<string, Func<TInstance, object>>(Properties.Length, Comparer);
            _genericPropertiesSettersCache = new Dictionary<string, Action<TInstance, object>>(Properties.Length, Comparer);
            _genericInstanceGettersCache = new ConcurrentDictionary<string, object>(Comparer);
            _genericInstanceSettersCache = new ConcurrentDictionary<string, object>(Comparer);

            foreach (var prop in Properties) {
                var propName = prop.Name;
             
                if (prop.CanRead)
                {
                    _genericPropertiesGettersCache[propName] = AccessorBuilder.BuildGetter<TInstance>(prop, IncludesNonPublic);
                }
             
                if (prop.CanWrite)
                {
                    _genericPropertiesSettersCache[propName] = AccessorBuilder.BuildSetter<TInstance>(prop, IncludesNonPublic);
                }
            }
        }
        
        /// <summary>
        /// Attempts to get the value of the given <paramref name="propertyName"/> for 
        /// the given <paramref name="instance"/>.
        /// </summary>
        public bool TryGet(TInstance instance, string propertyName, out object value)
        {
            if (!_genericPropertiesGettersCache.TryGetValue(propertyName, out Func<TInstance, object> getter))
            {
                value = null;
                return false;
            }

            value = getter(instance);
            return true;
        }

        /// <summary>
        /// Attempts to set the value of the given <paramref name="propertyName"/> for 
        /// the given <paramref name="instance"/>.
        /// </summary>
        public bool TrySet(TInstance instance, string propertyName, object value)
        {
            if (!_genericPropertiesSettersCache.TryGetValue(propertyName, out Action<TInstance, object> setter))
            {
                return false;
            }

            setter(instance, value);
            return true;
        }

        /// <summary>
        /// Attempts to get the value of a property selected by the given <paramref name="propertyName"/> 
        /// for the given <paramref name="instance"/>.
        /// </summary>
        public bool TryGet<TProperty>(TInstance instance, string propertyName, out TProperty value)
        {
            var cache = _genericInstanceGettersCache;

            Func<TInstance, TProperty> getter;
            if (!cache.TryGetValue(propertyName, out object tmpGetter))
            {
                var propInfo = Array.Find(Properties, p => Comparer.Compare(p.Name, propertyName) == 0);
                if (propInfo == null || !propInfo.CanRead)
                {
                    value = default(TProperty);
                    return false;
                }
                
                getter = AccessorBuilder.BuildGetter<TInstance, TProperty>(propInfo, IncludesNonPublic);
                cache[propertyName] = getter;
            }
            else
            {
                getter = (Func<TInstance, TProperty>)tmpGetter;
            }

            value = getter(instance);
            return true;
        }

        /// <summary>
        /// Attempts to set the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public bool TrySet<TProperty>(TInstance instance, string propertyName, TProperty value)
        {
            var cache = _genericInstanceSettersCache;

            Action<TInstance, TProperty> setter;
            if (!cache.TryGetValue(propertyName, out object tmpSetter))
            {
                var propInfo = Array.Find(Properties, p => Comparer.Compare(p.Name, propertyName) == 0);
                if (propInfo == null || !propInfo.CanWrite) { return false; }
                
                setter = AccessorBuilder.BuildSetter<TInstance, TProperty>(propInfo, IncludesNonPublic);
                cache[propertyName] = setter;
            }
            else
            {
                setter = (Action<TInstance, TProperty>)tmpSetter;
            }

            setter(instance, value);
            return true;
        }
    }
}
