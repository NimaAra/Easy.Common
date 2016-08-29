namespace Easy.Common
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;

    /// <summary>
    /// Provides a set of methods to help work with a <see cref="Process"/>.
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// Starts a Process Asynchronously;
        /// http://www.levibotelho.com/development/async-processes-with-taskcompletionsource/
        /// </summary>
        /// <param name="processInfo">The information for the process to run</param>
        /// <returns>A task representing the started process</returns>
        public static Task RunProcessAsync(ProcessStartInfo processInfo)
        {
            Ensure.NotNull(processInfo, nameof(processInfo));

            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<object>();
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = processInfo
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (args?.Data == null) { return; }
                args.Data.Print();
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                var errMsg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - [Process Error] | ";

                if (args?.Data != null)
                {
                    errMsg += args.Data;
                }
                else
                {
                    errMsg += "No data available";
                }

                errMsg.Print();
            };

            process.Exited += (sender, args) =>
            {
                if (process.ExitCode != 0)
                {
                    tcs.TrySetException(new InvalidOperationException("The process did not exit gracefully. Search for [Process Error]"));
                }
                else
                {
                    tcs.SetResult(null);
                }

                process.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return tcs.Task;
        }

        /// <summary>
        /// Starts a process represented by <paramref name="processPath"/> asynchronously
        /// </summary>
        /// <param name="processPath">The path to the process</param>
        /// <returns>A task representing the started process</returns>
        public static Task RunProcessAsync(string processPath)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(processPath);
            return RunProcessAsync(new ProcessStartInfo(processPath));
        }
    }

}