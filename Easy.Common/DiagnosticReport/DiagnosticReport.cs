// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Runtime;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Easy.Common.Extensions;

    /// <summary>
    /// A helper class for generating a report containing details related to 
    /// <c>System</c>, <c>Process</c>, <c>Assemblies</c>, <c>Networks</c> and <c>Environment</c> 
    /// on which the application executes.
    /// </summary>
    public sealed class DiagnosticReport
    {
        private const char Pipe = '|';
        private const char Dot = '.';
        private const char Dash = '-';
        private const char Space = ' ';
        private const char Colon = ':';
        private static readonly string NewLine = Environment.NewLine;
        private static readonly string LinePrefix = Pipe.ToString() + "\t";

        private static readonly DiagnosticReportType[] ReportSections =
        {
            DiagnosticReportType.System,
            DiagnosticReportType.Process,
            DiagnosticReportType.Drives,
            DiagnosticReportType.Assemblies,
            DiagnosticReportType.EnvironmentVariables,
            DiagnosticReportType.Networks
        };

        private static readonly IDictionary<DiagnosticReportType, Action<StringBuilder, DiagnosticReport>>
            SectionToFormattingMap = new Dictionary<DiagnosticReportType, Action<StringBuilder, DiagnosticReport>>
            {
                [DiagnosticReportType.System] = AddSystem,
                [DiagnosticReportType.Process] = AddProcess,
                [DiagnosticReportType.Drives] = AddDrives,
                [DiagnosticReportType.Assemblies] = AddAssemblies,
                [DiagnosticReportType.EnvironmentVariables] = AddEnvironmentVariables,
                [DiagnosticReportType.Networks] = AddNetwork
            };

        private static readonly string[] SystemHeaders =
        {
            "OS - Name",
            "OS - Type",
            "OS - 64Bit",
            ".NET Framework",
            "Machine Name",
            "FQDN",
            "Installed RAM",
            "CPU",
            "CPU Core Count",
            "User",
            "System Directory",
            "Current Directory",
            "Runtime Directory",
            "Uptime"
        };

        private static readonly string[] ProcessHeaders =
        {
            "Id",
            "Name",
            "Started",
            "Loaded In",
            "Optimized",
            "64Bit Process",
            "Large Address Aware",
            "Module Name",
            "Module File Name",
            "Product Name",
            "Original File Name",
            "File Name",
            "File Version",
            "Product Version",
            "Language",
            "Copyright",
            "WorkingSet",
            "Interactive",
            "CommandLine",
            "Server GC",
            "Threads",
            "IO Threads",
            "Worker Threads"
        };

        private static readonly string[] DrivesHeaders =
        {
            "Name", "Type", "Format", "Label", "Capacity(GB)", "Free(GB)", "Available(GB)"
        };

        private static readonly string[] AssemblyHeaders =
        {
            "FullName", "FileName", "GAC", "64Bit", "Optimized", "Framework", "Location",
            "CodeBase", "Version", "FileVersion", "ProductVersion", "ProductName", "CompanyName"
        };

        private static readonly string[] NetworkHeaders =
        {
            "Host", "Domain", "DHCP Scope", "Node Type", "Is WINS Proxy", "Name", "MAC",
            "Type", "Status", "Is Receive Only", "Supports Multicast", "Speed", "IP Addresses"
        };

        /// <summary>
        /// Creates a new instance of the <see cref="DiagnosticReport"/>.
        /// </summary>
        private DiagnosticReport(DiagnosticReportType type)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Timestamp = DateTimeOffset.Now;
            Type = type;

            SystemDetails = GetSystemDetails(type);
            ProcessDetails = GetProcessDetails(type);
            DriveDetails = GetDriveDetails(type);
            Assemblies = GetAssemblies(type);
            EnvironmentVariables = GetEnvironmentVariables(type);
            NetworkingDetails = GetNetworkDetails(type);

            TimeTaken = sw.Elapsed;
        }

        /// <summary>
        /// Get the time at which this report was generated.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the time taken to generate this report.
        /// </summary>
        public TimeSpan TimeTaken { get; }

        /// <summary>
        /// Gets the type of this report.
        /// </summary>
        public DiagnosticReportType Type { get; }

        /// <summary>
        /// Gets the information relating to the <c>System</c>.
        /// </summary>
        public SystemDetails SystemDetails { get; }

        /// <summary>
        /// Gets the information relating to the <c>Process</c>.
        /// </summary>
        public ProcessDetails ProcessDetails { get; }

        /// <summary>
        /// Gets the information relating to the <c>Drives</c>.
        /// </summary>
        public DriveDetails[] DriveDetails { get; }

        /// <summary>
        /// Gets the information relating to the referenced <c>Assemblies</c>.
        /// </summary>
        public AssemblyDetails[] Assemblies { get; }

        /// <summary>
        /// Gets the <c>Environment Variables</c>.
        /// </summary>
        public IDictionary<string, string> EnvironmentVariables { get; }

        /// <summary>
        /// Gets the information relating to the <c>Networks</c>.
        /// </summary>
        public NetworkDetails NetworkingDetails { get; }

        /// <summary>
        /// Generates and returns an instance of <see cref="DiagnosticReport"/>.
        /// </summary>
        public static DiagnosticReport Generate(DiagnosticReportType type = DiagnosticReportType.Full) => new DiagnosticReport(type);

        /// <summary>
        /// Returns this report as formatted <see cref="string"/>.
        /// </summary>
        public override string ToString()
        {
            try
            {
                return GenerateImpl(this);
            }
            catch (Exception e)
            {
                return $"Unable to generate the Diagnostic Report. Error:{NewLine}\t{e}";
            }
        }

        private static SystemDetails GetSystemDetails(DiagnosticReportType type)
        {
            if (!IsReportEnabled(type, DiagnosticReportType.System)) { return default; }

            return new SystemDetails
            {
                OSName = RuntimeInformation.OSDescription,
                OSType = GetOSPlatform(),
                Is64BitOS = Environment.Is64BitOperatingSystem,
                DotNetFrameworkVersion = GetDotNetFrameworkVersion(),
                MachineName = Environment.MachineName,
                FQDN = NetworkHelper.GetFQDN(),
                User = Environment.UserDomainName + "\\" + Environment.UserName,
                CPU = GetProcessorName(),
                CPUCoreCount = (uint)Environment.ProcessorCount,
                InstalledRAMInGigaBytes = GetInstalledMemoryInGigaBytes(),
                SystemDirectory = Environment.SystemDirectory,
                CurrentDirectory = Environment.CurrentDirectory,
                RuntimeDirectory = RuntimeEnvironment.GetRuntimeDirectory(),
                Uptime = GetUptime()
            };
        }

        private static ProcessDetails GetProcessDetails(DiagnosticReportType type)
        {
            if (!IsReportEnabled(type, DiagnosticReportType.Process)) { return default; }

            using Process p = Process.GetCurrentProcess();

            string processName = p.ProcessName;
            FileVersionInfo pVerInfo;
            
            Assembly assembly = Assembly.GetEntryAssembly();
            if (p.ProcessName.Equals("dotnet", StringComparison.OrdinalIgnoreCase) && assembly is not null)
            {
                pVerInfo = assembly.GetFileVersionInfo();
                processName = pVerInfo.ProductName;
            }
            else
            {
                pVerInfo = p.MainModule?.FileVersionInfo;
            }

            ThreadPool.GetMinThreads(out int minWrkrs, out int minComplWrkers);
            ThreadPool.GetMaxThreads(out int maxWrkrs, out int maxComplWrkers);

            return new()
            {
                PID = p.Id,
                Name = processName,
                Started = p.StartTime,
                LoadedIn = ApplicationHelper.GetProcessStartupDuration(),
                IsInteractive = Environment.UserInteractive,
                IsOptimized = IsOptimized(),
                Is64Bit = Environment.Is64BitProcess,
                IsServerGC = GCSettings.IsServerGC,
                IsLargeAddressAware = ApplicationHelper.IsProcessLargeAddressAware(),
                WorkingSetInMegaBytes = UnitConverter.BytesToMegaBytes(Environment.WorkingSet),
                FileVersion = pVerInfo?.FileVersion,
                ProductVersion = pVerInfo?.ProductVersion,
                Language = pVerInfo?.Language,
                Copyright = pVerInfo?.LegalCopyright,
                OriginalFileName = pVerInfo?.OriginalFilename,
                FileName = pVerInfo?.FileName,
                ModuleName = p.MainModule?.ModuleName,
                ModuleFileName = p.MainModule?.FileName,
                ProductName = pVerInfo?.ProductName,
                CommandLine = Environment.GetCommandLineArgs(),
                ThreadPoolMinCompletionPortCount = (uint)minComplWrkers,
                ThreadPoolMaxCompletionPortCount = (uint)maxComplWrkers,
                ThreadPoolMinWorkerCount = (uint)minWrkrs,
                ThreadPoolMaxWorkerCount = (uint)maxWrkrs,
                ThreadCount = (uint)p.Threads.OfType<ProcessThread>().Count()
            };

            static string IsOptimized()
            {
                var executingAssembly = Assembly.GetEntryAssembly();
                return executingAssembly is null
                    ? "N/A - Assembly was called from Unmanaged code."
                    : executingAssembly.IsOptimized().ToString();
            }
        }

        private static Dictionary<string, string> GetEnvironmentVariables(DiagnosticReportType type)
        {
            if (!IsReportEnabled(type, DiagnosticReportType.EnvironmentVariables)) { return default; }

            return Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
        }

        private static DriveDetails[] GetDriveDetails(DiagnosticReportType type)
        {
            if (!IsReportEnabled(type, DiagnosticReportType.Drives)) { return default; }

            return DriveInfo.GetDrives()
                .Select(d =>
                {
                    string driveName = "Unknown", driveType = "Unknown", driveFormat = "Unknown", volumeLabel = "Unknown";
                    double capacity = 0, free = 0, available = 0;

                    try
                    {
                        driveName = d.Name;
                        driveType = d.DriveType.ToString();

                        if (d.IsReady)
                        {
                            driveFormat = d.DriveFormat;
                            volumeLabel = d.VolumeLabel;
                            capacity = UnitConverter.BytesToGigaBytes(d.TotalSize);
                            free = UnitConverter.BytesToGigaBytes(d.TotalFreeSpace);
                            available = UnitConverter.BytesToGigaBytes(d.AvailableFreeSpace);
                        }
                    }
                    catch (Exception) { /* ignored */ }

                    return new DriveDetails
                    {
                        Name = driveName,
                        Type = driveType,
                        Format = driveFormat,
                        Label = volumeLabel,
                        TotalCapacityInGigaBytes = capacity,
                        FreeCapacityInGigaBytes = free,
                        AvailableCapacityInGigaBytes = available
                    };
                })
                .ToArray();
        }

        private static AssemblyDetails[] GetAssemblies(DiagnosticReportType type)
        {
            if (!IsReportEnabled(type, DiagnosticReportType.Assemblies)) { return default; }

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(ass => !ass.IsDynamic)
                .OrderByDescending(o => o.GlobalAssemblyCache)
                .Select(ass =>
                {
                    string assLoc = ass.Location;
                    if (assLoc.IsNullOrEmptyOrWhiteSpace())
                    {
                        Uri uri = new(ass.CodeBase);
                        assLoc = uri.LocalPath;
                    }

                    string version = ass.GetName().Version.ToString();
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assLoc);

                    return new AssemblyDetails
                    {
                        FullName = ass.FullName,
                        FileName = versionInfo.OriginalFilename,
                        IsGAC = ass.GlobalAssemblyCache,
                        Is64Bit = !ass.Is32Bit(),
                        IsOptimized = ass.IsOptimized(),
                        Framework = ass.GetFrameworkVersion(),
                        Location = ass.Location,
                        CodeBase = new Uri(ass.CodeBase),
                        Version = version,
                        FileVersion = versionInfo.FileVersion,
                        ProductVersion = versionInfo.ProductVersion,
                        ProductName = versionInfo.ProductName,
                        CompanyName = versionInfo.CompanyName
                    };
                })
                .ToArray();
        }

        private static NetworkDetails GetNetworkDetails(DiagnosticReportType type)
        {
            if (!IsReportEnabled(type, DiagnosticReportType.Networks)) { return default; }

            IPGlobalProperties globalProps = IPGlobalProperties.GetIPGlobalProperties();

            return new NetworkDetails
            {
                DHCPScope = ApplicationHelper.IsWindows ? globalProps.DhcpScopeName : string.Empty,
                Domain = globalProps.DomainName,
                Host = globalProps.HostName,
                IsWINSProxy = ApplicationHelper.IsWindows && globalProps.IsWinsProxy,
                NodeType = globalProps.NodeType.ToString(),
                InterfaceDetails = NetworkInterface.GetAllNetworkInterfaces()
                    .Select(nic =>
                    {
                        var macAsBytes = nic.GetPhysicalAddress().GetAddressBytes();
                        return new NetworkInterfaceDetails
                        {
                            Interface = nic,
                            Addresses = NetworkHelper.GetLocalIPAddresses(nic).Select(IPAddressDetails.From).ToArray(),
                            MAC = string.Join(":", macAsBytes.Select(b => b.ToString("X2")))
                        };
                    })
                    .ToArray()
            };
        }

        private static string GenerateImpl(DiagnosticReport report)
        {
            StringBuilder builder = StringBuilderCache.Acquire();

            foreach (var section in ReportSections)
            {
                if (IsReportEnabled(report.Type, section))
                {
                    SectionToFormattingMap[section](builder, report);
                }
            }

            builder.Insert(0, $"/{NewLine}{Pipe}Diagnostic Report generated at: " +
                              $"{report.Timestamp.ToString("dd-MM-yyyy HH:mm:ss.fff (zzzz)")} in: " +
                              $"{report.TimeTaken.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)} milliseconds.{NewLine}")
                .Append('\\');

            return StringBuilderCache.GetStringAndRelease(builder);
        }

        private static void AddSystem(StringBuilder builder, DiagnosticReport report)
        {
            int maxHeaderLength = SystemHeaders.Max(h => h.Length);
            string formatter = "{0,-" + (maxHeaderLength + 1) + "}";

            int sectionIndex = builder.Length;
            Format(SystemHeaders[0], report.SystemDetails.OSName);
            Format(SystemHeaders[1], report.SystemDetails.OSType);
            Format(SystemHeaders[2], report.SystemDetails.Is64BitOS.ToString());
            Format(SystemHeaders[3], report.SystemDetails.DotNetFrameworkVersion);
            Format(SystemHeaders[4], report.SystemDetails.MachineName);
            Format(SystemHeaders[5], report.SystemDetails.FQDN);
            Format(SystemHeaders[9], report.SystemDetails.User);
            Format(SystemHeaders[7], report.SystemDetails.CPU);
            Format(SystemHeaders[8], report.SystemDetails.CPUCoreCount.ToString());
            Format(SystemHeaders[6], report.SystemDetails.InstalledRAMInGigaBytes.ToString("N0") + "GB");
            Format(SystemHeaders[10], report.SystemDetails.SystemDirectory);
            Format(SystemHeaders[11], report.SystemDetails.CurrentDirectory);
            Format(SystemHeaders[12], report.SystemDetails.RuntimeDirectory);

            string upTimeStr = "-";
            if (report.SystemDetails.Uptime != TimeSpan.MinValue)
            {
                upTimeStr = report.SystemDetails.Uptime.ToString(@"dd\D\a\y\ hh\H\o\u\r\ mm\M\i\n\ ss\S\e\c");
            }

            Format(SystemHeaders[13], upTimeStr);

            int maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("System", maxLineLength));

            void Format(string key, object value) =>
                builder.Append(LinePrefix)
                    .Append(Dot)
                    .Append(Space)
                    .AppendFormat(formatter, key)
                    .Append(Colon)
                    .Append(Space)
                    .AppendLine(value?.ToString());
        }

        private static void AddProcess(StringBuilder builder, DiagnosticReport report)
        {
            int maxHeaderLength = ProcessHeaders.Max(h => h.Length);
            string formatter = "{0,-" + (maxHeaderLength + 1) + "}";

            int sectionIndex = builder.Length;
            Format(ProcessHeaders[0], report.ProcessDetails.PID.ToString());
            Format(ProcessHeaders[1], report.ProcessDetails.Name);
            Format(ProcessHeaders[2], report.ProcessDetails.Started.ToString("dd-MM-yyyy HH:mm:ss.fff (zzzz)"));
            Format(ProcessHeaders[3], report.ProcessDetails.LoadedIn.ToString());
            Format(ProcessHeaders[17], report.ProcessDetails.IsInteractive.ToString());
            Format(ProcessHeaders[4], report.ProcessDetails.IsOptimized);
            Format(ProcessHeaders[5], report.ProcessDetails.Is64Bit.ToString());
            Format(ProcessHeaders[19], report.ProcessDetails.IsServerGC.ToString());
            Format(ProcessHeaders[6], report.ProcessDetails.IsLargeAddressAware.ToString());
            Format(ProcessHeaders[16], report.ProcessDetails.WorkingSetInMegaBytes.ToString("N0") + "MB");
            Format(ProcessHeaders[20], report.ProcessDetails.ThreadCount.ToString("N0"));
            Format(ProcessHeaders[21], $"{report.ProcessDetails.ThreadPoolMinCompletionPortCount.ToString("N0")} <-> {report.ProcessDetails.ThreadPoolMaxCompletionPortCount.ToString("N0")}");
            Format(ProcessHeaders[22], $"{report.ProcessDetails.ThreadPoolMinWorkerCount.ToString("N0")} <-> {report.ProcessDetails.ThreadPoolMaxWorkerCount.ToString("N0")}");
            Format(ProcessHeaders[12], report.ProcessDetails.FileVersion);
            Format(ProcessHeaders[13], report.ProcessDetails.ProductVersion);
            Format(ProcessHeaders[14], report.ProcessDetails.Language);
            Format(ProcessHeaders[15], report.ProcessDetails.Copyright);
            Format(ProcessHeaders[10], report.ProcessDetails.OriginalFileName);
            Format(ProcessHeaders[11], report.ProcessDetails.FileName);
            Format(ProcessHeaders[7], report.ProcessDetails.ModuleName);
            Format(ProcessHeaders[8], report.ProcessDetails.ModuleFileName);
            Format(ProcessHeaders[9], report.ProcessDetails.ProductName);
            Format(ProcessHeaders[18], report.ProcessDetails.CommandLine[0]);

            string[] cmdArgs = report.ProcessDetails.CommandLine;
            for (int i = 1; i < cmdArgs.Length; i++)
            {
                builder.Append(LinePrefix)
                    .Append(Space).Append(Space)
                    .AppendFormat(formatter, string.Empty)
                    .Append(Space).Append(Space)
                    .AppendLine(cmdArgs[i]);
            }

            int maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Process", maxLineLength));

            void Format(string key, object value) =>
                builder.Append(LinePrefix)
                    .Append(Dot)
                    .Append(Space)
                    .AppendFormat(formatter, key)
                    .Append(Colon)
                    .Append(Space)
                    .AppendLine(value?.ToString());
        }

        private static void AddDrives(StringBuilder builder, DiagnosticReport report)
        {
            List<string[]> values = new();

            foreach (DriveDetails d in report.DriveDetails)
            {
                var row = new string[7];
                row[0] = d.Name;
                row[1] = d.Type;
                row[2] = d.Format;
                row[3] = d.Label;
                row[4] = d.TotalCapacityInGigaBytes.ToString("N0");
                row[5] = d.FreeCapacityInGigaBytes.ToString("N0");
                row[6] = d.AvailableCapacityInGigaBytes.ToString("N0");
                values.Add(row);
            }

            WrapInTable(builder, DrivesHeaders, values);
        }

        private static void AddAssemblies(StringBuilder builder, DiagnosticReport report)
        {
            int sectionIndex = builder.Length;

            int maxHeaderLength = AssemblyHeaders.Max(h => h.Length);

            string nameFormatter = "{0}{1:D3}{2} {3,-" + (maxHeaderLength + 1) + "}: {4}{5}";
            string formatter = "{0,-" + (maxHeaderLength + 1) + "}";

            int assCounter = 1;
            report.Assemblies
                .OrderByDescending(a => a.IsGAC)
                .ForEach(ass =>
                {
                    builder.AppendFormat(nameFormatter, LinePrefix, assCounter.ToString(), Pipe.ToString(), AssemblyHeaders[0], ass.FullName, NewLine);

                    Format(AssemblyHeaders[1], ass.FileName);
                    Format(AssemblyHeaders[2], ass.IsGAC.ToString());
                    Format(AssemblyHeaders[3], ass.Is64Bit.ToString());
                    Format(AssemblyHeaders[4], ass.IsOptimized.ToString());
                    Format(AssemblyHeaders[5], ass.Framework);
                    Format(AssemblyHeaders[6], ass.Location);
                    Format(AssemblyHeaders[7], ass.CodeBase);
                    Format(AssemblyHeaders[8], ass.Version);
                    Format(AssemblyHeaders[9], ass.FileVersion);
                    Format(AssemblyHeaders[10], ass.ProductVersion);
                    Format(AssemblyHeaders[11], ass.ProductName);
                    Format(AssemblyHeaders[12], ass.CompanyName);

                    if (assCounter != report.Assemblies.Length)
                    {
                        builder.Append(LinePrefix).AppendLine();
                    }

                    assCounter++;
                });

            int maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Assemblies", maxLineLength));

            void Format(string key, object value) =>
                builder.Append(LinePrefix)
                    .Append(Space).Append(Space).Append(Space)
                    .Append(Dot)
                    .Append(Space)
                    .AppendFormat(formatter, key)
                    .Append(Colon)
                    .Append(Space)
                    .AppendLine(value?.ToString());
        }

        private static void AddEnvironmentVariables(StringBuilder builder, DiagnosticReport report)
        {
            Dictionary<string, string> envKeyVals = report.EnvironmentVariables
                .OrderBy(kv => kv.Key)
                .ToDictionary(kv => kv.Key, kv => kv.Value);

            int sectionIndex = builder.Length;

            int envVarCounter = 1;
            int maxKeyLength = envKeyVals.Keys.Max(key => key.Length);
            string format = "{0}{1:D3}{2} {3,-" + maxKeyLength + "} : {4}{5}";
            envKeyVals.ForEach(kv =>
            {
                builder.AppendFormat(format, LinePrefix, envVarCounter.ToString(), Dot.ToString(), kv.Key, kv.Value, NewLine);
                envVarCounter++;
            });

            int maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Environment-Variables", maxLineLength));
        }

        private static void AddNetwork(StringBuilder builder, DiagnosticReport report)
        {
            NetworkDetails net = report.NetworkingDetails;

            int sectionIndex = builder.Length;

            builder.Append(LinePrefix)
                .Append(Dot).Append(Space)
                .AppendLine("Windows IP Configuration")
                .Append(LinePrefix)
                .Append(Space).Append(Space)
                .AppendLine("-------------------------");

            int maxKeyLength = NetworkHeaders.Max(h => h.Length);
            string format = "{0}  {1} {2,-" + maxKeyLength.ToString() + "} : {3}{4}";

            Format(NetworkHeaders[0], net.Host);
            Format(NetworkHeaders[1], net.Domain);
            Format(NetworkHeaders[2], net.DHCPScope);
            Format(NetworkHeaders[3], net.NodeType);
            Format(NetworkHeaders[4], net.IsWINSProxy.ToString());

            builder.Append(LinePrefix).AppendLine();

            foreach (NetworkInterfaceDetails item in net.InterfaceDetails)
            {
                builder.Append(LinePrefix)
                    .Append(Dot).Append(Space)
                    .AppendLine(item.Interface.Description)
                    .Append(LinePrefix)
                    .Append(Space).Append(Space)
                    .AppendLine(new string('-', item.Interface.Description.Length + 1));

                Format(NetworkHeaders[5], item.Interface.Name);
                Format(NetworkHeaders[6], item.MAC);
                Format(NetworkHeaders[7], item.Interface.NetworkInterfaceType);
                Format(NetworkHeaders[8], item.Interface.OperationalStatus);
                Format(NetworkHeaders[10], item.Interface.SupportsMulticast.ToString());

                if (ApplicationHelper.IsWindows)
                {
                    Format(NetworkHeaders[9], item.Interface.IsReceiveOnly.ToString());
                    Format(NetworkHeaders[11], (item.Interface.Speed / 1000000).ToString("N0") + " Mbit/s");
                }

                if (item.Addresses.Any())
                {
                    string ipAddresses = "[" + string.Join(" | ", item.Addresses.Select(i => i.ToString())) + "]";
                    Format(NetworkHeaders[12], ipAddresses);
                }

                builder.Append(LinePrefix).AppendLine();
            }

            int maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Networks", maxLineLength));

            void Format(string key, object value)
            {
                // ReSharper disable once RedundantToStringCall
                builder.AppendFormat(format, LinePrefix, Pipe.ToString(), key, value?.ToString(), NewLine);
            }
        }

        private static void WrapInTable(StringBuilder builder, IReadOnlyList<string> columnHeaders, IReadOnlyList<string[]> values)
        {
            foreach (string[] row in values)
            {
                if (row.Length != columnHeaders.Count)
                {
                    throw new InvalidDataException("There should be a corresponding data for every column header");
                }
            }

            // initialize cellLengths first based on length of the headers
            int[] cellLengths = new int[columnHeaders.Count];
            for (var i = 0; i < columnHeaders.Count; i++)
            {
                var headerLength = columnHeaders[i].Length;
                cellLengths[i] = headerLength;
            }

            foreach (string[] row in values)
            {
                for (int i = 0; i < columnHeaders.Count; i++)
                {
                    string cellVal = row[i];
                    if (cellVal.Length > cellLengths[i])
                    {
                        cellLengths[i] = cellVal.Length;
                    }
                }
            }

            for (int i = 0; i < cellLengths.Length; i++)
            {
                cellLengths[i] = cellLengths[i] + 2;
            }

            StringBuilder headerBuilder = StringBuilderCache.Acquire();

            // insert headers
            headerBuilder.Append(LinePrefix);
            for (int i = 0; i < columnHeaders.Count; i++)
            {
                string headerVal = columnHeaders[i];
                string formatter = "{0} {1,-" + (cellLengths[i] - 2).ToString() + "} ";
                headerBuilder.AppendFormat(formatter, Pipe.ToString(), headerVal);
            }
            headerBuilder.Append(Pipe).AppendLine();

            // insert headers underline
            headerBuilder.Append(LinePrefix);
            for (int i = 0; i < columnHeaders.Count; i++)
            {
                headerBuilder.Append(Pipe).Append(new string(Dash, cellLengths[i]));
            }

            int maxLineLengthInHeader = GetMaximumLineLength(headerBuilder);
            string beginAndEnd = $"{LinePrefix} {new string(Dash, maxLineLengthInHeader - LinePrefix.Length - 2)}{NewLine}";
            headerBuilder.Insert(0, beginAndEnd);

            int beginPos = builder.Length;

            // insert row values
            builder.Append(Pipe).AppendLine();
            foreach (string[] row in values)
            {
                builder.Append(LinePrefix);
                for (int j = 0; j < row.Length; j++)
                {
                    string formatter = "{0} {1,-" + (cellLengths[j] - 2).ToString() + "} ";
                    builder.AppendFormat(formatter, Pipe.ToString(), row[j]);
                }
                builder.Append(Pipe).AppendLine();
            }

            builder.Insert(beginPos, StringBuilderCache.GetStringAndRelease(headerBuilder));
            builder.Append(beginAndEnd);

            int maxLineLength = GetMaximumLineLength(builder, beginPos);
            builder.Insert(beginPos, GetSeperator("Drives", maxLineLength));
        }

        private static int GetMaximumLineLength(StringBuilder builder, int start = 0)
        {
            if (start >= builder.Length) { throw new IndexOutOfRangeException(); }

            int maxLength = 0, tmpLength = 0;
            char prevChar = '\0';

            for (int i = start; i < builder.Length; i++)
            {
                char currChar = builder[i];

                if (currChar == '\n')
                {
                    if (prevChar == '\r') { --tmpLength; }
                    if (maxLength < tmpLength) { maxLength = tmpLength; }
                    tmpLength = 0;
                }
                else { tmpLength++; }

                prevChar = currChar;
            }
            return maxLength;
        }

        private static bool IsReportEnabled(DiagnosticReportType requestedType, DiagnosticReportType section)
        {
            int indexOf = Array.IndexOf(ReportSections, section);
            return ((int)requestedType & (1 << indexOf)) != 0;
        }

        private static string GetProcessorName()
        {
            string result = "UNKNOWN";

            try
            {
                if (ApplicationHelper.IsWindows) { result = GetProcessorNameWindows(); }
                if (ApplicationHelper.IsLinux) { result = GetProcessorNameLinux(); }
                if (ApplicationHelper.IsOSX) { result = GetProcessorNameOSX(); }

                return Regex.Replace(result, @"\s+", " ");
            }
            catch (Exception) { return result; }
        }

        /// <summary>
        /// Returns the full CPU name using the registry. 
        /// See <see href="http://stackoverflow.com/questions/2376139/get-full-cpu-name-without-wmi"/>
        /// </summary>
        /// <returns>The CPU Name</returns>
        private static string GetProcessorNameWindows()
        {
#if NETCOREAPP || NETFRAMEWORK || NETSTANDARD2_0
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
            return key?.GetValue("ProcessorNameString").ToString() ?? "Not Found";
#else
            return "<INVALID>"; // This should be unreachable.
#endif
        }

        private static string GetProcessorNameLinux()
        {
            // ReSharper disable once InconsistentNaming
            const string CPUFile = "/proc/cpuinfo";
            string cpuLine = File.ReadLines(CPUFile)
                .FirstOrDefault(l => l.StartsWith("model name", StringComparison.InvariantCultureIgnoreCase));

            if (cpuLine is null) { return "<UNKNOWN>"; }

            const string Separator = ": ";
            int startIdx = cpuLine.IndexOf(Separator, StringComparison.Ordinal) + Separator.Length;
            return cpuLine.Substring(startIdx, cpuLine.Length - startIdx);
        }

        // ReSharper disable once InconsistentNaming
        private static string GetProcessorNameOSX() => AsBashCommand("sysctl -n machdep.cpu.brand_string").TrimEnd();

        private static long GetInstalledMemoryInGigaBytes()
        {
            try
            {
                if (ApplicationHelper.IsWindows) { return GetInstalledMemoryInGigaBytesWindows(); }
                if (ApplicationHelper.IsLinux) { return GetInstalledMemoryInGigaBytesLinux(); }
                if (ApplicationHelper.IsOSX) { return GetInstalledMemoryInGigaBytesOSX(); }

                return 0;
            }
            catch (Exception) { return 0; }
        }

        private static long GetInstalledMemoryInGigaBytesWindows()
        {
            GetPhysicallyInstalledSystemMemory(out var installedMemoryKb);
            return (long)UnitConverter.KiloBytesToMegaBytes(installedMemoryKb).MegaBytesToGigaBytes();
        }

        private static long GetInstalledMemoryInGigaBytesLinux()
        {
            const string MemFile = "/proc/meminfo";
            string memLine = File.ReadLines(MemFile)
                .FirstOrDefault(l => l.StartsWith("MemTotal:", StringComparison.InvariantCultureIgnoreCase));

            if (memLine is null) { return -1; }

            const string BeginSeparator = ":";
            const string EndSeparator = "kB";
            int startIdx = memLine.IndexOf(BeginSeparator, StringComparison.Ordinal) + BeginSeparator.Length;
            int endIdx = memLine.IndexOf(EndSeparator, StringComparison.Ordinal);
            string memStr = memLine.Substring(startIdx, endIdx - startIdx);
            return long.Parse(memStr) / 1000_000;
        }

        // ReSharper disable once InconsistentNaming
        private static long GetInstalledMemoryInGigaBytesOSX()
        {
            string memStr = AsBashCommand("sysctl -n hw.memsize");
            return long.Parse(memStr) / 1000_000_000;
        }

        private static TimeSpan GetUptime()
        {
            try
            {
                if (ApplicationHelper.IsWindows) { return GetUptimeWindows(); }
                if (ApplicationHelper.IsLinux) { return GetUptimeLinux(); }
                return TimeSpan.MinValue;
            }
            catch (Exception)
            {
                return TimeSpan.MinValue;
            }
        }

        private static TimeSpan GetUptimeWindows()
        {
            Stopwatch start = Stopwatch.StartNew();
            return TimeSpan.FromMilliseconds(GetTickCount64()).Subtract(start.Elapsed);
        }

        private static TimeSpan GetUptimeLinux()
        {
            const string UptimeFile = "/proc/uptime";
            string upTimeStr = File.ReadAllText(UptimeFile);

            int stopIdx = upTimeStr.IndexOf(Space);
            string upTimeSecStr = upTimeStr.Substring(0, stopIdx);
            double upTimeSec = double.Parse(upTimeSecStr);
            return TimeSpan.FromSeconds(upTimeSec);
        }

        private static string GetSeperator(string title, int count) =>
            $"{Pipe.ToString()}{NewLine}{Pipe.ToString()}{title}{Pipe.ToString()}{new string(Dot, count - title.Length)}{NewLine}{Pipe.ToString()}{NewLine}";

        private static string AsBashCommand(string command)
        {
            string escapedArgs = command.Replace("\"", "\\\"");
            using Process p = new()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            p.Start();
            var result = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return result;
        }

        private static string GetOSPlatform()
        {
            if (ApplicationHelper.OSPlatform == OSPlatform.Windows) { return IsWindowsServer() ? "Windows (Server)" : "Windows (Desktop)"; }
            if (ApplicationHelper.OSPlatform == OSPlatform.Linux) { return "Linux"; }
            if (ApplicationHelper.OSPlatform == OSPlatform.OSX) { return "OSX"; }

            return ApplicationHelper.OSPlatform.ToString();
        }

        private static bool IsWindowsServer() => IsOS(OS_ANYSERVER);

        private static string GetDotNetFrameworkVersion()
        {
#if NETFRAMEWORK
            return ".NET Framework " + GetVersionFull();

            // https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
            static Version GetVersionFull()
            {
                const string REG_KEY = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full";
                using Microsoft.Win32.RegistryKey ndpKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(REG_KEY);

                if (ndpKey != null)
                {
                    int value = (int)(ndpKey.GetValue("Release") ?? 0);
                    if (value >= 528040) { return new Version(4, 8, 0); }
                    if (value >= 461808) { return new Version(4, 7, 2); }
                    if (value >= 461308) { return new Version(4, 7, 1); }
                    if (value >= 460798) { return new Version(4, 7, 0); }
                    if (value >= 394802) { return new Version(4, 6, 2); }
                    if (value >= 394254) { return new Version(4, 6, 1); }
                    if (value >= 393295) { return new Version(4, 6, 0); }
                    if (value >= 379893) { return new Version(4, 5, 2); }
                    if (value >= 378675) { return new Version(4, 5, 1); }
                    if (value >= 378389) { return new Version(4, 5, 0); }

                    throw new NotSupportedException("Unable to detect a DotNet framework version of 4.5 or later, framework key value found: " + value.ToString());
                }

                throw new NotSupportedException($"Unable to find a registry key under '{REG_KEY}'.");
            }
#else
            try
            {
                return ".NET Core " + GetVersionCore();
            } catch (NotSupportedException)
            {
                return RuntimeInformation.FrameworkDescription + " (Self Contained)";
            }

            Version GetVersionCore()
            {
                const string REGEX_PATTERN = @"Microsoft\.NETCore\.App[\\,/](?<version>\d+\.\d+.\d+(.\d+)?)$";
                
                string runtimePath = Path.GetDirectoryName(typeof(object).Assembly.Location);

                Match match = Regex.Match(runtimePath, REGEX_PATTERN);

                if (!match.Success)
                {
                    throw new NotSupportedException("Unable to detect a DotNet Core version, runtime path found: " + runtimePath);
                }

                return new Version(match.Groups["version"].Value);
            }
#endif
        }
        /// <summary>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/cc300158(v=vs.85).aspx"/>
        /// </summary>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);

        [DllImport("kernel32")]
        private static extern ulong GetTickCount64();

        [DllImport("shlwapi.dll", SetLastError = true, EntryPoint = "#437")]
        private static extern bool IsOS(int os);

        private const int OS_ANYSERVER = 29;
    }
}