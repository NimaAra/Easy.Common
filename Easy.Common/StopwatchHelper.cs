namespace Easy.Common
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides a set of methods to help work with <see cref="Stopwatch"/>.
    /// </summary>
    public static class StopwatchHelper
    {
        /// <summary>
        /// Returns the duration between now and the given <paramref name="startTime"/> in milliseconds.
        /// </summary>
        public static double GetDurationInMillisecondsSince(long startTime) 
            => GetDurationInMilliseconds(startTime, Stopwatch.GetTimestamp());

        /// <summary>
        /// Returns the duration between now and the given <paramref name="startTime"/> in seconds.
        /// </summary>
        public static double GetDurationInSecondsSince(long startTime) 
            => GetDurationInSeconds(startTime, Stopwatch.GetTimestamp());

        /// <summary>
        /// Returns the duration between now and the given <paramref name="startTime"/>.
        /// </summary>
        public static TimeSpan GetDurationSince(long startTime) 
            => GetDuration(startTime, Stopwatch.GetTimestamp());

        /// <summary>
        /// Returns the duration between <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        public static TimeSpan GetDuration(long from, long to) 
            => TimeSpan.FromMilliseconds(GetDurationInMilliseconds(from, to));

        /// <summary>
        /// Returns the duration between <paramref name="from"/> and <paramref name="to"/> in milliseconds.
        /// </summary>
        public static double GetDurationInMilliseconds(long from, long to) 
            => GetDurationInSeconds(from, to) * 1000;

        /// <summary>
        /// Returns the duration between <paramref name="from"/> and <paramref name="to"/> in seconds.
        /// </summary>
        public static double GetDurationInSeconds(long from, long to) 
            => ((double)to - from) / Stopwatch.Frequency;
    }
}