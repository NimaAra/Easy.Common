namespace Easy.Common;

using Easy.Common.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// A dictionary whose <typeparamref name="TKey"/> can be defined as a delegate over 
/// any property of the <typeparamref name="TValue"/>. 
/// <remarks>
/// <para>This class is not thread-safe.</para>
/// <para>The keys associated to the values must not be changed after insertion.</para>
/// </remarks> 
/// </summary>
public sealed class EasyDictionary<TKey, TValue> : IEasyDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary;

    /// <summary>
    /// Creates an instance of <see cref="EasyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="keySelector">
    /// The delegate used to select the key against which the item will be stored.
    /// </param>
    /// <param name="sequence">
    /// A sequence to initialize the dictionary with.
    /// </param>
    /// <param name="comparer">
    /// The implementation of the <see cref="IEqualityComparer{TKey}"/> generic 
    /// interface to use when comparing keys, or null to use the default equality 
    /// comparer for the type of the key, obtained from <see cref="EqualityComparer{TKey}.Default"/>.
    /// </param>
    public EasyDictionary(
        Func<TValue, TKey> keySelector,
        IEnumerable<TValue> sequence,
        IEqualityComparer<TKey>? comparer = default)
        : this(keySelector, comparer: comparer) => PopulateFrom(sequence);

    /// <summary>
    /// Creates an instance of <see cref="EasyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="keySelector">
    /// The delegate used to select the key against which the item will be stored.
    /// </param>
    /// <param name="collection">
    /// A collection to initialize the dictionary with.
    /// </param>
    /// <param name="comparer">
    /// The implementation of the <see cref="IEqualityComparer{TKey}"/> generic 
    /// interface to use when comparing keys, or null to use the default equality 
    /// comparer for the type of the key, obtained from <see cref="EqualityComparer{TKey}.Default"/>.
    /// </param>
    public EasyDictionary(
        Func<TValue, TKey> keySelector,
        ICollection<TValue> collection,
        IEqualityComparer<TKey>? comparer = default)
        : this(keySelector, (uint)collection.Count, comparer) => PopulateFrom(collection);

    /// <summary>
    /// Creates an instance of <see cref="EasyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="keySelector">
    /// The delegate used to select the key against which the item will be stored.
    /// </param>
    /// <param name="dictionary">
    /// A dictionary to initialize the dictionary with.
    /// <remarks>
    /// Note a new key based on the <paramref name="keySelector"/> will be used to 
    /// store every value of the dictionary.
    /// </remarks>
    /// </param>
    /// <param name="comparer">
    /// The implementation of the <see cref="IEqualityComparer{TKey}"/> generic 
    /// interface to use when comparing keys, or null to use the default equality 
    /// comparer for the type of the key, obtained from <see cref="EqualityComparer{TKey}.Default"/>.
    /// </param>
    public EasyDictionary(
        Func<TValue, TKey> keySelector,
        IDictionary<TKey, TValue> dictionary, 
        IEqualityComparer<TKey>? comparer = default) 
        : this(keySelector, (uint)dictionary.Count, comparer) => PopulateFrom(dictionary);

    /// <summary>
    /// Creates an instance of <see cref="EasyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="keySelector">
    /// The delegate used to select the key against which the item will be stored.
    /// </param>
    /// <param name="capacity">The initial number of elements this instance can contain.</param>
    /// <param name="comparer">
    /// The implementation of the <see cref="IEqualityComparer{TKey}"/> generic 
    /// interface to use when comparing keys, or null to use the default equality 
    /// comparer for the type of the key, obtained from <see cref="EqualityComparer{TKey}.Default"/>.
    /// </param>
    public EasyDictionary(
        Func<TValue, TKey> keySelector,
        uint capacity = 0,
        IEqualityComparer<TKey>? comparer = default)
    {
        KeySelector = Ensure.NotNull(keySelector, nameof(keySelector));
        _dictionary = new Dictionary<TKey, TValue>((int)capacity, comparer);
    }

    /// <inheritdoc/>
    public Func<TValue, TKey> KeySelector { get; }

    /// <inheritdoc cref="ICollection" />
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public IEqualityComparer<TKey> Comparer => _dictionary.Comparer;

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<TValue> Values => _dictionary.Values;

    /// <inheritdoc/>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

    /// <inheritdoc/>
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

    /// <inheritdoc cref="IDictionary{TKey, TValue}" />
    public TValue this[TKey key] => _dictionary[key];

    /// <inheritdoc/>
    public IDictionary<TKey, TValue> ToDictionary() => new Dictionary<TKey, TValue>(_dictionary, Comparer);
        
    /// <inheritdoc/>
    public IDictionary<TKey, TValue> ToDictionary(IEqualityComparer<TKey> comparer) => 
        new Dictionary<TKey, TValue>(_dictionary, comparer);

    /// <inheritdoc/>
    public void Add(TValue value) => _dictionary.Add(KeySelector(value), value);

    /// <inheritdoc/>
    public void AddOrReplace(TValue value) => _dictionary[KeySelector(value)] = value;

    /// <inheritdoc/>
    public bool Remove(TKey key) => _dictionary.Remove(key);

    /// <inheritdoc/>
    public bool Remove(TValue value) => _dictionary.Remove(KeySelector(value));

    /// <inheritdoc/>
    public void Clear() => _dictionary.Clear();

    /// <inheritdoc/>
    public bool Contains(TValue value) => 
        _dictionary.ContainsKey(KeySelector(value)) && _dictionary.ContainsValue(value);
        
    /// <inheritdoc cref="IDictionary{TKey, TValue}" />
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    /// <inheritdoc cref="IDictionary{TKey, TValue}" />
    public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? value) => _dictionary.TryGetValue(key, out value!);

    /// <inheritdoc/>
    public void CopyTo(TValue[] array, int startIndex) => _dictionary.Values.CopyTo(array, startIndex);

    /// <inheritdoc/>
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => 
        _dictionary.GetEnumerator();

    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator() => _dictionary.Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

    [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
    private void PopulateFrom(IEnumerable<TValue> sequence)
    {
        switch (sequence)
        {
            case IReadOnlyList<TValue> readonlyList:
                for (var i = 0; i < readonlyList.Count; i++) { Add(readonlyList[i]); }
                break;
            case IList<TValue> list:
                for (var i = 0; i < list.Count; i++) { Add(list[i]); }
                break;
            default:
                foreach (var item in sequence) { Add(item); }
                break;
        }
    }

    private void PopulateFrom(IDictionary<TKey, TValue> dictionary)
    {
        if (dictionary is Dictionary<TKey, TValue> concrete)
        {
            foreach (var pair in concrete) { Add(pair.Value); }
        } 
        else
        {
            foreach (var pair in dictionary) { Add(pair.Value); }
        }
    }
}