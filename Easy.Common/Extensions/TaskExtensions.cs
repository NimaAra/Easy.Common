// ReSharper disable PossibleMultipleEnumeration
namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
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
        /// with the given <paramref name="timeoutPeriod"/>.
        /// </summary>
        /// <returns>The original task.</returns>
        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeoutPeriod)
        {
            await TimeoutAfterImpl(task, timeoutPeriod);
            return await task;
        }

        /// <summary>
        /// Ensures that every task in the given <paramref name="tasks"/> finishes before a timeout 
        /// <see cref="Task"/> with the given <paramref name="timeoutPeriod"/>.
        /// </summary>
        /// <returns>The original tasks.</returns>
        public static async Task<IEnumerable<Task<T>>> TimeoutAfter<T>(this IEnumerable<Task<T>> tasks, TimeSpan timeoutPeriod)
        {
            await TimeoutAfterImpl(tasks, timeoutPeriod);
            return tasks;
        }

        /// <summary>
        /// Ensures that the given <paramref name="task"/> finishes before a timeout <see cref="Task"/> 
        /// with the given <paramref name="timeoutPeriod"/>.
        /// </summary>
        public static async Task TimeoutAfter(this Task task, TimeSpan timeoutPeriod) 
            => await TimeoutAfterImpl(task, timeoutPeriod);

        /// <summary>
        /// Ensures that every task in the given <paramref name="tasks"/> finishes before a timeout 
        /// <see cref="Task"/> with the given <paramref name="timeoutPeriod"/>.
        /// </summary>
        public static async Task TimeoutAfter(this IEnumerable<Task> tasks, TimeSpan timeoutPeriod) 
            => await TimeoutAfterImpl(tasks, timeoutPeriod);

        /// <summary>
        /// Executes the given action on each of the tasks in turn, in the order of
        /// the sequence. The action is passed the result of each task.
        /// </summary>
        public static async Task ForEach<T>(this IEnumerable<Task<T>> tasks, Action<T> action)
        {
            Ensure.NotNull(tasks, nameof(tasks));
            Ensure.NotNull(action, nameof(action));

            foreach (var task in tasks)
            {
                var value = await task;
                action(value);
            }
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that is deemed to have completed when
        /// all the <paramref name="tasks"/> have completed. Completed could mean
        /// <c>Faulted</c>, <c>Canceled</c> or <c>RanToCompletion</c>.
        /// </summary>
        /// <remarks>
        /// <c>Task.WhenAll</c> method keeps you unaware of the outcome of all the tasks 
        /// until the final one has completed. With this method you can stop waiting if 
        /// any of the supplied <paramref name="tasks"/> fails or cancels.
        /// </remarks>
        /// <typeparam name="T">Type of the result returned by the <paramref name="tasks"/></typeparam>
        /// <param name="tasks">The tasks to wait on.</param>
        /// <returns>A task returning all the results intended to be returned by <paramref name="tasks"/></returns>
        public static Task<T[]> WhenAllOrFail<T>(this IEnumerable<Task<T>> tasks)
        {
            var allTasks = Ensure.NotNull(tasks, nameof(tasks)).ToList();
            Ensure.That(allTasks.Count > 0);

            var tcs = new TaskCompletionSource<T[]>();
            var taskCompletedCount = 0;
            allTasks.ForEach(t => t.ContinueWith(CompleteAction));
            return tcs.Task;

            void CompleteAction(Task<T> t)
            {
                if (t.IsFaulted)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    tcs.TrySetException(t.Exception);
                    return;
                }

                if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                    return;
                }

                if (Interlocked.Increment(ref taskCompletedCount) == allTasks.Count)
                {
                    tcs.SetResult(allTasks.Select(ct => ct.Result).ToArray());
                }
            }
        }

        #region Exception Handling

        /// <summary>
        /// Suppresses default exception handling of a Task that would otherwise re-raise 
        /// the exception on the finalizer thread.
        /// </summary>
        /// <param name="task">The Task to be monitored</param>
        /// <returns>The original Task</returns>
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

        private static async Task TimeoutAfterImpl(this Task task, TimeSpan timeoutPeriod)
        {
            if (task == null) { throw new ArgumentNullException(nameof(task)); }

            var timeoutTask = Task.Delay(timeoutPeriod);

            var finishedTask = await Task.WhenAny(task, timeoutTask);
            if (finishedTask == timeoutTask)
            {
                throw new TimeoutException("Task timed out after: " + timeoutPeriod);
            }
        }

        private static async Task<IEnumerable<Task>> TimeoutAfterImpl(this IEnumerable<Task> tasks, TimeSpan timeoutPeriod)
        {
            if (tasks == null) { throw new ArgumentNullException(nameof(tasks)); }

            var timeoutTask = Task.Delay(timeoutPeriod);
            var tasksList = new List<Task>(tasks) { timeoutTask };
            while (tasksList.Any())
            {
                var finishedTask = await Task.WhenAny(tasksList);
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