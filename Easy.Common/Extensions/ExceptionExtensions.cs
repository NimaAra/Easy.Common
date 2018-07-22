namespace Easy.Common.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Contains a set of helper methods for working with <see cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is of type <typeparamref name="TEx"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx>(this Exception ex) where TEx : Exception
            => ex.IsExpectedException(e => e is TEx);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the types <typeparamref name="TEx1"/>
        /// or <typeparamref name="TEx2"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx1, TEx2>(this Exception ex) 
            where TEx1 : Exception
            where TEx2 : Exception
                => ex.IsExpectedException(e => e is TEx1 || e is TEx2);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the types <typeparamref name="TEx1"/>,
        /// <typeparamref name="TEx2"/> or <typeparamref name="TEx3"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx1, TEx2, TEx3>(this Exception ex) 
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
                => ex.IsExpectedException(e => e is TEx1 || e is TEx2 || e is TEx3);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the types <typeparamref name="TEx1"/>,
        /// <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/> or <typeparamref name="TEx4"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4>(this Exception ex) 
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
                => ex.IsExpectedException(e => e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the types <typeparamref name="TEx1"/>,
        /// <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/>
        /// or <typeparamref name="TEx5"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5>(this Exception ex) 
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
            where TEx5 : Exception
                => ex.IsExpectedException(e => e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4 || e is TEx5);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the types <typeparamref name="TEx1"/>,
        /// <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/>,
        /// <typeparamref name="TEx5"/> or <typeparamref name="TEx6"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6>(this Exception ex) 
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
            where TEx5 : Exception
            where TEx6 : Exception
                => ex.IsExpectedException(e => e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4 || e is TEx5 || e is TEx6);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the types <typeparamref name="TEx1"/>,
        /// <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/>,
        /// <typeparamref name="TEx5"/>, <typeparamref name="TEx6"/> or <typeparamref name="TEx7"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException<TEx1, TEx2, TEx3, TEx4, TEx5, TEx6, TEx7>(this Exception ex) 
            where TEx1 : Exception
            where TEx2 : Exception
            where TEx3 : Exception
            where TEx4 : Exception
            where TEx5 : Exception
            where TEx6 : Exception
            where TEx7 : Exception
                => ex.IsExpectedException(e => e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4 || e is TEx5 || e is TEx6 || e is TEx7);

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is any of the <paramref name="exceptionTypes"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException(this Exception ex, params Type[] exceptionTypes)
            => ex.IsExpectedException(e => exceptionTypes.Contains(e.GetType()));

        /// <summary>
        /// Determines whether the given <paramref name="ex"/> is the type matched by <paramref name="predicate"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsExpectedException(this Exception ex, Func<Exception, bool> predicate)
        {
            if (predicate(ex)) { return true; }

            if (ex is AggregateException aggEx)
            {
                var found = false;
                aggEx.Flatten().Handle(x =>
                {
                    if (predicate(x)) { found = true; }

                    return true;
                });

                return found;
            }

            return false;
        }
    }
}