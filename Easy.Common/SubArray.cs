namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// An object which represents a portion (segment) of an array.
    /// </summary>
    /// <typeparam name="T">The type of the array.</typeparam>
    public struct SubArray<T> : IEnumerable<T>, IEquatable<SubArray<T>>
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

    #region  Equality

        /// <summary>
        /// Determines whether the specified <paramref name="other"/> structure is equal to the current instance.
        /// </summary>
        public bool Equals(SubArray<T> other)
        {
            return Segment.Equals(other.Segment);
        }

        /// <summary>
        /// Determines whether the specified other structure is equal to the current instance.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is SubArray<T> && Equals((SubArray<T>)obj);
        }

        /// <summary>
        /// Provides the equality operator override.
        /// </summary>
        /// <returns></returns>
        public static bool operator ==(SubArray<T> left, SubArray<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Provides the in-equality operator override.
        /// </summary>
        /// <returns></returns>
        public static bool operator !=(SubArray<T> left, SubArray<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns the hash code for the current instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Segment.GetHashCode();
        }

    #endregion

        /// <summary>
        /// Obtains the <see cref="string"/> representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Offset: {Segment.Offset.ToString()} | Count: {Segment.Count.ToString()} | Segment: {Segment.ToString()}";
        }
    }
}