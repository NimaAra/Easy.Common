namespace Easy.Common
{
    using System;

    /// <summary>
    /// Provides a helper class for implementing <see cref="System.IEquatable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to provide equability</typeparam>
    public abstract class Equatable<T> : IEquatable<T>
    {
        /// <summary>
        /// Provides the hash code for the object.
        /// </summary>
        /// <returns></returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Determines whether this object is equal <paramref name="other"/>.
        /// </summary>
        public virtual bool Equals(T other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// Determines whether this object is equal <paramref name="obj"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((T)obj);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(Equatable<T> left, Equatable<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(Equatable<T> left, Equatable<T> right)
        {
            return !Equals(left, right);
        }
    }
}