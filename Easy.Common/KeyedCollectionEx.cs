namespace Easy.Common
{
    using System;
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
    public class KeyedCollectionEx<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private readonly Func<TItem, TKey> _getKeyForItemFunc;

        /// <summary>
        /// Creates an instance of the <see cref="KeyedCollectionEx{TKey,TItem}"/>.
        /// </summary>
        /// <param name="keySelector">The selector used to select the key for the collection</param>
        public KeyedCollectionEx(Func<TItem, TKey> keySelector)
        {
            _getKeyForItemFunc = Ensure.NotNull(keySelector, nameof(keySelector));
        }

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
        /// Gets the key for the given <paramref name="item"/>.
        /// </summary>
        protected override TKey GetKeyForItem(TItem item)
        {
            return _getKeyForItemFunc(item);
        }
    }
}