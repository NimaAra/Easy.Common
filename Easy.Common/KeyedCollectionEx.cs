namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A dictionary of <typeparamref name="TItem"/> whose <typeparamref name="TKey"/> can be defined as a delegate at the time of initialization. 
    /// <remarks>
    /// This class is not thread-safe.
    /// <para><code>var myDic = new KeyedCollectionEx&lt;int, Person>(p => p.Name);</code></para>
    /// </remarks> 
    /// </summary>
    /// <typeparam name="TKey">Key to be used as the HashKey</typeparam>
    /// <typeparam name="TItem">Item to be stored as Value</typeparam>
    [Obsolete("Use EasyDictionary<TKey, TValue> instead.")]
    public class KeyedCollectionEx<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private readonly Func<TItem, TKey> _getKeyForItemFunc;

        /// <summary>
        /// Creates an instance of the <see cref="KeyedCollectionEx{TKey,TItem}"/>.
        /// </summary>
        /// <param name="keySelector">The selector used to select the key for the collection.</param>
        public KeyedCollectionEx(Func<TItem, TKey> keySelector) => 
            _getKeyForItemFunc = Ensure.NotNull(keySelector, nameof(keySelector));

        /// <summary>
        /// Creates an instance of the <see cref="KeyedCollectionEx{TKey,TItem}"/>.
        /// </summary>
        /// <param name="keySelector">The selector used to select the key for the collection.</param>
        /// <param name="comparer">
        /// The implementation of the <see cref="IEqualityComparer{T}"/> generic 
        /// interface to use when comparing keys, or null to use the default equality 
        /// comparer for the type of the key, obtained from <see cref="EqualityComparer{T}.Default"/>.
        /// </param>
        public KeyedCollectionEx(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(comparer) 
            => _getKeyForItemFunc = Ensure.NotNull(keySelector, nameof(keySelector));

        /// <summary>
        /// Gets the keys stored in the instance.
        /// </summary>
        public ICollection<TKey> Keys => Dictionary?.Keys ?? new List<TKey>(0);

        /// <summary>
        /// Gets the values stored in the instance.
        /// </summary>
        public ICollection<TItem> Values => Dictionary?.Values ?? new List<TItem>(0);

        /// <summary>
        /// Gets the key for the given <paramref name="item"/>.
        /// </summary>
        protected override TKey GetKeyForItem(TItem item) => _getKeyForItemFunc(item);

        /// <summary>
        /// Attempts to return the value for the given <paramref name="key"/>.
        /// </summary>
        public bool TryGet(TKey key, out TItem value)
        {
            if (Contains(key))
            {
                value = this[key];
                return true;
            }
            
            value = default(TItem);
            return false;
        }

        /// <summary>
        /// Adds the <paramref name="key"/> and <paramref name="value"/> if the <paramref name="key"/> 
        /// does not already exists and returns the inserted value.
        /// </summary>
        public TItem GetOrAdd(TKey key, TItem value)
        {
            if (TryGet(key, out var result)) { return result; }

            Add(value);
            return value;
        }

        /// <summary>
        /// Adds the <paramref name="key"/> and the value created by <paramref name="valueCreator"/> 
        /// if the <paramref name="key"/> does not already exists and returns the inserted value.
        /// </summary>
        public TItem GetOrAdd(TKey key, Func<TItem> valueCreator)
        {
            if (TryGet(key, out var result)) { return result; }

            var value = valueCreator();
            Add(value);
            result = value;
            return result;
        }
    }
}