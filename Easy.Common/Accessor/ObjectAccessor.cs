// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of the given <see cref="Type"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed class ObjectAccessor
    {
        private readonly Dictionary<string, Func<object, object>> _getters;
        private readonly Dictionary<string, Action<object, object>> _setters;

        [DebuggerStepThrough]
        internal ObjectAccessor(IReflect type, bool ignoreCase, bool includeNonPublic)
        {
            var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeNonPublic)
            {
                flags = flags | BindingFlags.NonPublic;
            }

            Properties = type.GetProperties(flags);

            _getters = new Dictionary<string, Func<object, object>>(Properties.Length, comparer);
            _setters = new Dictionary<string, Action<object, object>>(Properties.Length, comparer);

            foreach (var prop in Properties)
            {
                _getters[prop.Name] = Accessor.CreateGetter(prop, includeNonPublic);
                _setters[prop.Name] = Accessor.CreateSetter(prop, includeNonPublic);
            }
        }

        /// <summary>
        /// Gets the properties to which this instance provides access to.
        /// </summary>
        public PropertyInfo[] Properties { get; }

        /// <summary>
        /// Gets or sets the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public object this[object instance, string propertyName]
        {
            get
            {
                Func<object, object> getter;
                Accessor.ThrowIfNotFound(_getters.TryGetValue(propertyName, out getter), propertyName);
                return getter(instance);
            }

            set
            {
                Action<object, object> setter;
                Accessor.ThrowIfNotFound(_setters.TryGetValue(propertyName, out setter), propertyName);
                setter(instance, value);
            }
        }
    }
}