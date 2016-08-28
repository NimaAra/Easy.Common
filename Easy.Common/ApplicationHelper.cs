namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Linq;
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
        public static bool IsDebugBuild { get; } =
#if DEBUG 
        true;
#else
        false;
#endif

        /// <summary>
        /// Returns the time taken to start the current process.
        /// </summary>
        public static TimeSpan GetProcessStartupDuration()
        {
            return DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);
        }

        /// <summary>
        /// Returns the system details on which the application executes.
        /// </summary>
        /// <returns>The system details</returns>
        public static string GetSystemDetails()
        {
            var details = new StringBuilder("Process, Environment & System Details:");
            details.AppendLine();
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
                    $"Module Name: {p.MainModule.ModuleName}",
                    $"Module File Name: {p.MainModule.FileName}",
                    $"Product Name: {pVerInfo.ProductName}",
                    $"Original File Name: {pVerInfo.OriginalFilename}",
                    $"File Name: {pVerInfo.FileName}",
                    $"File Version: {pVerInfo.FileVersion}",
                    $"Product Version: {pVerInfo.ProductVersion}",
                    $"Language: {pVerInfo.Language}",
                    $"Copyright: {pVerInfo.LegalCopyright}"
                }.ForEach(x => details.AppendFormat("\t{0}{1}", x, Environment.NewLine));
            }
            details.AppendLine(@"\-----------------------------Process-----------------------------/");

            details.AppendLine(@"/---------------------------Assemblies----------------------------\");
            var counter = 1;
            AppDomain.CurrentDomain.GetAssemblies()
                     .Where(ass => !ass.IsDynamic)
                     .OrderByDescending(o => o.GlobalAssemblyCache)
                     .ForEach(x =>
                     {
                         details.AppendFormat("\t{0:D2}.Name: {1}{2}", counter, x.FullName, Environment.NewLine);
                         details.AppendFormat("\t{0:D2}.Optimized: {1}{2}", counter, x.IsOptimized(), Environment.NewLine);
                         details.AppendFormat("\t\t {0} - Is GAC: {1}{2}", x.GetFrameworkVersion(), x.GlobalAssemblyCache, Environment.NewLine);
                         details.AppendFormat("\t\t Location: {0}{1}", x.Location, Environment.NewLine);
                         counter++;
                     });
            details.AppendLine(@"\---------------------------Assemblies----------------------------/");

            details.AppendLine(@"/---------------------------Environment---------------------------\");
            new[]
            {
                $"OS Version: {Environment.OSVersion}",
                $"64Bit OS: {Environment.Is64BitOperatingSystem}",
                $"64Bit Process: {Environment.Is64BitProcess}",
                $"Runtime: {Environment.Version}",
                $"FQDN: {NetworkHelper.GetFQDN()}",
                $"Machine Name: {Environment.MachineName}",
                $"Processor: {GetProcessorName()}",
                $"Processor Count: {Environment.ProcessorCount}",
                $"Running as: {Environment.UserDomainName}\\{Environment.UserName}",
                $"Current Directory: {Environment.CurrentDirectory}"
            }
            .ForEach(x => details.AppendFormat("\t{0}{1}", x, Environment.NewLine));
            details.AppendLine(@"\---------------------------Environment---------------------------/");

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
    }
}