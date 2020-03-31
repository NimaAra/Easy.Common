namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Extension methods for <see cref="long"/>.
    /// </summary>
    public static class Int64Extensions
    {
        /// <summary>
        /// Converts milliseconds since Epoch to <see cref="DateTime"/>
        /// </summary>
        /// <param name="epochMilliseconds">Milliseconds since Epoch as <see cref="long"/></param>
        /// <returns><see cref="DateTime"/></returns>
        [DebuggerStepThrough]
        public static DateTime FromEpochMilliseconds(this long epochMilliseconds) => DateTimeExtensions.Epoch.AddMilliseconds(epochMilliseconds);

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing <paramref name="times"/> item.
        /// </summary>
        /// <param name="times">The number of items to include in the result</param>
        [DebuggerStepThrough]
        public static IEnumerable<long> Times(this long times)
        {
            for (long i = 1; i <= times; ++i) { yield return i; }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static void Times(this long times, Action<long> actionFn)
        {
            for (long index = 1; index <= times; ++index) { actionFn(index); }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static void Times(this long times, Action actionFn)
        {
            for (long index = 1; index <= times; ++index) { actionFn(); }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times and returns the result of each execution.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> Times<T>(this long times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (long index = 1; index <= times; ++index) { list.Add(actionFn()); }
            return list;
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times and returns the result of each execution.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> Times<T>(this long times, Func<long, T> actionFn)
        {
            var list = new List<T>();
            for (long index = 1; index <= times; ++index) { list.Add(actionFn(index)); }
            return list;
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Ticks</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Ticks(this long number) => TimeSpan.FromTicks(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Milliseconds</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this long number) => TimeSpan.FromMilliseconds(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Seconds</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Seconds(this long number) => TimeSpan.FromSeconds(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Minutes</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Minutes(this long number) => TimeSpan.FromMinutes(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Hours</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Hours(this long number) => TimeSpan.FromHours(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Days</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Days(this long number) => TimeSpan.FromDays(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Weeks</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Weeks(this long number) => TimeSpan.FromDays(number * 7);
    }

    /// <summary>
    /// Extension methods for <see cref="int"/>
    /// </summary>
    public static class Int32Extensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing <paramref name="times"/> item.
        /// </summary>
        /// <param name="times">The number of items to include in the result</param>
        [DebuggerStepThrough]
        public static IEnumerable<int> Times(this int times)
        {
            for (var i = 1; i <= times; ++i) { yield return i; }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static void Times(this int times, Action<int> actionFn)
        {
            for (var index = 1; index <= times; ++index) { actionFn(index); }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static void Times(this int times, Action actionFn)
        {
            for (var index = 1; index <= times; ++index) { actionFn(); }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times and returns the result of each execution.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> Times<T>(this int times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (var index = 1; index <= times; ++index) { list.Add(actionFn()); }
            return list;
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times and returns the result of each execution.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> Times<T>(this int times, Func<int, T> actionFn)
        {
            var list = new List<T>();
            for (var index = 1; index <= times; ++index) { list.Add(actionFn(index)); }
            return list;
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Ticks</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Ticks(this int number) => TimeSpan.FromTicks(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Milliseconds</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this int number) => TimeSpan.FromMilliseconds(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Seconds</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Seconds(this int number) => TimeSpan.FromSeconds(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Minutes</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Minutes(this int number) => TimeSpan.FromMinutes(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Hours</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Hours(this int number) => TimeSpan.FromHours(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Days</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Days(this int number) => TimeSpan.FromDays(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Weeks</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Weeks(this int number) => TimeSpan.FromDays(number * 7);
    }

    /// <summary>
    /// Extension methods for <see cref="short"/>
    /// </summary>
    public static class Int16Extensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing <paramref name="times"/> item.
        /// </summary>
        /// <param name="times">The number of items to include in the result</param>
        [DebuggerStepThrough]
        public static IEnumerable<short> Times(this short times)
        {
            for (short i = 1; i <= times; ++i) { yield return i; }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static void Times(this short times, Action<short> actionFn)
        {
            for (short index = 1; index <= times; ++index) { actionFn(index); }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static void Times(this short times, Action actionFn)
        {
            for (short index = 1; index <= times; ++index) { actionFn(); }
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times and returns the result of each execution.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> Times<T>(this short times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (short index = 1; index <= times; ++index) { list.Add(actionFn()); }
            return list;
        }

        /// <summary>
        /// Executes the given <paramref name="actionFn"/> <paramref name="times"/> times and returns the result of each execution.
        /// </summary>
        /// <param name="times">The number of times the <paramref name="actionFn"/> should be executed</param>
        /// <param name="actionFn">The action to execute</param>
        [DebuggerStepThrough]
        public static IReadOnlyList<T> Times<T>(this short times, Func<short, T> actionFn)
        {
            var list = new List<T>();
            for (short index = 1; index <= times; ++index) { list.Add(actionFn(index)); }
            return list;
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Ticks</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Ticks(this short number) => TimeSpan.FromTicks(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Milliseconds</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this short number) => TimeSpan.FromMilliseconds(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Seconds</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Seconds(this short number) => TimeSpan.FromSeconds(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Minutes</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Minutes(this short number) => TimeSpan.FromMinutes(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Hours</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Hours(this short number) => TimeSpan.FromHours(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Days</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Days(this short number) => TimeSpan.FromDays(number);

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> represented by <paramref name="number"/> as <c>Weeks</c>.
        /// </summary>
        [DebuggerStepThrough]
        public static TimeSpan Weeks(this short number) => TimeSpan.FromDays(number * 7);
    }
}