namespace Easy.Common.Interfaces;

using System.Collections.Generic;

/// <summary>
/// Specifies the contract for <see cref="ILinkedQueue{T}"/>.
/// </summary>
public interface ILinkedQueue<T> : ICollection<T>
{
    /// <summary>
    /// Adds an object to the end of this instance.
    /// </summary>
    void Enqueue(T item);

    /// <summary>
    /// Attempts to remove the object at the beginning of this instance and copy it to the <paramref name="result"/>.
    /// </summary>
    bool TryDequeue(out T? result);

    /// <summary>
    /// Returns a value that indicates whether there is an object at the beginning of this instance and if one
    /// is present, copies it to the <paramref name="result"/>. The object is not removed from the instance.
    /// </summary>
    bool TryPeek(out T? result);

    /// <summary>
    /// Removes the item at the specified <paramref name="index"/> from this instance.
    /// </summary>
    bool RemoveAt(int index);
}