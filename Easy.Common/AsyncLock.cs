namespace Easy.Common;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// An abstraction for a lock which can be used in an asynchronous scenarios.
/// </summary>
public sealed class AsyncLock : AsyncSemaphore
{
    /// <summary>
    /// Creates an instance of <see cref="AsyncLock"/>.
    /// </summary>
    public AsyncLock() : base(maxConcurrency: 1) { }
}

/// <summary>
/// An abstraction for a semaphore which can be used in asynchronous scenarios.
/// </summary>
public class AsyncSemaphore
{
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Creates an instance of <see cref="AsyncSemaphore"/>.
    /// </summary>
    public AsyncSemaphore(int maxConcurrency = 1) =>
        _semaphore = new(maxConcurrency, maxConcurrency);

    /// <summary>
    /// Attempts to acquire a lock within the given <paramref name="timeout"/> period; If succeeds an <see cref="IDisposable"/> is
    /// returned which on disposal releases the lock.
    /// </summary>
    /// <exception cref="TimeoutException">Thrown in case of a failure to acquire the lock.</exception>
    public async Task<IDisposable> Acquire(TimeSpan timeout)
    {
        bool timedOut = !await _semaphore.WaitAsync(timeout).ConfigureAwait(false);

        if (timedOut)
        {
            throw new TimeoutException("The request to semaphore timed out after: " + timeout);
        }

        return Disposable.Create(() => _semaphore.Release());
    }
}