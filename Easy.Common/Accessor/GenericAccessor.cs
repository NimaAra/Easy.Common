 // ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// An abstraction for gaining fast access to all of the <see cref="PropertyInfo"/> of the given <typeparamref name="{T}"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed class GenericAccessor<T> where T : class
    {
        private readonly Dictionary<string, Func<T, object>> _getters;
        private readonly Dictionary<string, Action<T, object>> _setters;

        [DebuggerStepThrough]
        internal GenericAccessor(bool ignoreCase, bool includeNonPublic)
        {
            var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (includeNonPublic)
            {
                flags = flags | BindingFlags.NonPublic;
            }

            Properties = typeof(T).GetProperties(flags);

            _getters = new Dictionary<string, Func<T, object>>(Properties.Length, comparer);
            _setters = new Dictionary<string, Action<T, object>>(Properties.Length, comparer);

            foreach (var prop in Properties)
            {
                _getters[prop.Name] = Accessor.CreateGetter<T>(prop, includeNonPublic);
                _setters[prop.Name] = Accessor.CreateSetter<T>(prop, includeNonPublic);
            }
        }

        /// <summary>
        /// Gets the properties to which this instance provides access to.
        /// </summary>
        public PropertyInfo[] Properties { get; }

        /// <summary>
        /// Gets or sets the value of the given <paramref name="propertyName"/> for the given <paramref name="instance"/>.
        /// </summary>
        public object this[T instance, string propertyName]
        {
            get
            {
                Func<T, object> getter;
                Accessor.ThrowIfNotFound(_getters.TryGetValue(propertyName, out getter), propertyName);
                return getter(instance);
            }

            set
            {
                Action<T, object> setter;
                Accessor.ThrowIfNotFound(_setters.TryGetValue(propertyName, out setter), propertyName);
                setter(instance, value);
            }
        }
    }
}