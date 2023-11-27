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

    /// <inheritdoc/>
    public uint Count => (uint)_pool.Count;

    /// <inheritdoc/>
    public T Rent() => _pool.TryTake(out T? item) ? item : _factory();

    /// <inheritdoc/>
    public bool Return(T item, bool reset = true)
    {
        if (reset) { _reset?.Invoke(item); }
        if (_pool.Count >= _maxCount) { return false; }

        _pool.Add(item);
        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        while (_pool.TryTake(out T? _)) { /* ignore */ }
    }
}