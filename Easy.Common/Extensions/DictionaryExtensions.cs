namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
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
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryGetValue(key, out TValue result))
            {
                dictionary[key] = value;
                return value;
            }
            return result;
        }

        /// <summary>
        /// Adds the <paramref name="key"/> and the value created by <paramref name="valueCreator"/> to 
        /// the <paramref name="dictionary"/> if the <paramref name="key"/> does not already exists 
        /// and returns the inserted value.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueCreator)
        {
            if (!dictionary.TryGetValue(key, out TValue result))
            {
                var value = valueCreator();
                dictionary[key] = value;
                result = value;
            }
            return result;
        }
        
        /// <summary>
        /// Gets the value associated with the specified key or the <paramref name="defaultValue"/> if it does not exist.
        /// </summary>
        /// <param name="dictionary">The Source Dictionary</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The default value to return if an item with the specified <paramref name="key"/> does not exist.</param>
        /// <returns>The value associated with the specified key or the <paramref name="defaultValue"/> if it does not exist.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary.TryGetValue(key, out TValue value)) { return value; }
            return defaultValue;
        }

        /// <summary>
        /// Returns a <see cref="NameValueCollection"/> as a Dictionary
        /// </summary>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection namedValueCollection)
        {
            Ensure.NotNull(namedValueCollection, nameof(namedValueCollection));
            return namedValueCollection.AllKeys.ToDictionary(key => key, key => namedValueCollection[key]);
        }

        /// <summary>
        /// Returns a <see cref="ConcurrentDictionary{TKey,TValue}"/> from an <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            Ensure.NotNull(dictionary, nameof(dictionary));
            return new ConcurrentDictionary<TKey, TValue>(dictionary);
        }
    }
}