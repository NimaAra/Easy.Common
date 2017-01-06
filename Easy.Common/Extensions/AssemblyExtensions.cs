namespace Easy.Common.Extensions
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Versioning;

    /// <summary>
    /// A set of extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Obtains the .NET framework version against which the <paramref name="assembly"/> has been built.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The .NET framework version</returns>
        public static string GetFrameworkVersion(this Assembly assembly)
        {
            var targetFrameAttribute = assembly.GetCustomAttributes(true)
                .OfType<TargetFrameworkAttribute>().FirstOrDefault();

            if (targetFrameAttribute == null)
            {
                return ".NET 2, 3 or 3.5";
            }

            var result = targetFrameAttribute.FrameworkDisplayName;

            if (result == null)
            {
                result = targetFrameAttribute.FrameworkName;
            }

            return result.Replace(".NETFramework", ".NET").Replace(".NET Framework", ".NET");
        }

        /// <summary>
        /// Obtains the location from which the <paramref name="assembly"/> was loaded.
        /// <remarks>
        /// <para>
        /// The <c>CodeBase</c> is a URL to the place where the file was found, while the <c>Location</c> is 
        /// the path from where it was actually loaded. For example, if the assembly was downloaded from the 
        /// web, its <c>CodeBase</c> may start with “http://”, but its <c>Location</c> may start with “C:\”. 
        /// If the file was shadow copied, the <c>Location</c> would be the path to the copy of the file in the 
        /// shadow-copy directory.
        /// </para>
        /// <para>
        /// Note  that the <c>CodeBase</c> is not guaranteed to be set for assemblies in the GAC. 
        /// <c>Location</c> will always be set for assemblies loaded from disk however.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="assembly">The assembly for which location is returned</param>
        /// <returns>The location as <see cref="DirectoryInfo"/></returns>
        public static DirectoryInfo GetAssemblyLocation(this Assembly assembly)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new DirectoryInfo(Path.GetDirectoryName(assembly.Location));
        }

        /// <summary>
        /// Obtains the location from which the <paramref name="assembly"/> was found.
        /// <remarks>
        /// <para>
        /// The <c>CodeBase</c> is a URL to the place where the file was found, while the <c>Location</c> is 
        /// the path from where it was actually loaded. For example, if the assembly was downloaded from the 
        /// web, its <c>CodeBase</c> may start with “http://”, but its <c>Location</c> may start with “C:\”. 
        /// If the file was shadow copied, the <c>Location</c> would be the path to the copy of the file in the 
        /// shadow-copy directory.
        /// </para>
        /// <para>
        /// Note  that the <c>CodeBase</c> is not guaranteed to be set for assemblies in the GAC. 
        /// <c>Location</c> will always be set for assemblies loaded from disk however.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="assembly">The assembly for which location is returned</param>
        /// <returns>The location as <see cref="DirectoryInfo"/></returns>
        public static DirectoryInfo GetAssemblyCodeBase(this Assembly assembly)
        {
            var uri = new Uri(assembly.CodeBase);
            // ReSharper disable once AssignNullToNotNullAttribute
            return new DirectoryInfo(Path.GetDirectoryName(uri.LocalPath));
        }

        /// <summary>
        /// Determines whether the given <paramref name="assembly"/> has been compiled in <c>Release</c> mode.
        /// Credit to: <see href="http://www.hanselman.com/blog/HowToProgrammaticallyDetectIfAnAssemblyIsCompiledInDebugOrReleaseMode.aspx"/>
        /// </summary>
        /// <param name="assembly">The assembly to examine</param>
        /// <returns><c>True</c> if the <paramref name="assembly"/> is optimized otherwise <c>False</c></returns>
        public static bool IsOptimized(this Assembly assembly)
        {
            var attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);

            if (attributes.Length == 0) { return true; }

            foreach (Attribute attr in attributes)
            {
                if (attr is DebuggableAttribute)
                {
                    var d = attr as DebuggableAttribute;
                    // FYI
                    // "Run time Optimizer is enabled: " + !d.IsJITOptimizerDisabled
                    // "Run time Tracking is enabled: " + d.IsJITTrackingEnabled
                    if (d.IsJITOptimizerDisabled) { return false; }
                    return true;
                }
            }
            return false;
        }
    }

}