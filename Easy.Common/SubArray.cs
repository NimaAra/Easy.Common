namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An object which represents a portion (segment) of an array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    public sealed class SubArray<T> : IEnumerable<T>
    {
        /// <summary>
        /// The segment representing a portion of the array.
        /// </summary>
        public ArraySegment<T> Segment { get; }

        /// <summary>
        /// Creates an instance of the <see cref="SubArray{T}"/>.
        /// </summary>
        /// <param name="array">The source array.</param>
        /// <param name="offset">The zero based index of the first element in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        public SubArray(T[] array, int offset, int count)
        {
            Segment = new ArraySegment<T>(array, offset, count);
        }
        /// <summary>
        /// Gets the number of elements in the range.
        /// </summary>
        public int Length => Segment.Count;

        /// <summary>
        /// Gets the element stored at the given zero based index.
        /// </summary>
        public T this[int index] => Segment.Array[Segment.Offset + index];

        /// <summary>
        /// Creates an array from this instance.
        /// </summary>
        public T[] ToArray()
        {
            var temp = new T[Segment.Count];
            Array.Copy(Segment.Array, Segment.Offset, temp, 0, Segment.Count);
            return temp;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> from this instance.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = Segment.Offset; i < Segment.Offset + Segment.Count; i++)
            {
                yield return Segment.Array[i];
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator"/> from this instance.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}