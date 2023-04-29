namespace Easy.Common;

using Easy.Common.Interfaces;
using System;
using System.Collections.Concurrent;

/// <summary>
/// A generic thread-safe object pool.
/// </summary>
/// <typeparam name="T">The type of object to pool.</typeparam>
public sealed class EasyPool<T> : IEasyPool<T> where T : class
{
    private readonly ConcurrentBag<T> _pool;
    private readonly Func<T> _factory;
    private readonly Action<T>? _reset;
    private readonly uint _maxCount;

    /// <summary>
    /// Creates an instance of the <see cref="EasyPool{T}"/>.
    /// </summary>
    /// <param name="factory">
    /// The factory used to create an instance of <typeparamref name="T"/>
    /// </param>
    /// <param name="reset">The delegate used to reset the item returning to the pool.</param>
    /// <param name="maxCount">The maximum number of objects to store in the pool</param>
    public EasyPool(Func<T> factory, Action<T>? reset, uint maxCount)
    {
        _factory = Ensure.NotNull(factory, nameof(factory));
        _reset = reset;
        _maxCount = maxCount;

        _pool = new ConcurrentBag<T>();
    }

    /// <summary>
    /// Gets the count of items in the pool.
    /// </summary>
    public uint Count => (uint)_pool.Count;

    /// <summary>
    /// Gets an item from the pool or creates a new one if none exists.
    /// </summary>
    public T Rent() => _pool.TryTake(out T? item) ? item : _factory();

    /// <summary>
    /// Returns an item to the pool.
    /// </summary>
    /// <param name="item">The item to pool.</param>
    /// <param name="reset">
    /// The flag indicating whether the pool should reset the item to its default state.
    /// </param>
    /// <returns><c>True</c> if added or <c>False</c> if discarded</returns>
    public bool Return(T item, bool reset = true)
    {
        if (reset) { _reset?.Invoke(item); }
        if (_pool.Count >= _maxCount) { return false; }

        _pool.Add(item);
        return true;
    }

    /// <summary>
    /// Clears the pool.
    /// </summary>
    public void Dispose()
    {
        while (_pool.TryTake(out T _)) { /* ignore */ }
    }
}