namespace Easy.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Easy.Common.Extensions;
    using Easy.Common.Interfaces;

    /// <summary>
    /// A generic thread safe high performance object pool.
    /// </summary>
    public sealed class EasyPool : IObjectPool
    {
        private readonly Dictionary<Type, Pool> _pools;
        private Dictionary<Type, Pool> Pools
        {
            get
            {
                if (_disposed) { throw new ObjectDisposedException("The pool has been disposed."); }
                return _pools;
            }
        }

        private bool _disposed;

        /// <summary>
        /// Initializes an instance of the <see cref="EasyPool"/>
        /// </summary>
        public EasyPool()
        {
            _pools = new Dictionary<Type, Pool>();
        }

        /// <summary>
        /// Returns the total number of registered types in the <see cref="_pools"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the underlying pool has been disposed</exception>
        public uint TotalRegistrations => (uint)Pools.Count;

        /// <summary>
        /// Registers with the pool the function to create a new instance of <typeparamref name="{T}"/> 
        /// and the <paramref name="maximumCount"/> of objects to create in the pool.
        /// </summary>
        /// <param name="factory">The factory used to create a new instance of <typeparamref name="{T}"/></param>
        /// <param name="maximumCount">The maximum number of objects to store in the pool</param>
        public void Register<T>(Func<T> factory, uint maximumCount) where T : class, IPoolableObject
        {
            Ensure.NotNull(factory, nameof(factory));

            using (Pools.Lock(5.Seconds()))
            {
                Pools.Add(typeof(T), new Pool(factory, maximumCount));
            }
        }

        /// <summary>
        /// Returns an object of type <typeparamref name="{T}"/> from the pool.
        /// </summary>
        /// <returns>Object of type <typeparamref name="{T}"/></returns>
        public T Get<T>() where T : class, IPoolableObject
        {
            Pool pool;
            if (!Pools.TryGetValue(typeof(T), out pool))
            {
                throw new InvalidOperationException("The type is not registered with the pool, cannot build the object. Type: " + typeof(T));
            }

            IPoolableObject result;
            if (pool.Bag.TryTake(out result)) { return result as T; }

            result = pool.Factory();
            result.SetPoolManager(this);
            return result as T;
        }

        /// <summary>
        /// Puts an object of type <typeparamref name="{T}"/> back in the pool.
        /// </summary>
        /// <param name="item">Object of type <typeparamref name="{T}"/></param>
        public void Put<T>(T item) where T : class, IPoolableObject
        {
            Ensure.NotNull(item, nameof(item));

            Pool pool;
            if (!Pools.TryGetValue(typeof(T), out pool) || pool.Count >= pool.MaximumCount)
            {
                return;
            }

            pool.Bag.Add(item);
        }

        /// <summary>
        /// Returns the total number of instances of the <typeparamref name="{T}"/>
        /// currently in the pool.
        /// </summary>
        public uint GetCountOfObjectsInThePool<T>() where T : class, IPoolableObject
        {
            Pool pool;
            if (!Pools.TryGetValue(typeof(T), out pool)) { return 0; }

            return (uint)pool.Count;
        }

        /// <summary>
        /// Consumes and disposes every object in the pool.
        /// </summary>
        public void Dispose()
        {
            using (_pools.Lock(30.Seconds()))
            {
                Pools.Values.ForEach(pool => pool.Dispose());
                Pools.Clear();
                _disposed = true;
            }
        }

        private sealed class Pool : IDisposable
        {
            public Func<IPoolableObject> Factory { get; }
            public uint MaximumCount { get; }
            public int Count => Bag.Count;
            public ConcurrentBag<IPoolableObject> Bag { get; }
            public Pool(Func<IPoolableObject> factory, uint maximumCount)
            {
                Bag = new ConcurrentBag<IPoolableObject>();
                Factory = factory;
                MaximumCount = maximumCount;
            }

            public void Dispose()
            {
                IPoolableObject removed;
                while (Bag.TryTake(out removed)) { /* empty */ }
            }
        }
    }
}