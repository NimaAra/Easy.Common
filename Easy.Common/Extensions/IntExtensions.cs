namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Extension methods for <see cref="long"/>
    /// </summary>
    public static class Int64Extensions
    {
        /// <summary>
        /// Converts milliseconds since Epoch to <see cref="DateTime"/>
        /// </summary>
        /// <param name="epochMilliseconds">Milliseconds since Epoch as <see cref="long"/></param>
        /// <returns><see cref="DateTime"/></returns>
        [DebuggerStepThrough]
        public static DateTime FromEpochMilliseconds(this long epochMilliseconds)
        {
            return DateTimeExtensions.Epoch.AddMilliseconds(epochMilliseconds);
        }

        [DebuggerStepThrough]
        public static IEnumerable<long> Times(this long times)
        {
            for (long i = 1; i <= times; ++i) { yield return i; }
        }

        [DebuggerStepThrough]
        public static void Times(this long times, Action<long> actionFn)
        {
            for (long index = 1; index <= times; ++index) { actionFn(index); }
        }

        [DebuggerStepThrough]
        public static void Times(this long times, Action actionFn)
        {
            for (long index = 1; index <= times; ++index) { actionFn(); }
        }

        [DebuggerStepThrough]
        public static IList<T> Times<T>(this long times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (long index = 1; index <= times; ++index) { list.Add(actionFn()); }
            return list;
        }

        [DebuggerStepThrough]
        public static IList<T> Times<T>(this long times, Func<long, T> actionFn)
        {
            var list = new List<T>();
            for (long index = 1; index <= times; ++index) { list.Add(actionFn(index)); }
            return list;
        }

        [DebuggerStepThrough]
        public static TimeSpan Ticks(this long number)
        {
            return TimeSpan.FromTicks(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this long number)
        {
            return TimeSpan.FromMilliseconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Seconds(this long number)
        {
            return TimeSpan.FromSeconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Minutes(this long number)
        {
            return TimeSpan.FromMinutes(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Hours(this long number)
        {
            return TimeSpan.FromHours(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Days(this long number)
        {
            return TimeSpan.FromDays(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Weeks(this long number)
        {
            return TimeSpan.FromDays(number * 7);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="int"/>
    /// </summary>
    public static class Int32Extensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<int> Times(this int times)
        {
            for (var i = 1; i <= times; ++i) { yield return i; }
        }

        [DebuggerStepThrough]
        public static void Times(this int times, Action<int> actionFn)
        {
            for (var index = 1; index <= times; ++index) { actionFn(index); }
        }

        [DebuggerStepThrough]
        public static void Times(this int times, Action actionFn)
        {
            for (var index = 1; index <= times; ++index) { actionFn(); }
        }

        [DebuggerStepThrough]
        public static IList<T> Times<T>(this int times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (var index = 1; index <= times; ++index) { list.Add(actionFn()); }

            return list;
        }

        [DebuggerStepThrough]
        public static IList<T> Times<T>(this int times, Func<int, T> actionFn)
        {
            var list = new List<T>();
            for (var index = 1; index <= times; ++index) { list.Add(actionFn(index)); }

            return list;
        }

        [DebuggerStepThrough]
        public static TimeSpan Ticks(this int number)
        {
            return TimeSpan.FromTicks(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this int number)
        {
            return TimeSpan.FromMilliseconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Seconds(this int number)
        {
            return TimeSpan.FromSeconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Minutes(this int number)
        {
            return TimeSpan.FromMinutes(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Hours(this int number)
        {
            return TimeSpan.FromHours(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Days(this int number)
        {
            return TimeSpan.FromDays(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Weeks(this int number)
        {
            return TimeSpan.FromDays(number * 7);
        }
    }

    /// <summary>
    /// Extension methods for <see cref="short"/>
    /// </summary>
    public static class Int16Extensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<short> Times(this short times)
        {
            for (short i = 1; i <= times; ++i) { yield return i; }
        }

        [DebuggerStepThrough]
        public static void Times(this short times, Action<short> actionFn)
        {
            for (short index = 1; index <= times; ++index) { actionFn(index); }
        }

        [DebuggerStepThrough]
        public static void Times(this short times, Action actionFn)
        {
            for (short index = 1; index <= times; ++index) { actionFn(); }
        }

        [DebuggerStepThrough]
        public static IList<T> Times<T>(this short times, Func<T> actionFn)
        {
            var list = new List<T>();
            for (short index = 1; index <= times; ++index) { list.Add(actionFn()); }
            return list;
        }

        [DebuggerStepThrough]
        public static IList<T> Times<T>(this short times, Func<short, T> actionFn)
        {
            var list = new List<T>();
            for (short index = 1; index <= times; ++index) { list.Add(actionFn(index)); }
            return list;
        }

        [DebuggerStepThrough]
        public static TimeSpan Ticks(this short number)
        {
            return TimeSpan.FromTicks(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Milliseconds(this short number)
        {
            return TimeSpan.FromMilliseconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Seconds(this short number)
        {
            return TimeSpan.FromSeconds(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Minutes(this short number)
        {
            return TimeSpan.FromMinutes(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Hours(this short number)
        {
            return TimeSpan.FromHours(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Days(this short number)
        {
            return TimeSpan.FromDays(number);
        }

        [DebuggerStepThrough]
        public static TimeSpan Weeks(this short number)
        {
            return TimeSpan.FromDays(number * 7);
        }
    }
}