// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of the given <see cref="Type"/>.
    /// </summary>
    public sealed class ObjectAccessor : Accessor
    {
        private readonly Dictionary<string, Func<object, object>> _objectGettersCache;
        private readonly Dictionary<string, Action<object, object>> _objectSettersCache;

        [DebuggerStepThrough]
        internal ObjectAccessor(IReflect type, bool ignoreCase, bool includeNonPublic) 
            : base(type, ignoreCase, includeNonPublic)
        {
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
    }
}