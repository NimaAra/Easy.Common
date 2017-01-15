 // ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of the given <see cref="Type"/>.
    /// </summary>
    public class Accessor
    {
        /// <summary>
        /// Gets the <see cref="StringComparer"/> used by the <see cref="Accessor"/> to find the properties on the given instance. 
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
            if (includeNonPublic)
            {
                flags = flags | BindingFlags.NonPublic;
            }

            Properties = Type.GetProperties(flags);

            _objectGettersCache = new Dictionary<string, Func<object, object>>(Properties.Length, Comparer);
            _objectSettersCache = new Dictionary<string, Action<object, object>>(Properties.Length, Comparer);

            foreach (var prop in Properties)
            {
                var propName = prop.Name;
                _objectGettersCache[propName] = AccessorBuilder.BuildGetter(prop, IncludesNonPublic);
                _objectSettersCache[propName] = AccessorBuilder.BuildSetter(prop, IncludesNonPublic);
            }
        }

        /// <summary>
        /// Gets or sets the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public object this[object instance, string propertyName]
        {
            get
            {
                Func<object, object> getter;
                _objectGettersCache.TryGetValue(propertyName, out getter);
                // ReSharper disable once PossibleNullReferenceException
                return getter(instance);
            }

            set
            {
                Action<object, object> setter;
                _objectSettersCache.TryGetValue(propertyName, out setter);
                // ReSharper disable once PossibleNullReferenceException
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
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of the given <typeparamref name="TInstance"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed class Accessor<TInstance> : Accessor where TInstance : class
    {
        private readonly ConcurrentDictionary<string, object> _genericInstanceGettersCache, _genericInstanceSettersCache;
        private readonly Dictionary<string, Func<TInstance, object>> _genericPropertiesGettersCache;
        private readonly Dictionary<string, Action<TInstance, object>> _genericPropertiesSettersCache;

        internal Accessor(bool ignoreCase, bool includeNonPublic)
            : base(typeof(TInstance), ignoreCase, includeNonPublic)
        {
            _genericPropertiesGettersCache = new Dictionary<string, Func<TInstance, object>>(Properties.Length, Comparer);
            _genericPropertiesSettersCache = new Dictionary<string, Action<TInstance, object>>(Properties.Length, Comparer);
            _genericInstanceGettersCache = new ConcurrentDictionary<string, object>(Comparer);
            _genericInstanceSettersCache = new ConcurrentDictionary<string, object>(Comparer);

            foreach (var prop in Properties) {
                var propName = prop.Name;
                _genericPropertiesGettersCache[propName] = AccessorBuilder.BuildGetter<TInstance>(prop, IncludesNonPublic);
                _genericPropertiesSettersCache[propName] = AccessorBuilder.BuildSetter<TInstance>(prop, IncludesNonPublic);
            }
        }
        
        /// <summary>
        /// Gets the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public object Get(TInstance instance, string propertyName)
        {
            Func<TInstance, object> getter;
            _genericPropertiesGettersCache.TryGetValue(propertyName, out getter);
            // ReSharper disable once PossibleNullReferenceException
            return getter(instance);
        }

        /// <summary>
        /// Sets the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public void Set(TInstance instance, string propertyName, object propValue)
        {
            Action<TInstance, object> setter;
            _genericPropertiesSettersCache.TryGetValue(propertyName, out setter);
            // ReSharper disable once PossibleNullReferenceException
            setter(instance, propValue);
        }

        /// <summary>
        /// Gets the value of a property selected by the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public TProperty Get<TProperty>(TInstance instance, string propertyName)
        {
            var cache = _genericInstanceGettersCache;

            Func<TInstance, TProperty> getter;
            object tmpGetter;
            if (!cache.TryGetValue(propertyName, out tmpGetter))
            {
                getter = AccessorBuilder.BuildGetter<TInstance, TProperty>(propertyName, IncludesNonPublic);
                cache[propertyName] = getter;
            }
            else
            {
                getter = (Func<TInstance, TProperty>)tmpGetter;
            }

            return getter(instance);
        }

        /// <summary>
        /// Sets the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public void Set<TProperty>(TInstance instance, string propertyName, TProperty value)
        {
            var cache = _genericInstanceSettersCache;

            Action<TInstance, TProperty> setter;
            object tmpSetter;
            if (!cache.TryGetValue(propertyName, out tmpSetter))
            {
                setter = AccessorBuilder.BuildSetter<TInstance, TProperty>(propertyName, IncludesNonPublic);
                cache[propertyName] = setter;
            }
            else
            {
                setter = (Action<TInstance, TProperty>)tmpSetter;
            }

            setter(instance, value);
        }
    }
}