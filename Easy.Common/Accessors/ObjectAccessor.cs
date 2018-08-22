// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/>
    /// of the given <see cref="Type"/>.
    /// </summary>
    public class ObjectAccessor : Accessor
    {
        private readonly Hashtable _objectGettersCache, _objectSettersCache;

        [DebuggerStepThrough]
        internal ObjectAccessor(IReflect type, bool ignoreCase, bool includeNonPublic) 
            : base(type, ignoreCase, includeNonPublic)
        {
            _objectGettersCache = new Hashtable(Properties.Count, Comparer);
            _objectSettersCache = new Hashtable(Properties.Count, Comparer);

            foreach (var pair in Properties)
            {
                var propName = pair.Key;
                var prop = pair.Value;

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
                if (_objectGettersCache[propertyName] is Func<object, object> getter)
                {
                    return getter(instance);
                }
                throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports reading.");
            }

            set
            {
                if (_objectSettersCache[propertyName] is Action<object, object> setter)
                {
                    setter(instance, value);
                } else
                {
                    throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports writing.");
                }
            }
        }
    }
}