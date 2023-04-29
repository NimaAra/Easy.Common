namespace Easy.Common;

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
    public virtual bool Equals(T? other) => other is { } notNull && notNull.GetHashCode() == GetHashCode();

    /// <summary>
    /// Determines whether this object is equal <paramref name="obj"/>.
    /// </summary>
    public override bool Equals(object? obj) => obj is T other && Equals(other);

    /// <summary>
    /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
    /// </summary>
    public static bool operator ==(Equatable<T> left, Equatable<T> right) => Equals(left, right);

    /// <summary>
    /// Determines whether the given <paramref name="left"/> is equal <paramref name="right"/>.
    /// </summary>
    public static bool operator !=(Equatable<T> left, Equatable<T> right) => !Equals(left, right);
}