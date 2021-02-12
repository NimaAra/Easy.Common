// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;

    /// <summary>
    /// Represents the details of the assemblies referenced by the application.
    /// </summary>
    public sealed record AssemblyDetails
    {
        /// <summary>
        /// Gets the full name of the assembly.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the assembly has been loaded from the <c>GAC</c>.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public bool IsGAC { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the assembly is <c>64-bit</c>.
        /// </summary>
        public bool Is64Bit { get; init; }

        /// <summary>
        /// Gets the flag indicating whether the assembly has been compiled in <c>Release</c> mode.
        /// </summary>
        public bool IsOptimized { get; init; }

        /// <summary>
        /// Gets the framework for which the assembly has been compiled.
        /// </summary>
        public string Framework { get; init; }

        /// <summary>
        /// Gets the location from which the assembly has been loaded from.
        /// </summary>
        public string Location { get; init; }

        /// <summary>
        /// Gets the path to the location where the assembly was found.
        /// <remarks>
        /// If the assembly was downloaded from the web, this value may start with <c>http</c>.
        /// </remarks> 
        /// </summary>
        public Uri CodeBase { get; init; }
    }
}