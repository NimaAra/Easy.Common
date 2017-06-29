namespace Easy.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;

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
        /// <returns>A task representing the started process which you can await until process exits.</returns>
        public static Task RunProcessAsync(ProcessStartInfo processInfo)
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

            var tcs = new TaskCompletionSource<object>();
            process.Exited += (sender, args) =>
            {
                var p = (Process)sender;

                p.OutputDataReceived -= OnOutputDataReceived;
                p.ErrorDataReceived -= OnErrorDataReceived;

                if (p.ExitCode != 0)
                {
                    tcs.TrySetException(new InvalidOperationException("The process did not exit gracefully. Search for [Process Error]"));
                } else
                {
                    tcs.SetResult(null);
                }

                p.Dispose();
            };

            process.OutputDataReceived += OnOutputDataReceived;
            process.ErrorDataReceived += OnErrorDataReceived;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return tcs.Task;
        }

        /// <summary>
        /// Starts a process represented by <paramref name="processPath"/> asynchronously.
        /// <remarks><see href="http://www.levibotelho.com/development/async-processes-with-taskcompletionsource/"/></remarks>
        /// </summary>
        /// <param name="processPath">The path to the process.</param>
        /// <returns>A task representing the started process which you can await until process exits.</returns>
        public static Task RunProcessAsync(string processPath)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(processPath);
            return RunProcessAsync(new ProcessStartInfo(processPath));
        }

        /// <summary>
        /// Starts a process represented by <paramref name="processPath"/> and <paramref name="args"/> asynchronously.
        /// <remarks><see href="http://www.levibotelho.com/development/async-processes-with-taskcompletionsource/"/></remarks>
        /// </summary>
        /// <param name="processPath">The path to the process.</param>
        /// <param name="args">The arguments to be passed to the process.</param>
        /// <returns>A task representing the started process which you can await until process exits.</returns>
        public static Task RunProcessAsync(FileInfo processPath, string args)
        {
            Ensure.NotNull(processPath, nameof(processPath));
            Ensure.NotNullOrEmptyOrWhiteSpace(args);
            return RunProcessAsync(new ProcessStartInfo(processPath.FullName, args));
        }

        private static void OnErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            var errMsg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - [Process Error] | ";

            if (args?.Data != null)
            {
                errMsg += args.Data;
            } else
            {
                if (((Process)sender).HasExited)
                { return; }

                errMsg += "No data available";
            }

            errMsg.Print();
        }

        private static void OnOutputDataReceived(object o, DataReceivedEventArgs args)
        {
            args?.Data?.Print();
        }
    }
}