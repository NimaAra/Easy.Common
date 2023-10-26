namespace Easy.Common.EasyComparer;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Easy.Common.Extensions;
using Easy.Common.Interfaces;

/// <summary>
/// A utility class for comparing the property values of given objects against each other.
/// </summary>
public sealed class EasyComparer
{
    private readonly ConcurrentDictionary<CacheKey, KeyValuePair<PropertyInfo, object>[]> _cache;

    private EasyComparer() => _cache = new ConcurrentDictionary<CacheKey, KeyValuePair<PropertyInfo, object>[]>();

    /// <summary>
    /// Gets a single instance of the <see cref="EasyComparer"/>.
    /// </summary>
    public static EasyComparer Instance { get; } = new EasyComparer();

    /// <summary>
    /// Compares the values of all the properties for the given 
    /// <paramref name="left"/> and <paramref name="right"/> and returns the variance.
    /// </summary>
    public bool Compare<T>(T left, T right, bool inherit, bool includePrivate, out IEasyDictionary<PropertyInfo, Variance> variances)
    {
        var type = typeof(T);
        var key = new CacheKey(type, inherit, includePrivate);
            
        var cache = _cache.GetOrAdd(key, 
            () => type.GetInstanceProperties(inherit, includePrivate)
                .Select(p => new KeyValuePair<PropertyInfo, object>(p, AccessorBuilder.BuildGetter<T>(p, includePrivate)))
                .ToArray());

        var bothMatch = true;
        var result = new EasyDictionary<PropertyInfo, Variance>(variance => variance.Property);

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < cache.Length; i++)
        {
            var pair = cache[i];
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

        public override int GetHashCode() => HashHelper.GetHashCode(Type, Inherit, IncludePrivate);
    }
}