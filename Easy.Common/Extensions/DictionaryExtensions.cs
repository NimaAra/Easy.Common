namespace Easy.Common.Extensions
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets the value associated with the specified key or the <paramref name="defaultValue"/> if it does not exist.
        /// </summary>
        /// <param name="dictionary">The Source Dictionary</param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The default value to return if an item with the specified <paramref name="key"/> does not exist.</param>
        /// <returns>The value associated with the specified key or the <paramref name="defaultValue"/> if it does not exist.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;

            if (dictionary.TryGetValue(key, out value)) { return value; }

            return defaultValue;
        }

        /// <summary>
        /// Returns a <see cref="NameValueCollection"/> as a Dictionary
        /// </summary>
        /// <param name="namedValueCollection"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection namedValueCollection)
        {
            Ensure.NotNull(namedValueCollection, nameof(namedValueCollection));

            return namedValueCollection.AllKeys.ToDictionary(key => key, key => namedValueCollection[key]);
        }

        /// <summary>
        /// Returns a <see cref="ConcurrentDictionary{TKey,TValue}"/> from an <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The dictionary key</typeparam>
        /// <typeparam name="TValue">The dictionary value</typeparam>
        /// <param name="source">The source <see cref="IDictionary{TKey,TValue}"/></param>
        /// <returns>The <see cref="ConcurrentDictionary{TKey,TValue}"/></returns>
        public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            Ensure.NotNull(source, nameof(source));
            return new ConcurrentDictionary<TKey, TValue>(source);
        }
    }

}