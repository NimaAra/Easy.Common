namespace Easy.Common;

using Easy.Common.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An abstraction for representing a generic linked queue.
/// <remarks>
/// As per <see href="https://stackoverflow.com/a/24553394/1226568"/>
///           Queue          List          LinkedList
/// Enqueue:  O(1)/O(n)*     O(1)/O(n)*    O(1)
/// Dequeue:  O(1)           O(n)          O(1)
/// Remove :  n/a            O(n)          O(n)
/// </remarks>
/// </summary>
public sealed class LinkedQueue<T> : ILinkedQueue<T>
{
    private readonly LinkedList<T> _items;

    /// <summary>
    /// Creates a new instance of <see cref="LinkedQueue{T}"/>.
    /// </summary>
    public LinkedQueue() => _items = new();

    /// <summary>
    /// Creates a new instance of <see cref="LinkedQueue{T}"/>.
    /// </summary>
    public LinkedQueue(IEnumerable<T> collection) => _items = new(collection);

    /// <summary>
    /// Gets a value indicating whether this instance is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the number of nodes actually contained in this instance.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Adds an object to the end of this instance.
    /// </summary>
    public void Enqueue(T item) => _items.AddLast(item);

    /// <summary>
    /// Attempts to remove the object at the beginning of this instance and copy it to the <paramref name="result"/>.
    /// </summary>
    public bool TryDequeue(out T? result)
    {
        if (_items.First is null)
        {
            result = default;
            return false;
        }
            
        result = _items.First.Value;
        _items.RemoveFirst();

        return true;
    }

    /// <summary>
    /// Returns a value that indicates whether there is an object at the beginning of this instance and if one
    /// is present, copies it to the <paramref name="result"/>. The object is not removed from the instance.
    /// </summary>
    public bool TryPeek(out T? result)
    {
        if (_items.First is null)
        {
            result = default;
            return false;
        }

        result = _items.First.Value;
        return true;
    }

    /// <summary>
    /// Removes the item at the specified <paramref name="index"/> from this instance.
    /// </summary>
    public bool RemoveAt(int index) => Remove(_items.Skip(index).First());

    /// <summary>
    /// Adds the <paramref name="item"/> to the end of this instance.
    /// <remarks>Same behaviour as <see cref="Enqueue"/>.</remarks>
    /// </summary>
    public void Add(T item) => Enqueue(item);

    /// <summary>
    /// Removes the first occurrence of the specified <paramref name="item"/> from this instance.
    /// </summary>
    public bool Remove(T item) => _items.Remove(item);

    /// <summary>
    /// Removes all items from this instance.
    /// </summary>
    public void Clear() => _items.Clear();

    /// <summary>
    /// Determines whether <paramref name="item"/> is in this instance.
    /// </summary>
    public bool Contains(T item) => _items.Contains(item);

    /// <summary>
    /// Copies the entire instance to a compatible one-dimensional Array, starting at <paramref name="index"/>
    /// of the <paramref name="target"/>.
    /// </summary>
    public void CopyTo(T[] target, int index) => _items.CopyTo(target, index);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}