namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
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
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, 
            TKey key, TValue value)
        {
            if (dictionary.TryGetValue(key, out var result)) { return result; }
            
            dictionary[key] = value;
            return value;
        }

        /// <summary>
        /// Adds the <paramref name="key"/> and the value created by <paramref name="valueCreator"/> to 
        /// the <paramref name="dictionary"/> if the <paramref name="key"/> does not already exists 
        /// and returns the inserted value.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, 
            TKey key, Func<TValue> valueCreator)
        {
            if (dictionary.TryGetValue(key, out var result)) { return result; }
            
            var value = valueCreator();
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
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, 
            TKey key, TValue defaultValue = default(TValue)) 
                => dictionary.TryGetValue(key, out var value) ? value : defaultValue;

        /// <summary>
        /// Compares the given <paramref name="left"/> against <paramref name="right"/> for equality.
        /// </summary>
        public static bool Equals<TKey, TValue>(
            this IDictionary<TKey, TValue> left,
            IDictionary<TKey, TValue> right,
            IEqualityComparer<TValue> valueComparer = null)
                => Equals((IReadOnlyDictionary<TKey, TValue>) left, (IReadOnlyDictionary<TKey, TValue>) right, valueComparer);

        /// <summary>
        /// Compares the given <paramref name="left"/> against <paramref name="right"/> for equality.
        /// </summary>
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        public static bool Equals<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> left,
            IReadOnlyDictionary<TKey, TValue> right, 
            IEqualityComparer<TValue> valueComparer = null)
        {
            if (left == right) { return true; }
            if (left == null || right == null) { return false; }
            if (left.Count != right.Count) { return false; }
            if (left.Count == 0) { return true; }

            var comparer = valueComparer ?? EqualityComparer<TValue>.Default;

            if (left is Dictionary<TKey, TValue> leftConcrete)
            {
                foreach (var pair in leftConcrete) { if (!KeyValuePairExists(pair, right, comparer)) { return false; } }
            }
            else if (right is Dictionary<TKey, TValue> rightConcrete)
            {
                foreach (var pair in rightConcrete) { if (!KeyValuePairExists(pair, left, comparer)) { return false; } }
            } 
            else if (left is EasyDictionary<TKey, TValue> leftEasyConcrete)
            {
                var keySelector = leftEasyConcrete.KeySelector;
                foreach (var item in leftEasyConcrete) { if (!KeyValueExists(keySelector(item), item, right, comparer)) { return false; } }
            }
            else if (right is EasyDictionary<TKey, TValue> rightEasyConcrete)
            {
                var keySelector = rightEasyConcrete.KeySelector;
                foreach (var item in rightEasyConcrete) { if (!KeyValueExists(keySelector(item), item, left, comparer)) { return false; } }
            }
            else
            {
                foreach (var pair in left) { if (!KeyValuePairExists(pair, right, comparer)) { return false; } }
            }
            return true;
        }

        private static bool KeyValueExists<TKey, TValue>(TKey key, TValue value,
            IReadOnlyDictionary<TKey, TValue> dictionary, IEqualityComparer<TValue> comparer)
                => dictionary.TryGetValue(key, out var rightVal) && comparer.Equals(value, rightVal);

        private static bool KeyValuePairExists<TKey, TValue>(KeyValuePair<TKey, TValue> pair, 
            IReadOnlyDictionary<TKey, TValue> dictionary, IEqualityComparer<TValue> comparer)
                => KeyValueExists(pair.Key, pair.Value, dictionary, comparer);

        /// <summary>
        /// Returns a <see cref="NameValueCollection"/> as a Dictionary
        /// </summary>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection namedValueCollection)
            => namedValueCollection.AllKeys.ToDictionary(key => key, key => namedValueCollection[key]);

        /// <summary>
        /// Returns a <see cref="ConcurrentDictionary{TKey,TValue}"/> from an <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary) 
                => new ConcurrentDictionary<TKey, TValue>(dictionary);

        /// <summary>
        /// Returns a <see cref="ConcurrentDictionary{TKey,TValue}"/> from an <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
                => new ConcurrentDictionary<TKey, TValue>(dictionary, comparer);
    }
}