namespace Easy.Common.Interfaces
{
    using System;

    /// <summary>
    /// Specifies the contract which an object pool should implement.
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        /// Returns the total number of registered types in the pool.
        /// </summary>
        uint TotalRegistrations { get; }

        /// <summary>
        /// Registers the creation of the <typeparamref name="T"/> with the pool.
        /// </summary>
        /// <param name="factory">The factory used to create an instance of <typeparamref name="T"/></param>
        /// <param name="maximumCount">The maximum number of objects to store in the pool</param>
        void Register<T>(Func<T> factory, uint maximumCount) where T : class, IPoolableObject;

        /// <summary>
        /// Returns an instance of the <typeparamref name="T"/> from the pool.
        /// </summary>
        /// <typeparam name="T">Type of pooled object</typeparam>
        /// <returns>An instance of the <typeparamref name="T"/></returns>
        T Rent<T>() where T : class, IPoolableObject;

        /// <summary>
        /// Puts an instance of the <typeparamref name="T"/> back in the pool.
        /// </summary>
        /// <typeparam name="T">Type of pooled object</typeparam>
        /// <param name="item">An instance of the <typeparamref name="T"/></param>
        void Return<T>(T item) where T : class, IPoolableObject;

        /// <summary>
        /// Returns the total number of instances of the <typeparamref name="T"/>
        /// currently in the pool.
        /// </summary>
        uint GetCountOfObjectsInThePool<T>() where T : class, IPoolableObject;
    }

    /// <summary>
    /// Determines the contract for a poolable object.
    /// </summary>
    public interface IPoolableObject : IDisposable
    {
        /// <summary>
        /// Sets the <see cref="IObjectPool"/> for the <see cref="IPoolableObject"/>.
        /// </summary>
        /// <param name="pool">The object pool which stores the <see cref="IPoolableObject"/>.</param>
        void SetPoolManager(IObjectPool pool);
    }
}