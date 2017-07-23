namespace Easy.Common.EasyComparer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Easy.Common.Extensions;

    /// <summary>
    /// A utility class for comparing the property values of given objects against each other.
    /// </summary>
    public sealed class EasyComparer
    {
        private readonly ConcurrentDictionary<CacheKey, IEnumerable<KeyValuePair<PropertyInfo, object>>> _cache;

        private EasyComparer()
        {
            _cache = new ConcurrentDictionary<CacheKey, IEnumerable<KeyValuePair<PropertyInfo, object>>>();
        }

        /// <summary>
        /// Gets a single instance of the <see cref="EasyComparer"/>.
        /// </summary>
        public static EasyComparer Instance { get; } = new EasyComparer();

        /// <summary>
        /// Compares the values of all the properties for the given 
        /// <paramref name="left"/> and <paramref name="right"/> and returns the variance.
        /// </summary>
        public bool Compare<T>(T left, T right, bool inherit, bool includePrivate, 
            out KeyedCollectionEx<PropertyInfo, Variance> variances)
        {
            var type = typeof(T);
            var key = new CacheKey(type, inherit, includePrivate);
            
            var cache = _cache.GetOrAdd(key, () =>
            {
                return type.GetInstanceProperties(inherit, includePrivate)
                    .Select(p => new KeyValuePair<PropertyInfo, object>(p, AccessorBuilder.BuildGetter<T>(p, includePrivate)));
            });

            var bothMatch = true;
            var result = new KeyedCollectionEx<PropertyInfo, Variance>(variance => variance.Property);

            foreach (var pair in cache)
            {
                var p = pair.Key;
                var getter = (Func<T, object>)pair.Value;

                var leftVal = getter(left);
                var rightVal = getter(right);

                var variance = new Variance(p, leftVal, rightVal);
                
                result.Add(variance);

                if (!bothMatch) { continue; }
                if (variance.Varies) { bothMatch = false; }
            }

            variances = result;
            return bothMatch;
        }

        private sealed class CacheKey : Equatable<CacheKey>
        {
            public CacheKey(Type type, bool inherit, bool includePrivate)
            {
                Type = type;
                Inherit = inherit;
                IncludePrivate = includePrivate;
            }

            private Type Type { get; }
            private bool Inherit { get; }
            private bool IncludePrivate { get; }

            public override int GetHashCode()
            {
                return HashHelper.GetHashCode(Type, Inherit, IncludePrivate);
            }
        }
    }
}