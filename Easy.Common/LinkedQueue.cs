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
/// * O(1) is typical case but sometimes it'll be O(n) (when internal array need to be resized).
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

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public int Count => _items.Count;

    /// <inheritdoc/>
    public void Enqueue(T item) => _items.AddLast(item);

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool RemoveAt(int index) => Remove(_items.Skip(index).First());

    /// <inheritdoc/>
    public void Add(T item) => Enqueue(item);

    /// <inheritdoc/>
    public bool Remove(T item) => _items.Remove(item);

    /// <inheritdoc/>
    public void Clear() => _items.Clear();

    /// <inheritdoc/>
    public bool Contains(T item) => _items.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(T[] target, int index) => _items.CopyTo(target, index);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}