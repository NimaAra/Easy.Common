namespace Easy.Common
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// An abstraction which can update it's <see cref="Value"/> without resorting to locks.
    /// </summary>
    /// <typeparam name="T">The type of the value to be updated.</typeparam>
    public sealed class AtomicUpdater<T> where T : class
    {
        private T _primary, _secondary;

        /// <summary>
        /// Creates an instance of the <see cref="AtomicUpdater{T}"/>.
        /// </summary>
        [DebuggerStepThrough]
        public AtomicUpdater(T initialPrimary, T initialSecondary)
        {
            if (ReferenceEquals(initialPrimary, initialSecondary))
            {
                throw new InvalidOperationException(
                    $"{nameof(initialPrimary)} and {nameof(initialSecondary)} should not be the same object.");
            }

            _primary = initialPrimary;
            _secondary = initialSecondary;
        }

        /// <summary>
        /// Gets the latest value.
        /// </summary>
        public T Value => ReadFresh(ref _primary);

        /// <summary>
        /// Updates the <see cref="Value"/> atomically without locking.
        /// </summary>
        /// <param name="updater">The action which would update the <see cref="Value"/>.</param>
        [DebuggerStepThrough]
        public void Update(Func<T, T> updater)
        {
            // Let's first get the _secondary updated
            // Anyone reading the Value is will still get the _primary
            var latestSecondary = ReadFresh(ref _secondary);
            var updatedSecondary = updater(latestSecondary);
		
            // now atomically set the _primary to be the latest update value
            var primary = Interlocked.Exchange(ref _primary, updatedSecondary);
		
            // time to also update the old primary location to the latest value
            var updatedPrimary = updater(primary);
		
            _secondary = Interlocked.Exchange(ref _primary, updatedPrimary);
        }

        private static T ReadFresh(ref T location) => 
            Interlocked.CompareExchange(ref location, default, default);
    }
}