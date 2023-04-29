namespace Easy.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>
/// </summary>
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
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
    public static IEnumerable<T> GetPage<T>(this IEnumerable<T> sequence, uint pageIndex, uint pageSize) => 
        sequence.Skip((int)pageIndex * (int)pageSize).Take((int)pageSize);

    /// <summary>
    /// Validates that the <paramref name="sequence"/> is not null and contains items.
    /// </summary>
    [DebuggerStepThrough]
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? sequence) => 
        sequence is not null && sequence.Any();

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
    public static string ToCharSeparated<T>(this IEnumerable<T> sequence, char delimiter) => 
        ToStringSeparated(sequence, delimiter.ToString());

    /// <summary>
    /// Converts <paramref name="sequence"/> to a <c>Comma</c> separated string.
    /// </summary>
    [DebuggerStepThrough]
    public static string ToCommaSeparated<T>(this IEnumerable<T> sequence) => 
        ToCharSeparated(sequence, ',');

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
    public static T? SelectRandom<T>(this IEnumerable<T> sequence)
    {
        Random random = Random.Shared;
        return sequence.SelectRandom(random);
    }

    /// <summary>
    /// Selects a random element from an Enumerable with only one pass (O(N) complexity); 
    /// It contains optimizations for arguments that implement ICollection{T} by using the 
    /// Count property and the ElementAt LINQ method. The ElementAt LINQ method itself contains 
    /// optimizations for <see cref="IList{T}"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T? SelectRandom<T>(this IEnumerable<T> sequence, Random random)
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
    /// Returns a <see cref="EasyDictionary{TKey,TValue}"/> for the given <paramref name="sequence"/>
    /// based on the <paramref name="keySelector"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static EasyDictionary<TKey, TValue> ToEasyDictionary<TKey, TValue>(this IEnumerable<TValue> sequence, 
        Func<TValue, TKey> keySelector) where TKey : notnull => 
            ToEasyDictionary(sequence, keySelector, EqualityComparer<TKey>.Default);
        
    /// <summary>
    /// Returns a <see cref="EasyDictionary{TKey,TValue}"/> for the given <paramref name="sequence"/>
    /// based on the <paramref name="keySelector"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static EasyDictionary<TKey, TValue> ToEasyDictionary<TKey, TValue>(this IEnumerable<TValue> sequence, 
        Func<TValue, TKey> keySelector, 
        IEqualityComparer<TKey> comparer) where TKey : notnull => new(keySelector, sequence, comparer);

    /// <summary>
    /// Attempts to convert the given <paramref name="sequence"/> to an <see cref="IList{T}"/>
    /// by casting it first and if not successful then calling <c>ToList()</c>.
    /// </summary>
    [DebuggerStepThrough]
    public static IList<T> SpeculativeToList<T>(this IEnumerable<T> sequence) => sequence as IList<T> ?? sequence.ToList();

    /// <summary>
    /// Attempts to convert the given <paramref name="sequence"/> to an <see cref="IReadOnlyList{T}"/>
    /// by casting it first and if not successful then calling <c>ToList()</c>.
    /// </summary>
    [DebuggerStepThrough]
    public static IReadOnlyList<T> SpeculativeToReadOnlyList<T>(this IEnumerable<T> sequence) => sequence as IReadOnlyList<T> ?? sequence.ToList();

    /// <summary>
    /// Attempts to convert the given <paramref name="sequence"/> to an <see cref="T:T[]"/>
    /// by casting it first and if not successful then calling <c>ToArray()</c>.
    /// </summary>
    [DebuggerStepThrough]
    public static T[] SpeculativeToArray<T>(this IEnumerable<T> sequence) => sequence as T[] ?? sequence.ToArray();

    /// <summary>
    /// Converts an Enumerable into a read-only collection
    /// </summary>
    [DebuggerStepThrough]
    public static IEnumerable<T> ToReadOnlySequence<T>(this IEnumerable<T> sequence)
    {
        Ensure.NotNull(sequence, nameof(sequence));
        return sequence is IReadOnlyList<T> ? sequence : sequence.Skip(0);
    }

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

    /// <summary>
    /// Batches the source sequence into sized buckets.
    /// </summary>
    /// <typeparam name="TSource">Type of elements in <paramref name="source"/> sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="size">Size of buckets.</param>
    /// <returns>A sequence of equally sized buckets containing elements of the source collection.</returns>
    /// <remarks>
    /// This operator uses deferred execution and streams its results (buckets and bucket content).
    /// </remarks>
    [DebuggerStepThrough]
    public static IEnumerable<TSource[]> Batch<TSource>(this IEnumerable<TSource> source, uint size) 
    {
        Ensure.NotNull(source, nameof(source));
        Ensure.That<ArgumentOutOfRangeException>(size > 0, nameof(size));

        TSource[]? bucket = null;
        var count = 0;

        if (source is IReadOnlyList<TSource> indexibale)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < indexibale.Count; i++)
            {
                bucket ??= new TSource[size];

                TSource item = indexibale[i];

                bucket[count++] = item;

                if (count != size) { continue; }

                yield return bucket;

                bucket = null;
                count = 0;
            }
        } 
        else
        {
            foreach (var item in source)
            {
                bucket ??= new TSource[size];

                bucket[count++] = item;

                if (count != size) { continue; }

                yield return bucket;

                bucket = null;
                count = 0;
            }
        }

        if (bucket is null || count <= 0) { yield break; }
            
        Array.Resize(ref bucket, count);
        yield return bucket;
    }
}