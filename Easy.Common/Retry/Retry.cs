// ReSharper disable once CheckNamespace
namespace Easy.Common;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Extensions;

/// <summary>
/// A helper class which provides retry logic for actions and delegates.
/// </summary>
public static class Retry
{
    private const int DEFAULT_RETRY_COUNT = 1;
        
    /// <summary>
    /// Retries the given <paramref name="func"/> in case of an exception of 
    /// type <typeparamref name="TEx"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task On<TEx>(Func<Task> func, params TimeSpan[] delays) 
        where TEx : Exception => On(func, e => e.IsExpectedException<TEx>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// <typeparamref name="TEx1"/> and <typeparamref name="TEx2"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task OnAny<TEx1, TEx2>(Func<Task> func, params TimeSpan[] delays)
        where TEx1 : Exception where TEx2 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/> and <typeparamref name="TEx3"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task OnAny<TEx1, TEx2, TEx3>(Func<Task> func, params TimeSpan[] delays)
        where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/> and <typeparamref name="TEx4"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task OnAny<TEx1, TEx2, TEx3, TEx4>(Func<Task> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>,
    /// <typeparamref name="TEx4"/> and <typeparamref name="TEx5"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task OnAny<TEx1, TEx2, TEx3, TEx4, TEx5>(Func<Task> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        where TEx5 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>,
    /// <typeparamref name="TEx4"/>, <typeparamref name="TEx5"/> and <typeparamref name="TEx6"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task OnAny<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6>(Func<Task> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        where TEx5 : Exception 
        where TEx6 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>,
    /// <typeparamref name="TEx4"/>, <typeparamref name="TEx5"/>, <typeparamref name="TEx6"/>
    /// and <typeparamref name="TEx7"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task OnAny<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7>(Func<Task> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        where TEx5 : Exception 
        where TEx6 : Exception 
        where TEx7 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// the <paramref name="exceptionPredicate"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static async Task On(
        Func<Task> func, Func<Exception, bool> exceptionPredicate, params TimeSpan[] delays)
    {
        bool hasDelays = delays.Length > 0;
        int retryCount = hasDelays ?  delays.Length : DEFAULT_RETRY_COUNT;
            
        for (var i = 0; i <= retryCount; i++)
        {
            try
            {
                await func().ConfigureAwait(false);
                return;
            } 
            catch (Exception e) when (i == retryCount)
            {
                throw new RetryException((uint)retryCount, e);
            }
            catch (Exception e) when (exceptionPredicate(e))
            {
                if (hasDelays)
                {
                    await Task.Delay(delays[i]).ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// the <paramref name="exceptionPredicate"/>.
    /// <param name="func">The factory for the task to be retried.</param>
    /// <param name="exceptionPredicate">The predicate indicating which exception to retry on.</param>
    /// <param name="delayFactory">The factory for returning delay period between retries.</param>
    /// <param name="cToken">The cancellation token for canceling the retries.</param>
    /// </summary>
    [DebuggerStepThrough]
    public static async Task On(
        Func<Task> func,
        Func<Exception, bool> exceptionPredicate, 
        Func<Exception, uint, TimeSpan> delayFactory,
        CancellationToken cToken)
    {
        uint failureCount = 0;
        while (true)
        {
            try
            {
                failureCount++;
                await func().ConfigureAwait(false);
                return;
            } catch (Exception e) when (exceptionPredicate(e))
            {
                try
                {
                    await Task.Delay(delayFactory(e, failureCount), cToken).ConfigureAwait(false);
                } catch (TaskCanceledException)
                {
                    throw new RetryException(failureCount - 1, e);
                }
            }
        }
    }

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of an exception of 
    /// type <typeparamref name="TEx"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> On<TEx, TResult>(Func<Task<TResult>> func, params TimeSpan[] delays) 
        where TEx : Exception 
        => On(func, e => e.IsExpectedException<TEx>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
    /// <typeparamref name="TEx1"/> and <typeparamref name="TEx2"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> OnAny<TEx1, TEx2, TResult>(
        Func<Task<TResult>> func, params TimeSpan[] delays)
        where TEx1 : Exception where TEx2 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/> and <typeparamref name="TEx3"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TResult>(
        Func<Task<TResult>> func, params TimeSpan[] delays)
        where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>
    /// and <typeparamref name="TEx4"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TEx4, TResult>(
        Func<Task<TResult>> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>,
    /// <typeparamref name="TEx4"/> and <typeparamref name="TEx5"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TEx4, TEx5, TResult>(
        Func<Task<TResult>> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        where TEx5 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>,
    /// <typeparamref name="TEx4"/>, <typeparamref name="TEx5"/> and <typeparamref name="TEx6"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TResult>(
        Func<Task<TResult>> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        where TEx5 : Exception 
        where TEx6 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
    /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>,
    /// <typeparamref name="TEx4"/>, <typeparamref name="TEx5"/>, <typeparamref name="TEx6"/>
    /// and <typeparamref name="TEx7"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7, TResult>(
        Func<Task<TResult>> func, params TimeSpan[] delays)
        where TEx1 : Exception 
        where TEx2 : Exception 
        where TEx3 : Exception 
        where TEx4 : Exception 
        where TEx5 : Exception 
        where TEx6 : Exception 
        where TEx7 : Exception 
        => On(func, e => e.IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7>(), delays);

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// the <paramref name="exceptionPredicate"/>.
    /// <remarks>
    /// If the given <paramref name="delays"/> is not supplied then the given 
    /// <paramref name="func"/> will be retried once.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static async Task<TResult> On<TResult>(
        Func<Task<TResult>> func, Func<Exception, bool> exceptionPredicate, params TimeSpan[] delays)
    {
        bool hasDelays = delays.Length > 0;
        int retryCount = hasDelays ?  delays.Length : DEFAULT_RETRY_COUNT;
            
        for (int i = 0; i <= retryCount; i++)
        {
            try
            {
                return await func().ConfigureAwait(false);
            } 
            catch (Exception e) when (i == retryCount)
            {
                throw new RetryException((uint)retryCount, e);
            }
            catch (Exception e) when (exceptionPredicate(e))
            {
                if (hasDelays)
                {
                    await Task.Delay(delays[i]).ConfigureAwait(false);
                }
            }
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by
    /// the <paramref name="exceptionPredicate"/>.
    /// <param name="func">The factory for the task to be retried.</param>
    /// <param name="exceptionPredicate">The predicate indicating which exception to retry on.</param>
    /// <param name="delayFactory">The factory for returning delay period between retries.</param>
    /// <param name="cToken">The cancellation token for canceling the retries.</param>
    /// </summary>
    [DebuggerStepThrough]
    public static async Task<TResult> On<TResult>(
        Func<Task<TResult>> func,
        Func<Exception, bool> exceptionPredicate,
        Func<Exception, uint, TimeSpan> delayFactory,
        CancellationToken cToken)
    {
        uint failureCount = 0;
        while (true)
        {
            try
            {
                failureCount++;
                return await func().ConfigureAwait(false);
            } catch (Exception e) when (exceptionPredicate(e))
            {
                try
                {
                    await Task.Delay(delayFactory(e, failureCount), cToken).ConfigureAwait(false);
                } catch (TaskCanceledException)
                {
                    throw new RetryException(failureCount - 1, e);
                }
            }
        }
    }
}