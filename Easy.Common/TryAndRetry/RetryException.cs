// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;

    /// <summary>
    /// The <see cref="RetryException"/> thrown by the <see cref="Retry"/> class.
    /// </summary>
    public sealed class RetryException : Exception
    {
        /// <summary>
        /// Creates an instance of the <see cref="RetryException"/>.
        /// </summary>
        /// <param name="retryCount">
        /// The number of attempts after which <paramref name="innerException"/> was thrown.
        /// </param>
        /// <param name="innerException">The inner exception.</param>
        public RetryException(uint retryCount, Exception innerException)
            : base($"Retry failed after: {retryCount} attempts.", innerException) => RetryCount = retryCount;

        /// <summary>
        /// Gets the number of attempts after which this exception was thrown.
        /// </summary>
        public uint RetryCount { get; }
    }
}