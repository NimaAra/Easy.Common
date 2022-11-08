// ReSharper disable once CheckNamespace
namespace Easy.Common;

using System;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents the details of the assemblies referenced by the application.
/// <param name="FullName">Gets the full name of the assembly.</param>
/// <param name="FileName">Gets the file name of the assembly.</param>
/// <param name="IsGAC">Gets the flag indicating whether the assembly has been loaded from the <c>GAC</c>.</param>
/// <param name="Is64Bit">Gets the flag indicating whether the assembly is <c>64-bit</c>.</param>
/// <param name="IsOptimized">Gets the flag indicating whether the assembly has been compiled in <c>Release</c> mode.</param>
/// <param name="Framework">Gets the framework for which the assembly has been compiled.</param>
/// <param name="Location">Gets the location from which the assembly has been loaded from.</param>
/// <param name="CodeBase">
/// Gets the path to the location where the assembly was found. If the assembly was downloaded from the web, this value may start with <c>http</c>.
/// </param>
/// <param name="Version">Gets the version of the assembly.</param>
/// <param name="FileVersion">Gets the file version of the assembly.</param>
/// <param name="ProductVersion">Gets the production version of the assembly.</param>
/// <param name="ProductName">Gets the product name of the assembly.</param>
/// <param name="CompanyName">Gets the company name of the assembly.</param>
/// </summary>
public sealed record class AssemblyDetails(
    string FullName, string FileName, bool IsGAC, bool Is64Bit, bool IsOptimized, string Framework, string Location,
    Uri CodeBase, string Version, string FileVersion, string ProductVersion, string ProductName, string CompanyName);

/// <summary>
/// Represents the details of the drives installed in the system.
/// <param name="Name">Gets the name of the drive.</param>
/// <param name="Type">Gets the type of the drive e.g. <c>Fixed, CDRom</c> etc.</param>
/// <param name="Format">Gets the format of the drive e.g. <c>NTFS</c>.</param>
/// <param name="Label">Gets the label of the drive.</param>
/// <param name="TotalCapacityInGigaBytes">Gets the capacity of the drive.</param>
/// <param name="FreeCapacityInGigaBytes">Gets the total amount of free space available on the drive (not just what is available to the current user).</param>
/// <param name="AvailableCapacityInGigaBytes">Gets the amount of free space available on the drive. This value takes into account disk quotas.</param>
/// </summary>
public sealed record class DriveDetails(
    string Name, string Type, string Format, string Label,
    double TotalCapacityInGigaBytes, double FreeCapacityInGigaBytes, double AvailableCapacityInGigaBytes);

/// <summary>
/// Represents the details of the system under which the application is running.
/// <param name="OSName">Gets the name of the operating system.</param>
/// <param name="OSType">Gets the type of the operating system e.g <c>Windows, Linux or OSX</c>.</param>
/// <param name="Is64BitOS">Gets the flag indicating whether the operating system is 64-bit capable.</param>
/// <param name="DotNetFrameworkVersion">Gets the version of the <c>.NET</c> framework.</param>
/// <param name="MachineName">Gets the machine name.</param>
/// <param name="FQDN">Gets the Fully Qualified Domain Name (FQDN).</param>
/// <param name="User">Gets the user under which the process is running.</param>
/// <param name="CPU">Gets the processor name.</param>
/// <param name="CPUCoreCount">Gets the number of processor cores including <c>Hyper Threading</c>.</param>
/// <param name="InstalledRAMInGigaBytes">Gets the number of installed <c>RAM</c>.</param>
/// <param name="SystemDirectory">Gets the location of the <c>System</c> directory.</param>
/// <param name="CurrentDirectory">Gets the location of the <c>Current</c> directory the application is running.</param>
/// <param name="RuntimeDirectory">Gets the location where the <c>CLR</c> is installed.</param>
/// <param name="Uptime">Gets the duration the system has been up.</param>
/// </summary>
public sealed record class SystemDetails(
    string OSName, string OSType, bool Is64BitOS, string DotNetFrameworkVersion, string MachineName,
    string FQDN, string User, string CPU, uint CPUCoreCount, long InstalledRAMInGigaBytes,
    string SystemDirectory, string CurrentDirectory, string RuntimeDirectory, TimeSpan Uptime
);

/// <summary>
/// Represents the details of the process via which the application is running.
/// <param name="PID">Gets the process ID (PID).</param>
/// <param name="Name">Gets the process name.</param>
/// <param name="Started">Gets the time at which the process was started.</param>
/// <param name="LoadedIn">Gets the time taken for the process to be loaded.</param>
/// <param name="IsOptimized">Gets the flag indicating whether the application has been compiled in <c>Release</c> mode.</param>
/// <param name="Is64Bit">Gets the flag indicating whether the process is <c>64-bit</c>.</param>
/// <param name="IsServerGC">Gets the flag indicating whether the <c>GC</c> mode is <c>Server</c>.</param>
/// <param name="IsLargeAddressAware">Gets the flag indicating whether the process is <c>Large Address Aware</c>.</param>
/// <param name="ThreadCount">Gets the number of threads owned by the process.</param>
/// <param name="ThreadPoolMinWorkerCount">Gets the minimum number of worker threads in the <c>ThreadPool</c>.</param>
/// <param name="ThreadPoolMaxWorkerCount">Gets the maximum number of worker threads in the <c>ThreadPool</c>.</param>
/// <param name="ThreadPoolMinCompletionPortCount">Gets the minimum number of completion port worker threads in the <c>ThreadPool</c>.</param>
/// <param name="ThreadPoolMaxCompletionPortCount">Gets the maximum number of completion port worker threads in the <c>ThreadPool</c>.</param>
/// <param name="ModuleName">Gets the name of the process module.</param>
/// <param name="ModuleFileName">Gets the file representing the process module.</param>
/// <param name="ProductName">Gets the name of the product the process is distributed with.</param>
/// <param name="OriginalFileName">Gets the name of the file the process was created as.</param>
/// <param name="FileName">Gets the file name representing the process.</param>
/// <param name="FileVersion">Gets the file version of the process.</param>
/// <param name="ProductVersion">Gets the version of the product the process is distributed with.</param>
/// <param name="Language">Gets the default language for the process.</param>
/// <param name="Copyright">Gets the copyright notices that apply to the process.</param>
/// <param name="WorkingSetInMegaBytes">Gets the current value of Working Set memory (RAM) in use by the process. This value includes both Shared and Private memory.</param>
/// <param name="IsInteractive">Gets the flag indicating whether the process is running in <c>User Interactive</c> mode. This will be false for a Windows Service or a service such as IIS that runs without a UI.</param>
/// <param name="CommandLine">Gets the <c>CommandLine</c> including any arguments passed into the process.</param>
/// </summary>
public sealed record class ProcessDetails(
    int PID, string Name, DateTimeOffset Started, TimeSpan LoadedIn, bool IsOptimized, bool Is64Bit,
    bool IsServerGC, bool IsLargeAddressAware, uint ThreadCount, uint ThreadPoolMinWorkerCount,
    uint ThreadPoolMaxWorkerCount, uint ThreadPoolMinCompletionPortCount, uint ThreadPoolMaxCompletionPortCount, 
    string ModuleName, string ModuleFileName, string ProductName, string OriginalFileName, string FileName, string FileVersion, 
    string ProductVersion, string Language, string Copyright, double WorkingSetInMegaBytes, bool IsInteractive, string[] CommandLine);

/// <summary>
/// Represents the details of the network available in the local computer.
/// <param name="DHCPScope">Gets the <c>Dynamic Host Configuration Protocol</c> (DHCP) scope name.</param>
/// <param name="Domain">Gets the domain in which the local computer is registered.</param>
/// <param name="Host">Gets the host name for the local computer.</param>
/// <param name="IsWINSProxy">Gets the flag indicating whether the local computer is acting as a  <c>Windows Internet Name Service</c> (WINS) proxy.</param>
/// <param name="NodeType">Gets the <c>Network Basic Input/Output System</c> (NetBIOS) node type of the local computer.</param>
/// <param name="InterfaceDetails">Gets the details of the network interfaces in the local computer.</param>
/// </summary>
public sealed record class NetworkDetails(
    string DHCPScope, string Domain, string Host, bool IsWINSProxy, string NodeType, NetworkInterfaceDetails[] InterfaceDetails);

/// <summary>
/// Represents the details of the network interface in the local computer.
/// <param name="Id">Gets the Id of the network interface.</param>
/// <param name="MAC">Gets the physical address a.k.a. <c>Media Access Control</c> address of the interface.</param>
/// <param name="Name">Gets the name of the network interface.</param>
/// <param name="Description">Gets the description of the network interface.</param>
/// <param name="Type">Gets the interface type.</param>
/// <param name="Speed">Gets the speed of the interface in bits per second as reported by the interface.</param>
/// <param name="IsReceiveOnly">Gets a bool value that indicates whether the network interface is set to only receive data packets.</param>
/// <param name="SupportsMulticast">Gets a bool value that indicates whether this network interface is enabled to receive multi-cast packets.</param>
/// <param name="OperationalStatus">Gets the current operational state of the network connection.</param>
/// <param name="Addresses">Gets all of the <c>IP</c> addresses currently assigned to the interface.</param>
/// </summary>
public sealed record class NetworkInterfaceDetails(
    string Id, string MAC, string Name, string Description, string Type, 
    long Speed, bool IsReceiveOnly, bool SupportsMulticast, string OperationalStatus, IPAddressDetails[] Addresses);

/// <summary>
/// Represents the details of the IP address
/// <param name="AddressFamily">Gets the address family of the IP address.</param>
/// <param name="IsIPv6Multicast">Gets whether the address is an IPv6 multi-cast global address.</param>
/// <param name="IsIPv6LinkLocal">Gets whether the address is an IPv6 link local address.</param>
/// <param name="IsIPv6SiteLocal">Gets whether the address is an IPv6 site local address.</param>
/// <param name="IsIPv6Teredo">Gets whether the address is an IPv6 Teredo address.</param>
/// <param name="IsIPv4MappedToIPv6">Gets whether the IP address is an IPv4-mapped IPv6 address.</param>
/// <param name="AsString">Gets the IP address as string in dotted-quad notation for IPv4 and in colon-hexadecimal notation for IPv6.</param>
/// </summary>
public sealed record class IPAddressDetails(
    AddressFamily AddressFamily, bool IsIPv6Multicast, bool IsIPv6LinkLocal, bool IsIPv6SiteLocal, bool IsIPv6Teredo, bool IsIPv4MappedToIPv6, string AsString)
{
    /// <summary>
    /// Returns a <see cref="IPAddressDetails"/> from the given <paramref name="ipAddress"/>.
    /// </summary>
    internal static IPAddressDetails From(IPAddress ipAddress) => new(
        ipAddress.AddressFamily,
        ipAddress.IsIPv6Multicast,
        ipAddress.IsIPv6LinkLocal,
        ipAddress.IsIPv6SiteLocal,
        ipAddress.IsIPv6Teredo,
        ipAddress.IsIPv4MappedToIPv6,
        ipAddress.ToString());
}
    
/// <summary>
/// Represents the different types of report which can 
/// be generated by <see cref="DiagnosticReport"/>.
/// <remarks>
/// Reports can be mixed as: 
/// <example>DiagnosticReportType.System | DiagnosticReportType.Process</example>
/// </remarks>
/// </summary>
[Flags]
public enum DiagnosticReportType
{
    /// <summary>
    /// Includes details about the System.
    /// </summary>
    System = 1,

    /// <summary>
    /// Includes details about the Process.
    /// </summary>
    Process = 2,

    /// <summary>
    /// Includes details about the Drives.
    /// </summary>
    Drives = 4,

    /// <summary>
    /// Includes details about the referenced Assemblies.
    /// </summary>
    Assemblies = 8,

    /// <summary>
    /// Includes details about the Environment Variables.
    /// </summary>
    EnvironmentVariables = 16,

    /// <summary>
    /// Includes details about the Networks.
    /// </summary>
    Networks = 32,

    /// <summary>
    /// Generates the full report.
    /// </summary>
    Full = System | Process | Drives | Assemblies | EnvironmentVariables | Networks
}