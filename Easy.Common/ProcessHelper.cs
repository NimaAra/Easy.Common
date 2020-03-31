namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a set of methods to help work with a <see cref="Process"/>.
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// Starts a Process Asynchronously.
        /// <remarks><see href="http://www.levibotelho.com/development/async-processes-with-taskcompletionsource/"/></remarks>
        /// </summary>
        /// <param name="processInfo">The information for the process to run.</param>
        /// <param name="cToken">The cancellation token.</param>
        /// <returns>A task representing the started process which you can await until process exits.</returns>
        public static Task<ProcessExecutionResult> ExecuteAsync(ProcessStartInfo processInfo, CancellationToken cToken = default(CancellationToken))
        {
            Ensure.NotNull(processInfo, nameof(processInfo));

            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = processInfo
            };

            var tcs = new TaskCompletionSource<ProcessExecutionResult>();
            var standardOutput = new List<string>();
            var standardError = new List<string>();

            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardOutput.Add(args.Data);
                } 
                else
                {
                    standardOutputResults.SetResult(standardOutput.ToArray());
                }
            };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardError.Add(args.Data);
                } else
                {
                    standardErrorResults.SetResult(standardError.ToArray());
                }
            };

            process.Exited += (sender, args) =>
            {
                // Since the Exited event can happen asynchronously to the output and error events, 
                // we use the task results for stdout/stderr to ensure they are both closed
                tcs.TrySetResult(new ProcessExecutionResult(process, standardOutputResults.Task.Result, standardErrorResults.Task.Result));
            };

            using (cToken.Register(() =>
                {
                    tcs.TrySetCanceled();
                    try
                    {
                        if (!process.HasExited) { process.Kill(); }
                    } catch (InvalidOperationException) { }
                }))
            {
                cToken.ThrowIfCancellationRequested();

                if (!process.Start())
                {
                    tcs.TrySetException(new InvalidOperationException("Failed to start the process."));
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                return tcs.Task;
            }
        }

        /// <summary>
        /// Starts a process represented by <paramref name="processPath"/> asynchronously.
        /// <remarks><see href="http://www.levibotelho.com/development/async-processes-with-taskcompletionsource/"/></remarks>
        /// </summary>
        /// <param name="processPath">The path to the process.</param>
        /// <param name="args">The arguments to be passed to the process.</param>
        /// <param name="cToken">The cancellation token.</param>
        /// <returns>A task representing the started process which you can await until process exits.</returns>
        public static Task<ProcessExecutionResult> ExecuteAsync(string processPath, string args, CancellationToken cToken = default(CancellationToken))
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(processPath);
            return ExecuteAsync(new ProcessStartInfo(processPath, args), cToken);
        }

        /// <summary>
        /// Starts a process represented by <paramref name="processPath"/> and <paramref name="args"/> asynchronously.
        /// <remarks><see href="http://www.levibotelho.com/development/async-processes-with-taskcompletionsource/"/></remarks>
        /// </summary>
        /// <param name="processPath">The path to the process.</param>
        /// <param name="args">The arguments to be passed to the process.</param>
        /// <param name="cToken">The cancellation token.</param>
        /// <returns>A task representing the started process which you can await until process exits.</returns>
        public static Task<ProcessExecutionResult> ExecuteAsync(FileInfo processPath, string args, CancellationToken cToken = default(CancellationToken))
        {
            Ensure.NotNull(processPath, nameof(processPath));
            return ExecuteAsync(new ProcessStartInfo(processPath.FullName, args), cToken);
        }
    }

    /// <summary>
    /// Represents the result of executing a process.
    /// </summary>
    public sealed class ProcessExecutionResult : IDisposable
    {
        private readonly Process _process;

        internal ProcessExecutionResult(Process process, string[] standardOutput, string[] standardError)
        {
            _process = Ensure.NotNull(process, nameof(process));
            
            PID = _process.Id;
            ExecutionTime = _process.ExitTime - _process.StartTime;
            StandardOutput = Ensure.NotNull(standardOutput, nameof(standardOutput));
            StandardError = Ensure.NotNull(standardError, nameof(standardError));
        }

        /// <summary>
        /// Gets the process ID.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PID { get; }

        /// <summary>
        /// Gets the execution time of the process.
        /// </summary>
        public TimeSpan ExecutionTime { get; }

        /// <summary>
        /// Gets the standard output of the process.
        /// </summary>
        public string[] StandardOutput { get; }

        /// <summary>
        /// Gets the standard error of the process.
        /// </summary>
        public string[] StandardError { get; }

        /// <summary>
        /// Read the value of the process property identified by the given <paramref name="selector"/>.
        /// </summary>
        public T ReadProcessInfo<T>(Func<Process, T> selector) => selector(_process);

        /// <summary>
        /// Releases all resources used by the underlying process.
        /// </summary>
        public void Dispose() => _process?.Dispose();
    }
}