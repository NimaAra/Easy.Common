// ReSharper disable PossibleMultipleEnumeration
namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Convenience method for retrieving a specific page of items within the given <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="T">The type of element in the sequence</typeparam>
        /// <param name="sequence">The sequence of elements</param>
        /// <param name="pageIndex">The 0-based index for the page</param>
        /// <param name="pageSize">The size of the elements in the page</param>
        /// <returns>The returned paged sequence</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> sequence, uint pageIndex, uint pageSize)
            => sequence.Skip((int)pageIndex * (int)pageSize).Take((int)pageSize);

        /// <summary>
        /// Converts an Enumerable into a read-only collection
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<T> ToReadOnlyCollection<T>(this IEnumerable<T> sequence)
        {
            Ensure.NotNull(sequence, nameof(sequence));
            return sequence.Skip(0);
        }

        /// <summary>
        /// Validates that the <paramref name="sequence"/> is not null and contains items.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> sequence) => 
            sequence != null && sequence.Any();

        /// <summary>
        /// Concatenates the members of a collection, using the specified separator between each item.
        /// </summary>
        [DebuggerStepThrough]
        public static string ToStringSeparated<T>(this IEnumerable<T> sequence, string separator)
        {
            Ensure.NotNull(sequence, nameof(sequence));

            if (!sequence.Any()) { return string.Empty; }
            return string.Join(separator, sequence);
        }

        /// <summary>
        /// Converts <paramref name="sequence"/> to a <paramref name="delimiter"/> separated <see cref="string"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static string ToCharSeparated<T>(this IEnumerable<T> sequence, char delimiter) 
            => ToStringSeparated(sequence, delimiter.ToString());

        /// <summary>
        /// Converts <paramref name="sequence"/> to a <c>Comma</c> separated string.
        /// </summary>
        [DebuggerStepThrough]
        public static string ToCommaSeparated<T>(this IEnumerable<T> sequence) 
            => ToCharSeparated(sequence, ',');

        /// <summary>
        /// Executes an <paramref name="action"/> for each of the items in the sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="action"></param>
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            Ensure.NotNull(action, nameof(action));
            foreach (var item in sequence) { action(item); }
        }

        /// <summary>
        /// Selects a random element from an Enumerable with only one pass (O(N) complexity); 
        /// It contains optimizations for arguments that implement ICollection{T} by using the 
        /// Count property and the ElementAt LINQ method. The ElementAt LINQ method itself contains 
        /// optimizations for <see cref="IList{T}"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static T SelectRandom<T>(this IEnumerable<T> sequence)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            return sequence.SelectRandom(random);
        }

        /// <summary>
        /// Selects a random element from an Enumerable with only one pass (O(N) complexity); 
        /// It contains optimizations for arguments that implement ICollection{T} by using the 
        /// Count property and the ElementAt LINQ method. The ElementAt LINQ method itself contains 
        /// optimizations for <see cref="IList{T}"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static T SelectRandom<T>(this IEnumerable<T> sequence, Random random)
        {
            if (sequence is ICollection<T> collection)
            {
                return collection.ElementAt(random.Next(collection.Count));
            }

            var count = 1;
            var selected = default(T);

            foreach (var element in sequence)
            {
                if (random.Next(count++) == 0)
                {
                    // Select the current element with 1/count probability
                    selected = element;
                }
            }
            return selected;
        }

        /// <summary>
        /// Randomizes a <paramref name="sequence"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> sequence)
        {
            Ensure.NotNull(sequence, nameof(sequence));
            return Randomize(sequence, new Random(Guid.NewGuid().GetHashCode()));
        }

        /// <summary>
        /// Randomizes a <paramref name="sequence"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> sequence, Random random)
        {
            Ensure.NotNull(sequence, nameof(sequence));
            Ensure.NotNull(random, nameof(random));

            var buffer = sequence.ToArray();
            for (var i = 0; i < buffer.Length; i++)
            {
                var j = random.Next(i, buffer.Length);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        /// <summary>
        /// Returns all the distinct elements of the given source where <c>distictness</c> is determined
        /// via a projection and the <see cref="EqualityComparer{TKey}.Default"/> for the given <paramref name="sequence"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> selector) 
            => DistinctBy(sequence, selector, EqualityComparer<TKey>.Default);

        /// <summary>
        /// Returns all the distinct elements of the given source where <c>distictness</c> is determined
        /// via a projection and the <paramref name="comparer"/> and the given <paramref name="comparer"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> sequence, 
            Func<T, TKey> selector, 
            IEqualityComparer<TKey> comparer)
        {
            var keys = new HashSet<TKey>(comparer);
            foreach (var item in sequence)
            {
                if (keys.Add(selector(item)))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="HashSet{T}"/> from the given <paramref name="sequence"/> 
        /// based on the <see cref="EqualityComparer{TKey}.Default"/> eliminating any duplicate values.
        /// </summary>
        [DebuggerStepThrough]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence)
            => ToHashSet(sequence, EqualityComparer<T>.Default);

        /// <summary>
        /// Returns a <see cref="HashSet{T}"/> from the given <paramref name="sequence"/> 
        /// base on the given <paramref name="comparer"/> eliminating any duplicate values.
        /// </summary>
        [DebuggerStepThrough]
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> sequence, IEqualityComparer<T> comparer) 
            => new HashSet<T>(sequence, comparer);

        /// <summary>
        /// Returns a <see cref="EasyDictionary{TKey,TValue}"/> for the given <paramref name="sequence"/>
        /// based on the <paramref name="keySelector"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static EasyDictionary<TKey, TValue> ToEasyDictionary<TKey, TValue>(
            this IEnumerable<TValue> sequence, Func<TValue, TKey> keySelector)
                => ToEasyDictionary(sequence, keySelector, EqualityComparer<TKey>.Default);
        
        /// <summary>
        /// Returns a <see cref="EasyDictionary{TKey,TValue}"/> for the given <paramref name="sequence"/>
        /// based on the <paramref name="keySelector"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static EasyDictionary<TKey, TValue> ToEasyDictionary<TKey, TValue>(
            this IEnumerable<TValue> sequence, 
            Func<TValue, TKey> keySelector, 
            IEqualityComparer<TKey> comparer) 
                => new EasyDictionary<TKey, TValue>(keySelector, sequence, comparer);

        /// <summary>
        /// Attempts to convert the given <paramref name="sequence"/> to an <see cref="IList{T}"/>
        /// by casting it first and if not successful then calling <c>ToList()</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static IList<T> SpeculativeToList<T>(this IEnumerable<T> sequence)
            => sequence as IList<T> ?? sequence.ToList();

        /// <summary>
        /// Attempts to convert the given <paramref name="sequence"/> to an <see cref="IReadOnlyList{T}"/>
        /// by casting it first and if not successful then calling <c>ToList()</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> SpeculativeToReadOnlyList<T>(this IEnumerable<T> sequence)
            => sequence as IReadOnlyList<T> ?? sequence.ToList();

        /// <summary>
        /// Attempts to convert the given <paramref name="sequence"/> to an <see cref="T:T[]"/>
        /// by casting it first and if not successful then calling <c>ToArray()</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static T[] SpeculativeToArray<T>(this IEnumerable<T> sequence)
            => sequence as T[] ?? sequence.ToArray();

        /// <summary>
        /// Allows exception handling when yield returning an IEnumerable
        /// <example>
        /// myList.HandleExceptionWhenYieldReturning{int}(e => 
        /// {
        ///     Logger.Error(e);
        ///     throw new SybaseException("Exception occurred", e);
        /// }, e => e is AseException || e is DbException);
        /// </example>
        /// </summary>
        /// <typeparam name="T">Type of data to enumerate.</typeparam>
        /// <param name="sequence">The sequence of <typeparamref name="T"/> which will be enumerated.</param>
        /// <param name="exceptionPredicate">The predicate specifying which exception(s) to handle.</param>
        /// <param name="actionToExecuteOnException">The action to which the handled exception will be passed to.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> HandleExceptionWhenYieldReturning<T>(
            this IEnumerable<T> sequence, 
            Func<Exception, bool> exceptionPredicate, 
            Action<Exception> actionToExecuteOnException)
        {
            Ensure.NotNull(exceptionPredicate, nameof(exceptionPredicate));
            Ensure.NotNull(actionToExecuteOnException, nameof(actionToExecuteOnException));

            var enumerator = sequence.GetEnumerator();

            while (true)
            {
                T result;
                try
                {
                    if (!enumerator.MoveNext()) { break; }
                    result = enumerator.Current;
                }
                catch (Exception e)
                {
                    if (exceptionPredicate(e))
                    {
                        actionToExecuteOnException(e);
                        yield break;
                    }
                    throw;
                }
                yield return result;
            }

            enumerator.Dispose();
        }
    }
}