namespace Easy.Common.Extensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Extension methods for <see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Adds the <paramref name="key"/> and <paramref name="value"/> to the <paramref name="dictionary"/>
    /// if the <paramref name="key"/> does not already exists and returns the inserted value.
    /// </summary>
    [DebuggerStepThrough]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary.TryGetValue(key, out TValue? result)) { return result; }
            
        dictionary[key] = value;
        return value;
    }

    /// <summary>
    /// Adds the <paramref name="key"/> and the value created by <paramref name="valueCreator"/> to 
    /// the <paramref name="dictionary"/> if the <paramref name="key"/> does not already exists 
    /// and returns the inserted value.
    /// </summary>
    [DebuggerStepThrough]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueCreator)
    {
        if (dictionary.TryGetValue(key, out TValue? result)) { return result; }
            
        TValue value = valueCreator();
        dictionary[key] = value;
        result = value;
        return result;
    }
        
    /// <summary>
    /// Gets the value associated with the specified key or the <paramref name="defaultValue"/> if it does not exist.
    /// </summary>
    /// <param name="dictionary">The Source Dictionary</param>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="defaultValue">The default value to return if an item with the specified <paramref name="key"/> does not exist.</param>
    /// <returns>The value associated with the specified key or the <paramref name="defaultValue"/> if it does not exist.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default) =>
        dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;

    /// <summary>
    /// Adds the given <paramref name="pairsToAdd"/> to the given <paramref name="dictionary"/>.
    /// <remarks>This method is used to duck-type <see cref="IDictionary{TKey, TValue}"/> with multiple pairs.</remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> pairsToAdd)
    {
        foreach (KeyValuePair<TKey, TValue> pair in pairsToAdd)
        {
            dictionary.Add(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Returns a <see cref="NameValueCollection"/> as a Dictionary
    /// </summary>
    public static Dictionary<string, string> ToDictionary(this NameValueCollection namedValueCollection) =>
        namedValueCollection.AllKeys.ToDictionary(key => key!, key => namedValueCollection[key]!);

    /// <summary>
    /// Returns a <see cref="ConcurrentDictionary{TKey,TValue}"/> from an <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dictionary) where TKey : notnull => new(dictionary);

    /// <summary>
    /// Returns a <see cref="ConcurrentDictionary{TKey,TValue}"/> from an <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary,
        IEqualityComparer<TKey> comparer) where TKey : notnull => new (dictionary, comparer);

    /// <summary>
    /// Compares the given <paramref name="left"/> against <paramref name="right"/> for equality.
    /// </summary>
    public static bool EqualsAnother<TKey, TValue>(this IDictionary<TKey, TValue> left, 
        IDictionary<TKey, TValue> right, IEqualityComparer<TValue>? valueComparer = default) where TKey : notnull =>
            EqualsAnother((IReadOnlyDictionary<TKey, TValue>) left, (IReadOnlyDictionary<TKey, TValue>) right, valueComparer);


    /// <summary>
    /// Compares the given <paramref name="left"/> against <paramref name="right"/> for equality.
    /// </summary>
    [DebuggerStepThrough]
    public static bool EqualsAnother<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> left, 
        IReadOnlyDictionary<TKey, TValue> right, IEqualityComparer<TValue>? valueComparer = default) where TKey : notnull
    {
        if (left == right) { return true; }
        if (left.Count != right.Count) { return false; }
        if (left.Count == 0) { return true; }
        
        IEqualityComparer<TValue> comparer = valueComparer ?? EqualityComparer<TValue>.Default;

        if (left is Dictionary<TKey, TValue> leftConcrete)
        {
            foreach (KeyValuePair<TKey, TValue> pair in leftConcrete)
            {
                if (!KeyValueExists(pair.Key, pair.Value, right, comparer)) { return false; }
            }
        }
        else if (right is Dictionary<TKey, TValue> rightConcrete)
        {
            foreach (KeyValuePair<TKey, TValue> pair in rightConcrete)
            {
                if (!KeyValueExists(pair.Key, pair.Value, left, comparer)) { return false; }
            }
        } 
        else if (left is EasyDictionary<TKey, TValue> leftEasyConcrete)
        {
            Func<TValue, TKey> keySelector = leftEasyConcrete.KeySelector;
            foreach (TValue item in leftEasyConcrete)
            {
                if (!KeyValueExists(keySelector(item), item, right, comparer)) { return false; }
            }
        }
        else if (right is EasyDictionary<TKey, TValue> rightEasyConcrete)
        {
            Func<TValue, TKey> keySelector = rightEasyConcrete.KeySelector;
            foreach (TValue item in rightEasyConcrete)
            {
                if (!KeyValueExists(keySelector(item), item, left, comparer)) { return false; }
            }
        }
        else
        {
            foreach (KeyValuePair<TKey, TValue> pair in left)
            {
                if (!KeyValueExists(pair.Key, pair.Value, right, comparer)) { return false; }
            }
        }
        return true;

        static bool KeyValueExists(TKey key, TValue value, IReadOnlyDictionary<TKey, TValue> dictionary, IEqualityComparer<TValue> comparer) =>
            dictionary.TryGetValue(key, out var rightVal) && comparer.Equals(value, rightVal);
    }
}