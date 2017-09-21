namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Easy.Common.Extensions;
    using Microsoft.Win32;

    /// <summary>
    /// A set of helpful methods
    /// </summary>
    public static class ApplicationHelper
    {
        /// <summary>
        /// Gets the flag indicating whether this application has been compiled in <c>DEBUG</c>.
        /// </summary>
        public static bool IsDebugBuild =>
#if DEBUG 
        true;
#else
        false;
#endif

        /// <summary>
        /// Returns the time taken to start the current process.
        /// </summary>
        public static TimeSpan GetProcessStartupDuration() => 
            DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);

        /// <summary>
        /// Returns the details related to <c>System</c>, <c>Process</c>, <c>Assemblies</c>
        /// and <c>Environment</c> on which the application executes.
        /// </summary>
        public static string GetDiagnosticReport()
        {
            var details = new StringBuilder("Diagnostic Report:");
            details.AppendLine();

            details.AppendLine(@"/------------------------------System-----------------------------\");
            new[]
                {
                    $"OS Version: {Environment.OSVersion}",
                    $"64Bit OS: {Environment.Is64BitOperatingSystem}",
                    $"Runtime: {Environment.Version}",
                    $"FQDN: {NetworkHelper.GetFQDN()}",
                    $"Machine Name: {Environment.MachineName}",
                    $"Installed RAM: {GetInstalledMemoryInMegaBytes():N0} MB",
                    $"Processor: {GetProcessorName()}",
                    $"Processor Count: {Environment.ProcessorCount}",
                    $"Running as: {Environment.UserDomainName}\\{Environment.UserName}",
                    $"Current Directory: {Environment.CurrentDirectory}"
                }
                .ForEach(x => details.AppendFormat("\t{0}{1}", x, Environment.NewLine));
            details.AppendLine(@"\------------------------------System-----------------------------/");

            details.AppendLine(@"/-----------------------------Process-----------------------------\");
            using (var p = Process.GetCurrentProcess())
            {
                var pVerInfo = p.MainModule.FileVersionInfo;
                new[]
                {
                    $"Id: {p.Id}",
                    $"Name: {p.ProcessName}",
                    $"Started: {p.StartTime:yyyy-MM-dd HH:mm:ss.fff}",
                    $"Loaded In: {GetProcessStartupDuration()}",
                    $"Debug Enabled: {IsDebugBuild}",
                    $"64Bit Process: {Environment.Is64BitProcess}",
                    $"Large Address Aware: {IsProcessLargeAddressAware()}",
                    $"Module Name: {p.MainModule.ModuleName}",
                    $"Module File Name: {p.MainModule.FileName}",
                    $"Product Name: {pVerInfo.ProductName}",
                    $"Original File Name: {pVerInfo.OriginalFilename}",
                    $"File Name: {pVerInfo.FileName}",
                    $"File Version: {pVerInfo.FileVersion}",
                    $"Product Version: {pVerInfo.ProductVersion}",
                    $"Language: {pVerInfo.Language}",
                    $"Copyright: {pVerInfo.LegalCopyright}"
                }
                .ForEach(x => details.AppendFormat("\t{0}{1}", x, Environment.NewLine));
            }
            details.AppendLine(@"\-----------------------------Process-----------------------------/");

            details.AppendLine(@"/---------------------------Assemblies----------------------------\");
            var counter = 1;
            AppDomain.CurrentDomain.GetAssemblies()
                     .Where(ass => !ass.IsDynamic)
                     .OrderByDescending(o => o.GlobalAssemblyCache)
                     .ForEach(x =>
                     {
                         details.AppendFormat("\t{0:D2}. Name: {1}{2}", counter, x.FullName, Environment.NewLine);
                         details.AppendFormat("\t\t| {0} - Is GAC: {1}{2}", x.GetFrameworkVersion(), x.GlobalAssemblyCache.ToString(), Environment.NewLine);
                         details.AppendFormat("\t\t| Is64Bit: {0}{1}", !x.Is32Bit(), Environment.NewLine);
                         details.AppendFormat("\t\t| Optimized: {0}{1}", x.IsOptimized(), Environment.NewLine);
                         details.AppendFormat("\t\t| Location: {0}{1}", x.Location, Environment.NewLine);
                         details.AppendFormat("\t\t| CodeBase: {0}{1}", x.CodeBase, Environment.NewLine);
                         counter++;
                     });
            details.AppendLine(@"\---------------------------Assemblies----------------------------/");
            
            details.AppendLine(@"/----------------------Environment Variables----------------------\");
            Environment.GetEnvironmentVariables()
                       .Cast<DictionaryEntry>()
                       .ForEach(kv => details.AppendFormat("\t{0} = {1}{2}", kv.Key, kv.Value, Environment.NewLine));
            details.AppendLine(@"\----------------------Environment Variables----------------------/");

            details.AppendLine(@"/--------------------------System Drives--------------------------\");
            details.AppendFormat("\t{0}{1}", string.Join(" ", Environment.GetLogicalDrives()), Environment.NewLine);
            details.AppendLine(@"\--------------------------System Drives--------------------------/");

            return details.ToString();
        }

        /// <summary>
        /// Queries the process's headers to find if it is <c>LARGEADDRESSAWARE</c>.
        /// <remarks>The method is equivalent to running <c>DumpBin</c> on the executable.</remarks>
        /// </summary>
        public static bool IsProcessLargeAddressAware()
        {
            using (var p = Process.GetCurrentProcess())
            {
                return IsLargeAddressAware(p.MainModule.FileName);
            }
        }

        /// <summary>
        /// <see href="https://helloacm.com/large-address-aware/"/>
        /// </summary>
        internal static bool IsLargeAddressAware(string file)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(file);
            var fileInfo = new FileInfo(file);
            Ensure.Exists(fileInfo);

            const int ImageFileLargeAddressAware = 0x20;

            using (var stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new BinaryReader(stream))
            {
                //No MZ Header
                if (reader.ReadInt16() != 0x5A4D) { return false; }

                reader.BaseStream.Position = 0x3C;
                var peloc = reader.ReadInt32(); //Get the PE header location.

                reader.BaseStream.Position = peloc;
                
                //No PE header
                if (reader.ReadInt32() != 0x4550) { return false; }

                reader.BaseStream.Position += 0x12;
                return (reader.ReadInt16() & ImageFileLargeAddressAware) == ImageFileLargeAddressAware;
            }
        }

        /// <summary>
        /// Returns the full CPU name using the registry. 
        /// See <see href="http://stackoverflow.com/questions/2376139/get-full-cpu-name-without-wmi"/>
        /// </summary>
        /// <returns>The CPU Name</returns>
        private static string GetProcessorName()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
            if (key == null) { return "Not Found"; }
            return key.GetValue("ProcessorNameString").ToString();
        }

        private static long GetInstalledMemoryInMegaBytes()
        {
            GetPhysicallyInstalledSystemMemory(out var installedMemoryKb);
            return installedMemoryKb / 1024;
        }

        /// <summary>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/cc300158(v=vs.85).aspx"/>
        /// </summary>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);
    }
}