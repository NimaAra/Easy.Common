namespace Easy.Common
{
    using System;

    /// <summary>
    /// A helper class which provides try logic for actions and delegates.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Attempts to execute the given <paramref name="action"/>. If <paramref name="action"/> executes
        /// successfully then <c>True</c> is returned otherwise if the given <typeparamref name="TEx"/>
        /// is thrown then <c>False</c> is returned.
        /// </summary>
        public static bool Handle<TEx>(Action action) where TEx : Exception
        {
            try
            {
                action();
                return true;
            } catch (Exception e) when(e is TEx) { return false; }
        }

        /// <summary>
        /// Attempts to execute the given <paramref name="action"/>. If <paramref name="action"/> executes
        /// successfully then <c>True</c> is returned otherwise if any of the given <typeparamref name="TEx1"/>
        /// or <typeparamref name="TEx2"/> is thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2>(Action action, Action<Exception> onException)
            where TEx1 : Exception where TEx2 : Exception
        {
            try
            {
                action();
            } catch (Exception e) when (e is TEx1 || e is TEx2)
            {
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute the given <paramref name="action"/>. If <paramref name="action"/> executes
        /// successfully then <c>True</c> is returned otherwise if any of the given <typeparamref name="TEx1"/>
        /// <typeparamref name="TEx2"/> or <typeparamref name="TEx3"/> is thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TEx3>(Action action, Action<Exception> onException)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception
        {
            try
            {
                action();
            } catch (Exception e) when (e is TEx1 || e is TEx2 || e is TEx3)
            {
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute the given <paramref name="action"/>. If <paramref name="action"/> executes
        /// successfully then <c>True</c> is returned otherwise if any of the given <typeparamref name="TEx1"/>
        /// <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/> or <typeparamref name="TEx4"/> is 
        /// thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TEx3, TEx4>(Action action, Action<Exception> onException)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception
        {
            try
            {
                action();
            } catch (Exception e) when (e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4)
            {
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute the given <paramref name="action"/>. If <paramref name="action"/> executes
        /// successfully then <c>True</c> is returned otherwise if any of the given <typeparamref name="TEx1"/>
        /// <typeparamref name="TEx2"/>, <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/> or
        /// <typeparamref name="TEx5"/> is thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TEx3, TEx4, TEx5>(Action action, Action<Exception> onException)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception where TEx5 : Exception
        {
            try
            {
                action();
            } catch (Exception e) when (e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4 || e is TEx5)
            {
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute and return the result of the given <paramref name="func"/>. 
        /// If <paramref name="func"/> executes successfully then <c>True</c> is returned otherwise 
        /// if the given <typeparamref name="TEx"/> is thrown then <c>False</c> is returned.
        /// </summary>
        public static bool Handle<TEx, TResult>(Func<TResult> func, out TResult result) 
            where TEx : Exception
        {
            try
            {
                result = func();
                return true;
            } catch (Exception e) when (e is TEx)
            {
                result = default(TResult);
                return false;
            }
        }

        /// <summary>
        /// Attempts to execute and return the result of the given <paramref name="func"/>.
        /// If <paramref name="func"/> executes successfully then <c>True</c> is returned otherwise 
        /// if any of the given <typeparamref name="TEx1"/> or <typeparamref name="TEx2"/> is thrown 
        /// then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TResult>(
            Func<TResult> func, Action<Exception> onException, out TResult result)
            where TEx1 : Exception where TEx2 : Exception
        {
            try
            {
                result = func();
            } catch (Exception e) when (e is TEx1 || e is TEx2)
            {
                result = default(TResult);
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute and return the result of the given <paramref name="func"/>.
        /// If <paramref name="func"/> executes successfully then <c>True</c> is returned otherwise 
        /// if any of the given <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/> or 
        /// <typeparamref name="TEx3"/> is thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TEx3, TResult>(
            Func<TResult> func, Action<Exception> onException, out TResult result)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception
        {
            try
            {
                result = func();
            } catch (Exception e) when (e is TEx1 || e is TEx2 || e is TEx3)
            {
                result = default(TResult);
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute and return the result of the given <paramref name="func"/>.
        /// If <paramref name="func"/> executes successfully then <c>True</c> is returned otherwise 
        /// if any of the given <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, 
        /// <typeparamref name="TEx3"/> or <typeparamref name="TEx4"/> is thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TEx3, TEx4, TResult>(
            Func<TResult> func, Action<Exception> onException, out TResult result)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception
        {
            try
            {
                result = func();
            } catch (Exception e) when (e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4)
            {
                result = default(TResult);
                onException(e);
            }
        }

        /// <summary>
        /// Attempts to execute and return the result of the given <paramref name="func"/>.
        /// If <paramref name="func"/> executes successfully then <c>True</c> is returned otherwise 
        /// if any of the given <typeparamref name="TEx1"/>, <typeparamref name="TEx2"/>, 
        /// <typeparamref name="TEx3"/>, <typeparamref name="TEx4"/> or <typeparamref name="TEx4"/> 
        /// is thrown then <c>False</c> is returned.
        /// </summary>
        public static void HandleAny<TEx1, TEx2, TEx3, TEx4, TEx5, TResult>(
            Func<TResult> func, Action<Exception> onException, out TResult result)
            where TEx1 : Exception where TEx2 : Exception where TEx3 : Exception where TEx4 : Exception where TEx5 : Exception
        {
            try
            {
                result = func();
            } catch (Exception e) when (e is TEx1 || e is TEx2 || e is TEx3 || e is TEx4 || e is TEx5)
            {
                result = default(TResult);
                onException(e);
            }
        }
    }
}