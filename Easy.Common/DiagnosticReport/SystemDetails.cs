// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the details of the system under which the application is running.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class SystemDetails
    {
        /// <summary>
        /// Gets the name of the operating system.
        /// </summary>
        public string OSName { get; internal set; }
        
        /// <summary>
        /// Gets the type of the operating system e.g <c>Windows, Linux or OSX</c>.
        /// </summary>
        public string OSType { get; internal set; }
        
        /// <summary>
        /// Gets the flag indicating whether the operating system is 64-bit capable.
        /// </summary>
        public bool Is64BitOS { get; internal set; }
        
        /// <summary>
        /// Gets the version of the <c>.NET</c> framework.
        /// </summary>
        public string DotNetFrameworkVersion { get; internal set; }
        
        /// <summary>
        /// Gets the machine name.
        /// </summary>
        public string MachineName { get; internal set; }

        /// <summary>
        /// Gets the Fully Qualified Domain Name (FQDN).
        /// </summary>
        public string FQDN { get; internal set; }

        /// <summary>
        /// Gets the user under which the process is running.
        /// </summary>
        public string User { get; internal set; }

        /// <summary>
        /// Gets the processor name.
        /// </summary>
        public string CPU { get; internal set; }
        
        /// <summary>
        /// Gets the number of processor cores including <c>Hyper Threading</c>.
        /// </summary>
        public uint CPUCoreCount { get; internal set; }

        /// <summary>
        /// Gets the number of installed <c>RAM</c>.
        /// </summary>
        public long InstalledRAMInGigaBytes { get; internal set; }

        /// <summary>
        /// Gets the location of the <c>System</c> directory.
        /// </summary>
        public string SystemDirectory { get; internal set; }

        /// <summary>
        /// Gets the location of the <c>Current</c> directory the application is running.
        /// </summary>
        public string CurrentDirectory { get; internal set; }

        /// <summary>
        /// Gets the location where the <c>CLR</c> is installed.
        /// </summary>
        public string RuntimeDirectory { get; internal set; }
        
        /// <summary>
        /// Gets the duration the system has been up.
        /// </summary>
        public TimeSpan Uptime { get; internal set; }
    }
}