// ReSharper disable PossibleMultipleEnumeration
namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A set of helper methods as extensions on <see cref="System.Threading.Tasks"/>
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Ensures that the given <paramref name="task"/> finishes before a timeout <see cref="Task"/> 
        /// with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// Thrown when the <paramref name="task"/> times out.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            await TimeoutAfterImpl(task, timeout).ConfigureAwait(false);
            return task.Result;
        }

        /// <summary>
        /// Ensures that every task in the given <paramref name="tasks"/> finishes before a timeout 
        /// <see cref="Task"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// Thrown when any of the <paramref name="tasks"/> times out.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task<IEnumerable<Task<T>>> TimeoutAfter<T>(
            this IEnumerable<Task<T>> tasks, TimeSpan timeout)
        {
            await TimeoutAfterImpl(tasks, timeout).ConfigureAwait(false);
            return tasks;
        }

        /// <summary>
        /// Ensures that the given <paramref name="task"/> finishes before a timeout <see cref="Task"/> 
        /// with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// Thrown the <paramref name="task"/> times out.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
                => await TimeoutAfterImpl(task, timeout).ConfigureAwait(false);

        /// <summary>
        /// Ensures that every task in the given <paramref name="tasks"/> finishes before a timeout 
        /// <see cref="Task"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// Thrown when any of the <paramref name="tasks"/> times out.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task TimeoutAfter(this IEnumerable<Task> tasks, TimeSpan timeout) 
                => await TimeoutAfterImpl(tasks, timeout).ConfigureAwait(false);

        /// <summary>
        /// Executes the given action on each of the tasks in turn, in the order of
        /// the sequence. The action is passed the result of each task.
        /// </summary>
        [DebuggerStepThrough]
        public static async Task ForEachInOrder<T>(this IEnumerable<Task<T>> tasks, Action<T> action)
        {
            Ensure.NotNull(tasks, nameof(tasks));
            Ensure.NotNull(action, nameof(action));

            foreach (var task in tasks)
            {
                var value = await task.ConfigureAwait(false);
                action(value);
            }
        }

        /// <summary>
        /// Executes the given action on each of the tasks in turn, in the order of
        /// the sequence. The action is passed the result of each task.
        /// </summary>
        [DebuggerStepThrough]
        public static async Task ForEachInOrder(this IEnumerable<Task> tasks, Action<Task> action)
        {
            Ensure.NotNull(tasks, nameof(tasks));
            Ensure.NotNull(action, nameof(action));

            foreach (var task in tasks)
            {
                await task.ConfigureAwait(false);
                action(task);
            }
        }

    #region Exception Handling

        /// <summary>
        /// Suppresses default exception handling of the given <paramref name="task"/>
        /// that would otherwise re-raise the exception on the finalizer thread.
        /// </summary>
        /// <param name="task">The Task to be monitored</param>
        /// <returns>The original task</returns>
        [DebuggerStepThrough]
        public static Task<Task<T>> IgnoreExceptions<T>(this Task<Task<T>> task)
        {
            Ensure.NotNull(task, nameof(task));

            task.Unwrap().IgnoreExceptions();
            
            return task;
        }
        
        /// <summary>
        /// Suppresses default exception handling of the given <paramref name="task"/>
        /// that would otherwise re-raise the exception on the finalizer thread.
        /// </summary>
        /// <param name="task">The Task to be monitored</param>
        /// <returns>The original task</returns>
        [DebuggerStepThrough]
        public static Task<Task> IgnoreExceptions(this Task<Task> task)
        {
            Ensure.NotNull(task, nameof(task));

            task.Unwrap().IgnoreExceptions();

            return task;
        }

        /// <summary>
        /// Suppresses default exception handling of the given <paramref name="task"/>
        /// that would otherwise re-raise the exception on the finalizer thread.
        /// </summary>
        /// <param name="task">The Task to be monitored</param>
        /// <returns>The original task</returns>
        [DebuggerStepThrough]
        public static Task IgnoreExceptions(this Task task)
        {
            Ensure.NotNull(task, nameof(task));

            task.ContinueWith(
                t => { var _ = t.Exception; },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return task;
        }

        /// <summary>
        /// Handles all the exceptions thrown by the <paramref name="task"/>.
        /// </summary>
        /// <param name="task">The task which might throw exceptions</param>
        /// <param name="exceptionsHandler">The handler to which every exception is passed</param>
        /// <returns>The continuation task added to the <paramref name="task"/></returns>
        [DebuggerStepThrough]
        public static Task HandleExceptions(this Task task, Action<Exception> exceptionsHandler)
        {
            Ensure.NotNull(task, nameof(task));
            Ensure.NotNull(exceptionsHandler, nameof(exceptionsHandler));

            return task.ContinueWith(t =>
            {
                var aggEx = t.Exception;

                if (aggEx == null) { return; }

                aggEx.Flatten().Handle(ie =>
                {
                    exceptionsHandler(ie);
                    return true;
                });
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }

        /// <summary>
        /// Handles expected exception(s) thrown by the <paramref name="task"/>
        /// which are specified by <paramref name="exceptionPredicate"/>.
        /// </summary>
        /// <param name="task">The task which might throw exceptions.</param>
        /// <param name="exceptionPredicate">The predicate specifying which exception(s) to handle</param>
        /// <param name="exceptionHandler">The handler to which every exception is passed</param>
        /// <returns>The continuation task added to the <paramref name="task"/></returns>
        [DebuggerStepThrough]
        public static Task HandleExceptions(
            this Task task, Func<Exception, bool> exceptionPredicate, Action<Exception> exceptionHandler)
        {
            Ensure.NotNull(task, nameof(task));
            Ensure.NotNull(exceptionPredicate, nameof(exceptionPredicate));
            Ensure.NotNull(exceptionHandler, nameof(exceptionHandler));

            return task.ContinueWith(t =>
            {
                var aggEx = t.Exception;

                if (aggEx == null) { return; }

                aggEx.Flatten().Handle(ie =>
                {
                    if (exceptionPredicate(ie))
                    {
                        exceptionHandler(ie);
                        return true;
                    }

                    return false;
                });
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }

        /// <summary>
        /// Handles an expected exception thrown by the <paramref name="task"/>.
        /// </summary>
        /// <typeparam name="T">Type of exception to handle</typeparam>
        /// <param name="task">The task which might throw exceptions</param>
        /// <param name="exceptionHandler">The handler to which every exception is passed</param>
        /// <returns>The continuation task added to the <paramref name="task"/></returns>
        //[DebuggerStepThrough]
        public static Task HandleException<T>(this Task task, Action<T> exceptionHandler)
            where T : Exception
        {
            Ensure.NotNull(task, nameof(task));
            Ensure.NotNull(exceptionHandler, nameof(exceptionHandler));

            return task.ContinueWith(t =>
            {
                var aggEx = t.Exception;

                if (aggEx == null) { return; }

                aggEx.Flatten().Handle(ex =>
                {
                    if (ex is T expectedException)
                    {
                        exceptionHandler(expectedException);
                        return true;
                    }

                    return false;
                });
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }

    #endregion

        private static async Task TimeoutAfterImpl(this Task task, TimeSpan timeoutPeriod)
        {
            if (task == null) { throw new ArgumentNullException(nameof(task)); }

            using (var cts = new CancellationTokenSource())
            {
                var timeoutTask = Task.Delay(timeoutPeriod, cts.Token);
                var finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
                
                if (finishedTask == timeoutTask)
                {
                    throw new TimeoutException("Task timed out after: " + timeoutPeriod.ToString());
                }

                cts.Cancel();
            }
        }

        private static async Task<IEnumerable<Task>> TimeoutAfterImpl(
            this IEnumerable<Task> tasks, TimeSpan timeoutPeriod)
        {
            if (tasks == null) { throw new ArgumentNullException(nameof(tasks)); }

            using (var cts = new CancellationTokenSource())
            {
                var cToken = cts.Token;
                var timeoutTask = Task.Delay(timeoutPeriod, cToken);
                var tasksList = new List<Task>(tasks) { timeoutTask };
                
                while (tasksList.Count > 0)
                {
                    var finishedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

                    if (finishedTask == timeoutTask)
                    {
                        throw new TimeoutException(
                            "At least one of the tasks timed out after: " + timeoutPeriod.ToString());
                    }

                    tasksList.Remove(finishedTask);

                    if (tasksList.Count == 1 && tasksList[0] == timeoutTask) { break; }
                }

                cts.Cancel();
                return tasks;
            }
        }
    }
}