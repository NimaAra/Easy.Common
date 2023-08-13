namespace Easy.Common.Interfaces;

using System;

/// <summary>
/// Specifies the contract for implementing an <see cref="IEasyPool{T}"/>.
/// </summary>
/// <typeparam name="T">The type of object to pool.</typeparam>
public interface IEasyPool<T> : IDisposable where T : class
{
    /// <summary>
    /// Gets the count of items in the pool.
    /// </summary>
    uint Count { get; }
        
    /// <summary>
    /// Gets an item from the pool or creates a new one if none exists.
    /// </summary>
    T Rent();

    /// <summary>
    /// Returns an item to the pool.
    /// </summary>
    /// <param name="item">The item to pool.</param>
    /// <param name="reset">
    /// The flag indicating whether the pool should reset the item to its default state.
    /// </param>
    /// <returns><c>True</c> if added or <c>False</c> if discarded</returns>
    bool Return(T item, bool reset = true);
}