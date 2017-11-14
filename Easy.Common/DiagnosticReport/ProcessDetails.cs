// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;

    /// <summary>
    /// Represents the details of the process via which the application is running.
    /// </summary>
    public sealed class ProcessDetails
    {
        /// <summary>
        /// Gets the process ID (PID).
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PID { get; internal set; }
        
        /// <summary>
        /// Gets the process name.
        /// </summary>
        public string Name { get; internal set; }
        
        /// <summary>
        /// Gets the time at which the process was started.
        /// </summary>
        public DateTimeOffset Started { get; internal set; }
        
        /// <summary>
        /// Gets the time taken for the process to be loaded.
        /// </summary>
        public TimeSpan LoadedIn { get; internal set; }

        /// <summary>
        /// Gets the flag indicating whether the application has been compiled in <c>Release</c> mode.
        /// </summary>
        public string IsOptimized { get; internal set; }

        /// <summary>
        /// Gets the flag indicating whether the process is <c>64-bit</c>.
        /// </summary>
        public bool Is64Bit { get; internal set; }

        /// <summary>
        /// Gets the flag indicating whether the process is <c>Large Address Aware</c>.
        /// </summary>
        public bool IsLargeAddressAware { get; internal set; }

        /// <summary>
        /// Gets the name of the process module.
        /// </summary>
        public string ModuleName { get; internal set; }

        /// <summary>
        /// Gets the file representing the process module.
        /// </summary>
        public string ModuleFileName { get; internal set; }

        /// <summary>
        /// Gets the name of the product the process is distributed with.
        /// </summary>
        public string ProductName { get; internal set; }

        /// <summary>
        /// Gets the name of the file the process was created as.
        /// </summary>
        public string OriginalFileName { get; internal set; }

        /// <summary>
        /// Gets the file name representing the process.
        /// </summary>
        public string FileName { get; internal set; }

        /// <summary>
        /// Gets the file version of the process.
        /// </summary>
        public string FileVersion { get; internal set; }

        /// <summary>
        /// Gets the version of the product the process is distributed with
        /// </summary>
        public string ProductVersion { get; internal set; }

        /// <summary>
        /// Gets the default language for the process.
        /// </summary>
        public string Language { get; internal set; }

        /// <summary>
        /// Gets the copyright notices that apply to the process.
        /// </summary>
        public string Copyright { get; internal set; }

        /// <summary>
        /// Gets the current value of Working Set memory (RAM) in use by the process. 
        /// <remarks>This value includes both Shared and Private memory.</remarks>
        /// </summary>
        public double WorkingSetInMegaBytes { get; internal set; }

        /// <summary>
        /// Gets the flag indicating whether the process is running in <c>User Interactive</c> mode.
        /// <remarks>This will be false for a Windows Service or a service such as IIS that runs without a UI.</remarks>
        /// </summary>
        public bool IsInteractive { get; internal set; }

        /// <summary>
        /// Gets the <c>CommandLine</c> including any arguments passed into the process.
        /// </summary>
        public string[] CommandLine { get; internal set; }
    }
}