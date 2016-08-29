namespace Easy.Common
{
    using System;

    /// <summary>
    /// A helper class which provides JavaScript style timers, setInterval and SetTimeout
    /// </summary>
    public static class EasyTimer
    {
        /// <summary>
        /// Executes a timer which invokes <paramref name="action"/> after the specified <paramref name="interval"/> 
        /// until the timer has been disposed.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="interval">The interval at which <paramref name="action"/> will be executed</param>
        public static IDisposable SetInterval(Action action, TimeSpan interval)
        {
            Ensure.NotNull(action, nameof(action));
            var clock = new TimerClock(interval);
            clock.Tick += (sender, args) => action();
            clock.Enabled = true;
            return clock;
        }

        /// <summary>
        /// Executes a timer which invokes <paramref name="action"/> once after the specified <paramref name="timeout"/> 
        /// until the timer has been disposed.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <param name="timeout">The timeout after which <paramref name="action"/> will be executed</param>
        public static IDisposable SetTimeout(Action action, TimeSpan timeout)
        {
            Ensure.NotNull(action, nameof(action));

            var clock = new TimerClock(timeout);
            clock.Tick += (sender, args) =>
            {
                clock.Enabled = false;
                action();
            };
            clock.Enabled = true;
            return clock;
        }
    }
}