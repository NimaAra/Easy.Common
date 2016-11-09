namespace Easy.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// A class similar to <see cref="ThreadLocal{T}"/> with the ability to dispose the <typeparamref name="T"/>.
    /// <para><see href="http://stackoverflow.com/a/7670762/1226568"/>.</para>
    /// </summary>
    public sealed class ThreadLocalDisposable<T> : IDisposable where T : IDisposable
    {
        private readonly ConcurrentBag<T> _values;
        private readonly ThreadLocal<T> _threadLocal;

        /// <summary>
        /// Create an instance of the <see cref="ThreadLocalDisposable{T}"/>.
        /// </summary>
        public ThreadLocalDisposable(Func<T> valueFactory)
        {
            _values = new ConcurrentBag<T>();
            _threadLocal = new ThreadLocal<T>(() =>
            {
                var value = valueFactory();
                _values.Add(value);
                return value;
            });
        }

        /// <summary>
        /// Gets whether the <c>Value</c> is initialized on the current thread.
        /// </summary>
        public bool IsValueCreated => _threadLocal.IsValueCreated;

        /// <summary>
        /// Gets the <c>Value</c> of this instance for the current thread.
        /// </summary>
        public T Value => _threadLocal.Value;

        /// <summary>
        /// Creates and returns the <see cref="string"/> representation of this instance for the current thread.
        /// </summary>
        public override string ToString()
        {
            return _threadLocal.ToString();
        }

        /// <summary>
        /// Releases all the resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            _threadLocal.Dispose();
            Array.ForEach(_values.ToArray(), v => v.Dispose());
        }
    }
}