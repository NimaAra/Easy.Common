namespace Easy.Common
{
    using System;
    using System.Collections;

    /// <summary>
    /// An implementation of a <c>Bloom filter</c> <see href="https://en.wikipedia.org/wiki/Bloom_filter"/>.
    /// </summary>
    /// <typeparam name="T">Type of the data to be stored.</typeparam>
    public class BloomFilter<T>
    {
        private readonly int _hashFunctionCount;
        private readonly BitArray _hashBits;
        private readonly Func<T, int> _getHashSecondary;

        /// <summary>
        /// Creates an instance of the <see cref="BloomFilter{T}"/>, specifying an error rate of 1/capacity,
        /// using the optimal size for the underlying data structure based on the desired capacity and error
        /// rate, as well as the optimal number of hash functions.
        /// A secondary hash function will be provided for you if your type <typeparamref name="T"/> is
        /// either <see cref="string"/> or <see cref="int"/>. Otherwise an exception will be thrown.
        /// If you are not using these types please use the overload that supports custom hash functions.
        /// </summary>
        /// <param name="capacity">
        /// The anticipated number of items to be added to the filter.
        /// More than this number of items can be added, but the error rate will exceed what is expected.
        /// </param>
        public BloomFilter(int capacity) : this(capacity, null) { }

        /// <summary>
        /// Creates an instance of the <see cref="BloomFilter{T}"/>, using the optimal size for the underlying
        /// data structure based on the desired capacity and error rate, as well as the optimal number of hash
        /// functions.
        /// A secondary hash function will be provided for you if your type T is either string or int.
        /// Otherwise an exception will be thrown. If you are not using these types please use the overload
        /// that supports custom hash functions.
        /// </summary>
        /// <param name="capacity">
        /// The anticipated number of items to be added to the filter. More than this number of items can
        /// be added, but the error rate will exceed what is expected.
        /// </param>
        /// <param name="errorRate">
        /// The acceptable false-positive rate (e.g., 0.01F = 1%)
        /// </param>
        public BloomFilter(int capacity, float errorRate) : this(capacity, errorRate, null) { }

        /// <summary>
        /// Creates an instance of the <see cref="BloomFilter{T}"/>, specifying an error rate of 1/capacity,
        /// using the optimal size for the underlying data structure based on the desired capacity and error
        /// rate, as well as the optimal number of hash functions.
        /// </summary>
        /// <param name="capacity">
        /// The anticipated number of items to be added to the filter. More than this number of items can
        /// be added, but the error rate will exceed what is expected.
        /// </param>
        /// <param name="hashFunction">
        /// The function to hash the input values. Do not use <see cref="object.GetHashCode"/>. If it is
        /// <c>null</c>, and <typeparamref name="T"/> is <see cref="string"/> or <see cref="int"/> a hash
        /// function will be provided for you.
        /// </param>
        public BloomFilter(int capacity, Func<T, int> hashFunction) 
            : this(capacity, BestErrorRate(capacity), hashFunction) { }

        /// <summary>
        /// Creates an instance of the <see cref="BloomFilter{T}"/>, using the optimal size for the underlying
        /// data structure based on the desired capacity and error rate, as well as the optimal number of hash
        /// functions.
        /// </summary>
        /// <param name="capacity">
        /// The anticipated number of items to be added to the filter. More than this number of items can be added,
        /// but the error rate will exceed what is expected.
        /// </param>
        /// <param name="errorRate">
        /// The acceptable false-positive rate (e.g., 0.01F = 1%)
        /// </param>
        /// <param name="hashFunction">
        /// The function to hash the input values. Do not use <see cref="object.GetHashCode"/>. If it is
        /// <c>null</c>, and <typeparamref name="T"/> is <see cref="string"/> or <see cref="int"/> a hash
        /// function will be provided for you.
        /// </param>
        public BloomFilter(int capacity, float errorRate, Func<T, int> hashFunction)
            : this(capacity, errorRate, hashFunction, BestM(capacity, errorRate), BestK(capacity, errorRate)) { }

        /// <summary>
        /// Creates an instance of the <see cref="BloomFilter{T}"/>.
        /// </summary>
        /// <param name="capacity">
        /// The anticipated number of items to be added to the filter. More than this number of items can be added,
        /// but the error rate will exceed what is expected.
        /// </param>
        /// <param name="errorRate">
        /// The acceptable false-positive rate (e.g., 0.01F = 1%)
        /// </param>
        /// <param name="hashFunction">
        /// The function to hash the input values. Do not use <see cref="object.GetHashCode"/>. If it is
        /// <c>null</c>, and <typeparamref name="T"/> is <see cref="string"/> or <see cref="int"/> a hash
        /// function will be provided for you.
        /// </param>
        /// <param name="m">
        /// The number of elements in the <see cref="BitArray"/>.
        /// </param>
        /// <param name="k">
        /// The number of hash functions to use.
        /// </param>
        public BloomFilter(int capacity, float errorRate, Func<T, int> hashFunction, int m, int k)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(capacity), capacity, "Value must be greater than 0.");
            }

            if (errorRate >= 1 || errorRate <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(errorRate), 
                    errorRate,
                    $"Value must be between 0 and 1 (exclusive) but it was {errorRate}");
            }

            if (m < 1)
            {
                throw new ArgumentOutOfRangeException(
                    $"The provided capacity and errorRate values would result in an array of length > int.MaxValue. Please reduce either of these values. Capacity: {capacity.ToString()}, Error rate: {errorRate}");
            }

            // set the secondary hash function
            if (hashFunction == null)
            {
                if (typeof(T) == typeof(string))
                {
                    _getHashSecondary = HashString;
                } else if (typeof(T) == typeof(int))
                {
                    _getHashSecondary = HashInt32;
                } else
                {
                    throw new ArgumentNullException(
                        nameof(hashFunction),
                        "Please provide a hash function for your type T, when T is not a String or Int.");
                }
            } else
            {
                _getHashSecondary = hashFunction;
            }

            _hashFunctionCount = k;
            _hashBits = new BitArray(m);
        }

        /// <summary>
        /// Gets the ratio of <c>False</c> to <c>True</c> bits in the filter.
        /// E.g., 1 true bit in a 10 bit filter means a truthiness of 0.1.
        /// </summary>
        public double Truthiness => (double) TrueBits() / _hashBits.Count;

        /// <summary>
        /// Adds a new item to the filter.
        /// <remarks>The item cannot be removed</remarks>
        /// </summary>
        public void Add(T item)
        {
            // start flipping bits for each hash of item
            var primaryHash = item.GetHashCode();
            var secondaryHash = _getHashSecondary(item);
            
            for (var i = 0; i < _hashFunctionCount; i++)
            {
                var hash = ComputeHash(primaryHash, secondaryHash, i);
                _hashBits[hash] = true;
            }
        }

        /// <summary>
        /// Checks for the existence of the item in the filter for a given probability.
        /// </summary>
        public bool Contains(T item)
        {
            var primaryHash = item.GetHashCode();
            var secondaryHash = _getHashSecondary(item);
            
            for (var i = 0; i < _hashFunctionCount; i++)
            {
                var hash = ComputeHash(primaryHash, secondaryHash, i);
                if (_hashBits[hash] == false)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The best k.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        /// <param name="errorRate"> The error rate. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private static int BestK(int capacity, float errorRate) => 
            (int) Math.Round(Math.Log(2.0) * BestM(capacity, errorRate) / capacity);

        /// <summary>
        /// The best m.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        /// <param name="errorRate"> The error rate. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private static int BestM(int capacity, float errorRate) => 
            (int) Math.Ceiling(capacity * Math.Log(errorRate, 1.0 / Math.Pow(2, Math.Log(2.0))));

        /// <summary>
        /// The best error rate.
        /// </summary>
        /// <param name="capacity"> The capacity. </param>
        /// <returns> The <see cref="float"/>. </returns>
        private static float BestErrorRate(int capacity)
        {
            var c = (float) (1.0 / capacity);
            if (c != 0)
            {
                return c;
            }

            // default
            // http://www.cs.princeton.edu/courses/archive/spring02/cs493/lec7.pdf
            return (float) Math.Pow(0.6185, int.MaxValue / capacity);
        }

        /// <summary>
        /// Hashes a 32-bit signed int using Thomas Wang's method v3.1
        /// <see href="http://www.concentric.net/~Ttwang/tech/inthash.htm"/>.
        /// <remarks>Runtime is suggested to be 11 cycles.</remarks>
        /// </summary>
        /// <param name="input">The integer to hash.</param>
        /// <returns>The hashed result.</returns>
        private static int HashInt32(T input)
        {
            var x = input as uint?;
            unchecked
            {
                x = ~x + (x << 15); // x = (x << 15) - x- 1, as (~x) + y is equivalent to y - x - 1 in two's complement representation
                x = x ^ (x >> 12);
                x = x + (x << 2);
                x = x ^ (x >> 4);
                x = x * 2057; // x = (x + (x << 3)) + (x<< 11);
                x = x ^ (x >> 16);
                return (int) x;
            }
        }

        /// <summary>
        /// Hashes a string using Bob Jenkin's "One At A Time" method from Dr. Dobbs
        /// <see href="http://burtleburtle.net/bob/hash/doobs.html"/>.
        /// <remarks>Runtime is suggested to be 9x+9, where x = input.Length.</remarks> 
        /// </summary>
        /// <param name="input">The string to hash.</param>
        /// <returns>The hashed result.</returns>
        private static int HashString(T input)
        {
            var s = input as string;
            var hash = 0;

            for (var i = 0; i < s.Length; i++)
            {
                hash += s[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

        /// <summary>
        /// The true bits.
        /// </summary>
        /// <returns> The <see cref="int"/>. </returns>
        private int TrueBits()
        {
            var output = 0;
            for (var i = 0; i < _hashBits.Count; i++)
            {
                if (_hashBits[i])
                {
                    output++;
                }
            }
            return output;
        }

        /// <summary>
        /// Performs Dillinger and Manolios double hashing. 
        /// </summary>
        /// <param name="primaryHash"> The primary hash. </param>
        /// <param name="secondaryHash"> The secondary hash. </param>
        /// <param name="i"> The i. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private int ComputeHash(int primaryHash, int secondaryHash, int i)
        {
            var resultingHash = (primaryHash + i * secondaryHash) % _hashBits.Count;
            return Math.Abs(resultingHash);
        }
    }
}