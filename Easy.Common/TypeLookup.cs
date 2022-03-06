namespace Easy.Common;

using System;
using System.Threading;

/// <summary>
/// An abstraction for a fast lookup between a given type and a <typeparamref name="TValue"/>.
/// </summary>
public sealed class TypeLookup<TValue>
{
    // ReSharper disable once StaticMemberInGenericType
    private static int TypeIndex = -1;
    
    private readonly object _sync = new();

    private (TValue, bool)[] _values = new (TValue, bool)[100];

    /// <summary>
    /// Gets the number of registered items.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Adds the given <paramref name="value"/> against the given <typeparamref name="TKey"/>.
    /// <remarks>Overwrites any existing value if key already exists.</remarks>
    /// </summary>
    public void Add<TKey>(TValue value)
    {
        lock (_sync)
        {
            int id = TypeKey<TKey>.Id;
            if (id >= _values.Length)
            {
                Array.Resize(ref _values, id * 2);
            }

            _values[id] = new(value, true);
            Count++;
        }
    }

    /// <summary>
    /// Attempts to resolve the value stored against the given <typeparamref name="TKey"/> if any.
    /// </summary>
    public bool TryGet<TKey>(out TValue value)
    {
        int id = TypeKey<TKey>.Id;
        if (id >= _values.Length)
        {
            value = default;
            return false;
        }

        (TValue val, bool hasVal) = _values[id];

        value = val;
        return hasVal;
    }

    // ReSharper disable once UnusedTypeParameter
    private static class TypeKey<TKey>
    {
        // ReSharper disable once StaticMemberInGenericType
        internal static readonly int Id = Interlocked.Increment(ref TypeIndex);
    }
}