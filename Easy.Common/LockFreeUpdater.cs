namespace Easy.Common
{
    using System;
    using System.Threading;

    /// <summary>
    /// An abstraction for updating its value without resorting to locks.
    /// </summary>
    /// <typeparam name="T">The type of the value to be updated.</typeparam>
    public sealed class LockFreeUpdater<T> where T : class
    {
        private T _primary, _secondary;

        /// <summary>
        /// Creates an instance of the <see cref="LockFreeUpdater{T}"/>.
        /// </summary>
        /// <param name="initializer">The function to initialize the <see cref="Value"/>.</param>
        public LockFreeUpdater(Func<T> initializer)
        {
            _primary = initializer();
            _secondary = initializer();
		
            if (ReferenceEquals(_primary, _secondary))
            {
                throw new InvalidOperationException(
                    $"The {nameof(initializer)} should not return the same object.");
            }
        }

        /// <summary>
        /// Gets the latest value.
        /// </summary>
        public T Value => Interlocked.CompareExchange(ref _primary, null, null);

        /// <summary>
        /// Updates the <see cref="Value"/> atomically without locking.
        /// </summary>
        /// <param name="updater">The action which would update the <see cref="Value"/>.</param>
        public void Update(Action<T> updater)
        {
            var latestSecondary = Interlocked.CompareExchange(ref _secondary, null, null);
		
            updater(latestSecondary);
		
            var oldPrimary = Interlocked.Exchange(ref _primary, latestSecondary);
		
            updater(oldPrimary);
		
            _secondary = Interlocked.Exchange(ref _primary, oldPrimary);
        }
    }
}