namespace Easy.Common
{
    using System;
    using System.Threading;

    /// <summary>
    /// This is a helper class providing handy methods around locking
    /// </summary>
    public static class LockHelper
    {
        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <typeparam name="T">Type of variable to update</typeparam>
        /// <param name="field">The variable to be updated</param>
        /// <param name="updater">The functions that provides the updated value</param>
        public static void LockFreeUpdate<T>(ref T field, Func<T, T> updater) where T : class
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (ReferenceEquals(Interlocked.CompareExchange(ref field, updater(snapshot), snapshot), snapshot)) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <typeparam name="T">Type of variable to update</typeparam>
        /// <param name="field">The variable to be updated</param>
        /// <param name="newValue">The new value to replace the value at <paramref name="field"/></param>
        public static void LockFreeUpdate<T>(ref T field, T newValue) where T : class
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (ReferenceEquals(Interlocked.CompareExchange(ref field, newValue, snapshot), snapshot)) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="updater">The functions that provides the updated value</param>
        public static void LockFreeUpdate(ref Int32 field, Func<Int32, Int32> updater)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, updater(snapshot), snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="newValue">The new value to replace the value at <paramref name="field"/></param>
        public static void LockFreeUpdate(ref Int32 field, Int32 newValue)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, newValue, snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="updater">The functions that provides the updated value</param>
        public static void LockFreeUpdate(ref Int64 field, Func<Int64, Int64> updater)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, updater(snapshot), snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="newValue">The new value to replace the value at <paramref name="field"/></param>
        public static void LockFreeUpdate(ref Int64 field, Int64 newValue)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, newValue, snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="updater">The functions that provides the updated value</param>
        public static void LockFreeUpdate(ref Single field, Func<Single, Single> updater)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, updater(snapshot), snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="newValue">The new value to replace the value at <paramref name="field"/></param>
        public static void LockFreeUpdate(ref Single field, Single newValue)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, newValue, snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="updater">The functions that provides the updated value</param>
        public static void LockFreeUpdate(ref Double field, Func<Double, Double> updater)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, updater(snapshot), snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides lock free and atomic update of <paramref name="field"/>
        /// </summary>
        /// <param name="field">The variable to be updated</param>
        /// <param name="newValue">The new value to replace the value at <paramref name="field"/></param>
        public static void LockFreeUpdate(ref Double field, Double newValue)
        {
            var spinner = new SpinWait();

            while (true)
            {
                var snapshot = field;
                if (snapshot.Equals(Interlocked.CompareExchange(ref field, newValue, snapshot))) { return; }

                spinner.SpinOnce();
            }
        }

        /// <summary>
        /// Provides a lock with a timeout.
        /// </summary>
        /// <example>
        /// var locker = new object();
        /// using(locker.Lock(TimeSpan.FromSeconds(1)))
        /// {
        ///     sharedVariable++;
        /// }
        /// </example>
        /// <param name="obj">Object on which lock is taken.</param>
        /// <param name="timeout">Timeout for the lock.</param>
        /// <returns>A locker struct on which lock will be taken.</returns>
        public static Locker Lock(this object obj, TimeSpan timeout)
        {
            var lockTaken = false;

            try
            {
                Monitor.TryEnter(obj, timeout, ref lockTaken);
                if (lockTaken)
                {
                    return new Locker(obj);
                }
                throw new TimeoutException("Failed to acquire lock within the timeout period of: " + timeout);
            }
            catch
            {
                if (lockTaken)
                {
                    Monitor.Exit(obj);
                }
                throw;
            }
        }

        /// <summary>
        /// As part of the Lock extension method on <c>object</c>s 
        /// it provides a timeout mechanism for acquiring locks.
        /// </summary>
        public struct Locker : IDisposable
        {
            private readonly object _obj;

            /// <summary>
            /// Returns an instance of <see cref="Locker"/>.
            /// </summary>
            /// <param name="obj">The <c>object</c> on which lock is taken.</param>
            public Locker(object obj)
            {
                _obj = obj;
            }

            /// <summary>
            /// Releases any locks taken by this instance.
            /// </summary>
            public void Dispose()
            {
                Monitor.Exit(_obj);
            }
        }
    }

}