// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of 
    /// the given <typeparamref name="TInstance"/>.
    /// </summary>
    public sealed class GenericAccessor<TInstance> : Accessor where TInstance : class
    {
        private readonly Dictionary<string, object> _genericInstanceGettersCache, _genericInstanceSettersCache;
        private readonly Dictionary<string, Func<TInstance, object>> _genericPropertiesGettersCache;
        private readonly Dictionary<string, Action<TInstance, object>> _genericPropertiesSettersCache;

        [DebuggerStepThrough]
        internal GenericAccessor(bool ignoreCase, bool includeNonPublic) : base(typeof(TInstance), ignoreCase, includeNonPublic)
        {
            _genericPropertiesGettersCache = new Dictionary<string, Func<TInstance, object>>(Comparer);
            _genericPropertiesSettersCache = new Dictionary<string, Action<TInstance, object>>(Comparer);
            _genericInstanceGettersCache = new Dictionary<string, object>(Comparer);
            _genericInstanceSettersCache = new Dictionary<string, object>(Comparer);

            foreach (var prop in Properties)
            {
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
        /// Gets or sets the value of the given <paramref name="propertyName"/> for 
        /// the given <paramref name="instance"/>.
        /// </summary>
        public object this[TInstance instance, string propertyName]
        {
            get
            {
                if (!_genericPropertiesGettersCache.TryGetValue(propertyName, out Func<TInstance, object> getter))
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports reading.");
                }
                return getter(instance);
            }

            set
            {
                if (!_genericPropertiesSettersCache.TryGetValue(propertyName, out Action<TInstance, object> setter))
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports writing.");
                }
                setter(instance, value);
            }
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
                lock (cache)
                {
                    if (!cache.TryGetValue(propertyName, out object tmp))
                    {
                        try
                        {
                            getter = AccessorBuilder.BuildGetter<TInstance, TProperty>(propertyName, IncludesNonPublic);
                            cache[propertyName] = getter;
                        } catch (ArgumentException)
                        {
                            value = default(TProperty);
                            return false;
                        }
                    } else
                    {
                        getter = (Func<TInstance, TProperty>)tmp;
                    }
                }
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
                lock (cache)
                {
                    if (!cache.TryGetValue(propertyName, out object tmp))
                    {
                        try
                        {
                            setter = AccessorBuilder.BuildSetter<TInstance, TProperty>(propertyName, IncludesNonPublic);
                            cache[propertyName] = setter;
                        } catch (ArgumentException)
                        {
                            return false;
                        }
                    } else
                    {
                        setter = (Action<TInstance, TProperty>)tmp;
                    }
                }
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