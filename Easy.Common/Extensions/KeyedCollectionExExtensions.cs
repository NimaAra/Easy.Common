namespace Easy.Common.Extensions
{
    using System;

    /// <summary>
    /// Extension methods for <see cref="KeyedCollectionEx{TKey, TValue}"/>.
    /// </summary>
    public static class KeyedCollectionExExtensions
    {
        /// <summary>
        /// Adds the <paramref name="key"/> and <paramref name="value"/> to the <paramref name="keyedCollection"/>
        /// if the <paramref name="key"/> does not already exists and returns the inserted value.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this KeyedCollectionEx<TKey, TValue> keyedCollection, TKey key, TValue value)
        {
            if (!keyedCollection.TryGet(key, out TValue result))
            {
                keyedCollection.Add(value);
                return value;
            }
            return result;
        }

        /// <summary>
        /// Adds the <paramref name="key"/> and the value created by <paramref name="valueCreator"/> to 
        /// the <paramref name="keyedCollection"/> if the <paramref name="key"/> does not already exists 
        /// and returns the inserted value.
        /// </summary>
        public static TValue GetOrAdd<TKey, TValue>(this KeyedCollectionEx<TKey, TValue> keyedCollection, TKey key, Func<TValue> valueCreator)
        {
            if (!keyedCollection.TryGet(key, out TValue result))
            {
                var value = valueCreator();
                keyedCollection.Add(value);
                result = value;
            }
            return result;
        }
    }
}