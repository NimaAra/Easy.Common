// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;

    /// <summary>
    /// Represents the details of the process via which the application is running.
    /// </summary>
    public sealed record ProcessDetails
    {
        /// <summary>
        /// Gets the process ID (PID).
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PID { get; init; }
        
        /// <summary>
        /// Gets the process name.
        /// </summary>
        public string Name { get; init; }
        
        /// <summary>
        /// Gets the time at which the process was started.
        /// </summary>
        public DateTimeOffset Started { get; init; }
        
        /// <summary>
        /// Gets the time taken for the process to be loaded.
        /// </summary>
        public TimeSpan LoadedIn { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the application has been compiled in <c>Release</c> mode.
        /// </summary>
        public bool IsOptimized { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the process is <c>64-bit</c>.
        /// </summary>
        public bool Is64Bit { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the <c>GC</c> mode is <c>Server</c>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool IsServerGC { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the process is <c>Large Address Aware</c>.
        /// </summary>
        public bool IsLargeAddressAware { get; init; }

        /// <summary>
        /// Gets the number of threads owned by the process.
        /// </summary>
        public uint ThreadCount { get; init; }

        /// <summary>
        /// Gets the minimum number of worker threads in the <c>ThreadPool</c>.
        /// </summary>
        public uint ThreadPoolMinWorkerCount { get; init; }

        /// <summary>
        /// Gets the maximum number of worker threads in the <c>ThreadPool</c>.
        /// </summary>
        public uint ThreadPoolMaxWorkerCount { get; init; }

        /// <summary>
        /// Gets the minimum number of completion port worker threads in the <c>ThreadPool</c>.
        /// </summary>
        public uint ThreadPoolMinCompletionPortCount { get; init; }

        /// <summary>
        /// Gets the maximum number of completion port worker threads in the <c>ThreadPool</c>.
        /// </summary>
        public uint ThreadPoolMaxCompletionPortCount { get; init; }

        /// <summary>
        /// Gets the name of the process module.
        /// </summary>
        public string ModuleName { get; init; }

        /// <summary>
        /// Gets the file representing the process module.
        /// </summary>
        public string ModuleFileName { get; init; }

        /// <summary>
        /// Gets the name of the product the process is distributed with.
        /// </summary>
        public string ProductName { get; init; }

        /// <summary>
        /// Gets the name of the file the process was created as.
        /// </summary>
        public string OriginalFileName { get; init; }

        /// <summary>
        /// Gets the file name representing the process.
        /// </summary>
        public string FileName { get; init; }

        /// <summary>
        /// Gets the file version of the process.
        /// </summary>
        public string FileVersion { get; init; }

        /// <summary>
        /// Gets the version of the product the process is distributed with
        /// </summary>
        public string ProductVersion { get; init; }

        /// <summary>
        /// Gets the default language for the process.
        /// </summary>
        public string Language { get; init; }

        /// <summary>
        /// Gets the copyright notices that apply to the process.
        /// </summary>
        public string Copyright { get; init; }

        /// <summary>
        /// Gets the current value of Working Set memory (RAM) in use by the process. 
        /// <remarks>This value includes both Shared and Private memory.</remarks>
        /// </summary>
        public double WorkingSetInMegaBytes { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the process is running in <c>User Interactive</c> mode.
        /// <remarks>This will be false for a Windows Service or a service such as IIS that runs without a UI.</remarks>
        /// </summary>
        public bool IsInteractive { get; init; }

        /// <summary>
        /// Gets the <c>CommandLine</c> including any arguments passed into the process.
        /// </summary>
        public string[] CommandLine { get; init; }
    }
}