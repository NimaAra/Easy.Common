// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A helper class which provides retry logic for actions and delegates.
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Retries the given <paramref name="action"/> in case of an exception of 
        /// type <typeparamref name="TEx"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="action"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task On<TEx>(Action action, params TimeSpan[] delays) 
            where TEx : Exception => OnImpl(action, delays, typeof(TEx));

        /// <summary>
        /// Retries the given <paramref name="action"/> in case of any of the given exceptions specified by
        /// <typeparamref name="TEx1"/> and <typeparamref name="TEx2"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="action"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task OnAny<TEx1, TEx2>(Action action, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception => OnImpl(action, delays, typeof(TEx1), typeof(TEx2));

        /// <summary>
        /// Retries the given <paramref name="action"/> in case of any of the given exceptions specified by
        /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/> and <typeparamref name="TEx3"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="action"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task OnAny<TEx1, TEx2, TEx3>(Action action, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception => 
            OnImpl(action, delays, typeof(TEx1), typeof(TEx2), typeof(TEx3));

        /// <summary>
        /// Retries the given <paramref name="action"/> in case of any of the given exceptions specified by
        /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/> and <typeparamref name="TEx4"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="action"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task OnAny<TEx1, TEx2, TEx3, TEx4>(Action action, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception =>
            OnImpl(action, delays, typeof(TEx1), typeof(TEx2), typeof(TEx3), typeof(TEx4));

        /// <summary>
        /// Retries the given <paramref name="action"/> in case of any of the given exceptions specified by
        /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/> and <typeparamref name="TEx5"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="action"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task OnAny<TEx1, TEx2, TEx3, TEx4, TEx5>(Action action, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception where TEx5 : Exception =>
            OnImpl(action, delays, typeof(TEx1), typeof(TEx2), typeof(TEx3), typeof(TEx4), typeof(TEx5));

        /// <summary>
        /// Retries the given <paramref name="func"/> in case of an exception of 
        /// type <typeparamref name="TEx"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="func"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task<TResult> On<TEx, TResult>(Func<TResult> func, params TimeSpan[] delays) 
            where TEx : Exception => OnImpl(func, delays, typeof(TEx));

        /// <summary>
        /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
        /// <typeparamref name="TEx1"/> and <typeparamref name="TEx2"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="func"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task<TResult> OnAny<TEx1, TEx2, TResult>(Func<TResult> func, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception => OnImpl(func, delays, typeof(TEx1), typeof(TEx2));

        /// <summary>
        /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
        /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/> and <typeparamref name="TEx3"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="func"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TResult>(Func<TResult> func, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception =>
            OnImpl(func, delays, typeof(TEx1), typeof(TEx2), typeof(TEx3));

        /// <summary>
        /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
        /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/> and <typeparamref name="TEx4"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="func"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TEx4, TResult>(Func<TResult> func, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception =>
            OnImpl(func, delays, typeof(TEx1), typeof(TEx2), typeof(TEx3), typeof(TEx4));

        /// <summary>
        /// Retries the given <paramref name="func"/> in case of any of the given exceptions specified by 
        /// <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/> and <typeparamref name="TEx5"/>.
        /// <remarks>
        /// If the given <paramref name="delays"/> is not supplied then the given 
        /// <paramref name="func"/> will be retried once.
        /// </remarks>
        /// </summary>
        public static Task<TResult> OnAny<TEx1, TEx2, TEx3, TEx4, TEx5, TResult>(Func<TResult> func, params TimeSpan[] delays)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception where TEx5 : Exception =>
            OnImpl(func, delays, typeof(TEx1), typeof(TEx2), typeof(TEx3), typeof(TEx4), typeof(TEx5));

        private static async Task OnImpl(Action action, IReadOnlyList<TimeSpan> delays, params Type[] exceptionTypes)
        {
            if (!delays.Any())
            {
                var count = 2;
                while (count-- > 0)
                {
                    try
                    {
                        action();
                        return;
                    } catch (Exception e) when (exceptionTypes.Contains(e.GetType()))
                    {
                        if (count == 0) { throw new RetryException(1, e); }
                    }
                }
            }

            for (var i = 0; i <= delays.Count; i++)
            {
                try
                {
                    action();
                    return;
                } catch (Exception e) when (exceptionTypes.Contains(e.GetType()))
                {
                    if (i == delays.Count) { throw new RetryException((uint)i, e); }
                    await Task.Delay(delays[i]).ConfigureAwait(false);
                }
            }
        }

        private static async Task<TResult> OnImpl<TResult>(Func<TResult> func, IReadOnlyList<TimeSpan> delays, params Type[] exceptionTypes)
        {
            if (!delays.Any())
            {
                uint count = 2;
                while (count-- > 0)
                {
                    try
                    {
                        return func();
                    } catch (Exception e) when (exceptionTypes.Contains(e.GetType()))
                    {
                        if (count == 0) { throw new RetryException(1, e); }
                    }
                }
            }

            for (var i = 0; i <= delays.Count; i++)
            {
                try
                {
                    return func();
                } catch (Exception e) when (exceptionTypes.Contains(e.GetType()))
                {
                    if (i == delays.Count) { throw new RetryException((uint)i, e); }
                    await Task.Delay(delays[i]).ConfigureAwait(false);
                }
            }
            throw new InvalidOperationException();
        }
    }
}