namespace Easy.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An abstraction for easily starting timers.
    /// </summary>
    public static class EasyTimer
    {
        /// <summary>
        /// Starts a timer which will execute the given <paramref name="work"/> after
        /// the given <paramref name="interval"/> until canceled by the given <paramref name="cToken"/>.
        /// </summary>
        /// <returns>The timer task.</returns>
        public static async Task Start(Action work, TimeSpan interval, CancellationToken cToken)
        {
            while (!cToken.IsCancellationRequested)
            {
                await WaitFor(interval, cToken).ConfigureAwait(false);
                if (!cToken.IsCancellationRequested) { work(); }
            }
        }

        /// <summary>
        /// Starts a timer which will execute the given <paramref name="work"/> after
        /// the given <paramref name="interval"/> until canceled by the given <paramref name="cToken"/>.
        /// </summary>
        /// <returns>The timer task.</returns>
        public static async Task Start(Func<Task> work, TimeSpan interval, CancellationToken cToken)
        {
            while (!cToken.IsCancellationRequested)
            {
                await WaitFor(interval, cToken).ConfigureAwait(false);
                if (!cToken.IsCancellationRequested) { await work(); }
            }
        }

        private static async Task WaitFor(TimeSpan delay, CancellationToken cToken)
        {
            try
            {
                await Task.Delay(delay, cToken).ConfigureAwait(false);
            } catch { /* ignored */ }
        }
    }
}