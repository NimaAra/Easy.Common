namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Easy.Common.Interfaces;

    /// <summary>
    /// A dictionary whose <typeparamref name="TKey"/> can be defined as a delegate over 
    /// any property of the <typeparamref name="TValue"/>. 
    /// <remarks>
    /// <para>This class is not thread-safe.</para>
    /// <para>The keys associated to the values must not be changed after insertion.</para>
    /// </remarks> 
    /// </summary>
    public sealed class EasyDictionary<TKey, TValue> : IEasyDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
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
            IEqualityComparer<TKey> comparer = null)
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
            IEqualityComparer<TKey> comparer = null)
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
            IEqualityComparer<TKey> comparer = null) 
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
            IEqualityComparer<TKey> comparer = null)
        {
            KeySelector = Ensure.NotNull(keySelector, nameof(keySelector));
            _dictionary = new Dictionary<TKey, TValue>((int)capacity, comparer);
        }

        /// <summary>
        /// Gets the delegate used to select the key against which the item will be stored.
        /// </summary>
        public Func<TValue, TKey> KeySelector { get; }

        /// <summary>
        /// Gets the count of items in the collection.
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Gets the flag indicating whether the <see cref="ICollection{TValue}"/> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{TKey}"/> that is used to determine 
        /// equality of keys for the dictionary.
        /// </summary>
        public IEqualityComparer<TKey> Comparer => _dictionary.Comparer;

        /// <summary>
        /// Gets the keys stored in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys => _dictionary.Keys;

        /// <summary>
        /// Gets the items stored as values stored in the dictionary.
        /// </summary>
        public ICollection<TValue> Values => _dictionary.Values;

        /// <summary>
        /// Gets the keys stored in the dictionary.
        /// </summary>
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _dictionary.Keys;

        /// <summary>
        /// Gets the items stored as values stored in the dictionary.
        /// </summary>
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _dictionary.Values;

        /// <summary>
        /// Gets the value associated with the given <paramref name="key"/>.
        /// </summary>
        public TValue this[TKey key] => _dictionary[key];

        /// <summary>
        /// Adds the given <paramref name="value"/> to the dictionary.
        /// </summary>
        public void Add(TValue value) => _dictionary.Add(KeySelector(value), value);

        /// <summary>
        /// Adds the <paramref name="value"/> if it does not already exist or replaces the existing value.
        /// <returns>
        /// <c>True</c> if the <paramref name="value"/> was added and <c>False</c> otherwise.
        /// </returns>
        /// </summary>
        public bool AddOrReplace(TValue value)
        {
            var key = KeySelector(value);
            
            if (!_dictionary.ContainsKey(key))
            {
                Add(value);
                return true;
            }

            if (_dictionary.ContainsValue(value)) { return false; }

            _dictionary[key] = value;
            return true;
        }

        /// <summary>
        /// Removes the given <paramref name="key"/> from the dictionary.
        /// </summary>
        public bool Remove(TKey key) => _dictionary.Remove(key);

        /// <summary>
        /// Removes the given <paramref name="value"/> from the dictionary.
        /// </summary>
        public bool Remove(TValue value)
        {
            var key = KeySelector(value);
            if (!_dictionary.ContainsKey(key) || !_dictionary.ContainsValue(value))
            {
                return false;
            }
            
            return _dictionary.Remove(key);
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear() => _dictionary.Clear();

        /// <summary>
        /// Determines whether the given <paramref name="value"/> exists.
        /// </summary>
        public bool Contains(TValue value) => 
            _dictionary.ContainsKey(KeySelector(value)) && _dictionary.ContainsValue(value);

        /// <summary>
        /// Determines whether the given <paramref name="key"/> exists.
        /// </summary>
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        /// <summary>
        /// Attempts to get the value associated with the specified <paramref name="key"/>.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{TValue}"/> to 
        /// the given <paramref name="array"/> starting at the given <paramref name="startIndex"/>.
        /// </summary>
        public void CopyTo(TValue[] array, int startIndex) => _dictionary.Values.CopyTo(array, startIndex);

        /// <summary>
        /// Return an enumerator that iterates through the dictionary.
        /// </summary>
        /// <returns></returns>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() 
            => _dictionary.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<TValue> GetEnumerator() => _dictionary.Values.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the sequence.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        private void PopulateFrom(IEnumerable<TValue> sequence)
        {
            switch (sequence)
            {
                case IList<TValue> indexible:
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < indexible.Count; i++) { Add(indexible[i]); }
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
            } else
            {
                foreach (var pair in dictionary) { Add(pair.Value); }
            }
        }
    }
}