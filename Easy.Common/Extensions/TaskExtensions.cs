// ReSharper disable PossibleMultipleEnumeration
namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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
        public static async Task<T> TimeoutAfter<T>(
            this Task<T> task, TimeSpan timeout, CancellationToken cToken = default(CancellationToken))
        {
            await TimeoutAfterImpl(task, timeout, cToken).ConfigureAwait(false);
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
            this IEnumerable<Task<T>> tasks, TimeSpan timeout, CancellationToken cToken = default(CancellationToken))
        {
            await TimeoutAfterImpl(tasks, timeout, cToken).ConfigureAwait(false);
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
        public static async Task TimeoutAfter(
            this Task task, TimeSpan timeout, CancellationToken cToken = default(CancellationToken))
                => await TimeoutAfterImpl(task, timeout, cToken).ConfigureAwait(false);

        /// <summary>
        /// Ensures that every task in the given <paramref name="tasks"/> finishes before a timeout 
        /// <see cref="Task"/> with the given <paramref name="timeout"/>.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// Thrown when any of the <paramref name="tasks"/> times out.
        /// </exception>
        [DebuggerStepThrough]
        public static async Task TimeoutAfter(
            this IEnumerable<Task> tasks, TimeSpan timeout, CancellationToken cToken = default(CancellationToken)) 
                => await TimeoutAfterImpl(tasks, timeout, cToken).ConfigureAwait(false);

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
        /// Suppresses default exception handling of a Task that would otherwise re-raise 
        /// the exception on the finalizer thread.
        /// </summary>
        /// <param name="task">The Task to be monitored</param>
        /// <returns>The original Task</returns>
        [DebuggerStepThrough]
        public static Task IgnoreExceptions(this Task task)
        {
            Ensure.NotNull(task, nameof(task));

            return task.ContinueWith(t =>
            {
                // ReSharper disable once UnusedVariable
                var ignored = t.Exception;
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        }

        /// <summary>
        /// Suppresses default exception handling of a Task that would otherwise re-raise 
        /// the exception on the finalizer thread.
        /// </summary>
        /// <param name="task">The Task to be monitored</param>
        /// <returns>The original Task</returns>
        [DebuggerStepThrough]
        public static Task<T> IgnoreExceptions<T>(this Task<T> task)
        {
            Ensure.NotNull(task, nameof(task));
            return (Task<T>)((Task)task).IgnoreExceptions();
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
                var e = t.Exception;

                if (e == null)
                { return; }

                e.Flatten().Handle(ie =>
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
        /// Handles expected exception(s) thrown by the <paramref name="task"/> which are specified by <paramref name="exceptionPredicate"/>.
        /// </summary>
        /// <param name="task">The task which might throw exceptions.</param>
        /// <param name="exceptionPredicate">The predicate specifying which exception(s) to handle</param>
        /// <param name="exceptionHandler">The handler to which every exception is passed</param>
        /// <returns>The continuation task added to the <paramref name="task"/></returns>
        [DebuggerStepThrough]
        public static Task HandleExceptions(this Task task, Func<Exception, bool> exceptionPredicate, Action<Exception> exceptionHandler)
        {
            Ensure.NotNull(task, nameof(task));
            Ensure.NotNull(exceptionPredicate, nameof(exceptionPredicate));
            Ensure.NotNull(exceptionHandler, nameof(exceptionHandler));

            return task.ContinueWith(t =>
            {
                var e = t.Exception;

                if (e == null)
                { return; }

                e.Flatten().Handle(ie =>
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
        [DebuggerStepThrough]
        public static Task HandleException<T>(this Task task, Action<T> exceptionHandler) where T : Exception
        {
            Ensure.NotNull(task, nameof(task));
            Ensure.NotNull(exceptionHandler, nameof(exceptionHandler));

            return task.ContinueWith(t =>
            {
                var e = t.Exception;

                if (e == null)
                { return; }

                e.Flatten().Handle(ie =>
                {
                    if (ie.GetType() == typeof(T))
                    {
                        exceptionHandler((T)ie);
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

        private static async Task TimeoutAfterImpl(
            this Task task, TimeSpan timeoutPeriod, CancellationToken cToken)
        {
            if (task == null) { throw new ArgumentNullException(nameof(task)); }

            var timeoutTask = Task.Delay(timeoutPeriod, cToken);
            var finishedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            cToken.ThrowIfCancellationRequested();
            
            if (finishedTask == timeoutTask)
            {
                throw new TimeoutException("Task timed out after: " + timeoutPeriod);
            }
        }

        private static async Task<IEnumerable<Task>> TimeoutAfterImpl(
            this IEnumerable<Task> tasks, TimeSpan timeoutPeriod, CancellationToken cToken)
        {
            if (tasks == null) { throw new ArgumentNullException(nameof(tasks)); }

            var timeoutTask = Task.Delay(timeoutPeriod, cToken);
            var tasksList = new List<Task>(tasks) { timeoutTask };
            while (tasksList.Any())
            {
                cToken.ThrowIfCancellationRequested();
                var finishedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);
                cToken.ThrowIfCancellationRequested();
                
                if (finishedTask == timeoutTask)
                {
                    throw new TimeoutException("At least one of the tasks timed out after: " + timeoutPeriod);
                }

                tasksList.Remove(finishedTask);

                if (tasksList.Count == 1 && tasksList[0] == timeoutTask) { break; }
            }
            return tasks;
        }
    }
}