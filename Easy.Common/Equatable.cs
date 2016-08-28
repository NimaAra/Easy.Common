namespace Easy.Common
{
    using System;

    /// <summary>
    /// Provides a helper class for implementing <see cref="System.IEquatable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to provide equability</typeparam>
    public abstract class Equatable<T> : IEquatable<T>
    {
        public abstract override int GetHashCode();

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

        public static bool operator ==(Equatable<T> left, Equatable<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Equatable<T> left, Equatable<T> right)
        {
            return !Equals(left, right);
        }
    }

}