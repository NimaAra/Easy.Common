namespace Easy.Common.Extensions;

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
        Ensure.NotNull(assembly, nameof(assembly));

        TargetFrameworkAttribute? targetFrameAttribute = assembly.GetCustomAttributes(true)
            .OfType<TargetFrameworkAttribute>().FirstOrDefault();

        if (targetFrameAttribute is null) { return ".NET 2, 3 or 3.5"; }

        return targetFrameAttribute.FrameworkName;
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
        Ensure.NotNull(assembly, nameof(assembly));

        return new DirectoryInfo(Path.GetDirectoryName(assembly.Location) ?? throw new InvalidOperationException());
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
        Ensure.NotNull(assembly, nameof(assembly));

        Uri uri = new(assembly.Location);
        string path = Ensure.NotNull(Path.GetDirectoryName(uri.LocalPath), nameof(uri.LocalPath));
        return new DirectoryInfo(path);
    }

    /// <summary>
    /// Determines whether the given <paramref name="assembly"/> has been compiled in <c>Release</c> mode.
    /// Credit to: <see href="http://www.hanselman.com/blog/HowToProgrammaticallyDetectIfAnAssemblyIsCompiledInDebugOrReleaseMode.aspx"/>
    /// </summary>
    /// <param name="assembly">The assembly to examine</param>
    /// <returns><c>True</c> if the <paramref name="assembly"/> is optimized otherwise <c>False</c></returns>
    public static bool IsOptimized(this Assembly assembly)
    {
        Ensure.NotNull(assembly, nameof(assembly));

        object[] attributes = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false);

        if (attributes.Length == 0) { return true; }

        foreach (Attribute attr in attributes)
        {
            if (attr is DebuggableAttribute d)
            {
                // FYI
                // "Run time Optimizer is enabled: " + !d.IsJITOptimizerDisabled
                // "Run time Tracking is enabled: " + d.IsJITTrackingEnabled
                return !d.IsJITOptimizerDisabled;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets the flag indicating whether the given <paramref name="assembly"/> is <c>32-bit</c>.
    /// </summary>
    public static bool Is32Bit(this Assembly assembly)
    {
        Ensure.NotNull(assembly, nameof(assembly));
            
        string location = assembly.Location;
#pragma warning disable SYSLIB0012
        if (location.IsNullOrEmptyOrWhiteSpace()) { location = assembly.CodeBase!; }
#pragma warning restore SYSLIB0012
            
        Uri uri = new(location);
        Ensure.That(uri.IsFile, "Assembly location is not a file.");

        AssemblyName assemblyName = AssemblyName.GetAssemblyName(uri.LocalPath);
#pragma warning disable SYSLIB0037
        return assemblyName.ProcessorArchitecture == ProcessorArchitecture.X86;
#pragma warning restore SYSLIB0037
    }

    /// <summary>
    /// Queries the assembly's headers to find if it is <c>LARGEADDRESSAWARE</c>.
    /// <remarks>The method is equivalent to running <c>DumpBin</c> on the assembly.</remarks>
    /// </summary>
    public static bool IsAssemblyLargeAddressAware(this Assembly assembly)
    {
        Ensure.NotNull(assembly, nameof(assembly));

        return ApplicationHelper.IsLargeAddressAware(assembly.Location);
    }

    /// <summary>
    /// Gets the <see cref="FileVersionInfo"/> for the given <paramref name="assembly"/>.
    /// </summary>
    public static FileVersionInfo GetFileVersionInfo(this Assembly assembly)
    {
        string assLoc = assembly.Location;
        if (assLoc.IsNotNullOrEmptyOrWhiteSpace())
        {
            return FileVersionInfo.GetVersionInfo(assLoc);
        }

        Uri uri = new(assembly.Location);
        assLoc = uri.LocalPath;
        return FileVersionInfo.GetVersionInfo(assLoc);
    }
}