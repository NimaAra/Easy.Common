namespace Easy.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies the contract to be implemented by any instance of <see cref="IEasyDictionary{TKey,TValue}"/>.
/// </summary>
public interface IEasyDictionary<TKey, TValue> : ICollection<TValue>
{
    /// <summary>
    /// Gets the delegate used to select the key against which the item will be stored.
    /// </summary>
    Func<TValue, TKey> KeySelector { get; }

    /// <summary>
    /// Gets the <see cref="IEqualityComparer{TKey}"/> that is used to determine 
    /// equality of keys for the dictionary.
    /// </summary>
    IEqualityComparer<TKey> Comparer { get; }

    /// <summary>
    /// Gets the keys stored in the dictionary.
    /// </summary>
    ICollection<TKey> Keys { get; }

    /// <summary>
    /// Gets the items stored as values stored in the dictionary.
    /// </summary>
    ICollection<TValue> Values { get; }

    /// <summary>
    /// Gets the value associated with the given <paramref name="key"/>.
    /// </summary>
    TValue this[TKey key] { get; }

    /// <summary>
    /// Returns the items stored in this instance as <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    IDictionary<TKey, TValue> ToDictionary();
        
    /// <summary>
    /// Returns the items stored in this instance as <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    IDictionary<TKey, TValue> ToDictionary(IEqualityComparer<TKey> comparer);

    /// <summary>
    /// Determines whether the given <paramref name="key"/> exists.
    /// </summary>
    bool ContainsKey(TKey key);

    /// <summary>
    /// Attempts to get the value associated with the specified <paramref name="key"/>.
    /// </summary>
    bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value);

    /// <summary>
    /// Removes the given <paramref name="key"/> from the dictionary.
    /// </summary>
    bool Remove(TKey key);

    /// <summary>
    /// Adds the <paramref name="value"/> if it does not already exist or replaces the existing value.
    /// </summary>
    void AddOrReplace(TValue value);
}