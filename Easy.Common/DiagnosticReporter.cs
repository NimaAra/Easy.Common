namespace Easy.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32;
    using Easy.Common.Extensions;

    /// <summary>
    /// A helper class for generating a report containing details related to 
    /// <c>System</c>, <c>Process</c>, <c>Assemblies</c> and <c>Environment</c> 
    /// on which the application executes.
    /// </summary>
    public static class DiagnosticReporter
    {
        private const char Pipe = '|';
        private const char Dot = '.';
        private const char Dash = '-';
        private const char Space = ' ';
        private const char Colon = ':';
        private static readonly string NewLine = Environment.NewLine;
        private static readonly string LinePrefix = Pipe + "\t";

        /// <summary>
        /// Returns the details related to <c>System</c>, <c>Process</c>, <c>Assemblies</c>
        /// and <c>Environment</c> on which the application executes.
        /// </summary>
        public static string Generate()
        {
            try { return GenerateImpl(); } 
            catch (Exception e) { return "Unable to generate the Diagnostic Report. Error:\r\n\t" + e; }
        }

        private static string GenerateImpl()
        {
            var sw = Stopwatch.StartNew();
            
            var builder = new StringBuilder();
            
            AddSystem(builder);
            AddProcess(builder);
            AddDrives(builder);
            AddAssemblies(builder);
            AddEnvironmentVariables(builder);
            AddNetworking(builder);

            sw.Stop();

            builder.Insert(0, $"/{NewLine}{Pipe}Diagnostic Report generated at: {DateTime.Now:dd-MM-yyyy HH:mm:ss.fff} in: {sw.Elapsed.TotalMilliseconds} milliseconds.{NewLine}");
            builder.Append('\\');
            return builder.ToString();
        }

        private static void AddSystem(StringBuilder builder)
        {
            var headers = new[]
            {
                "OS Version", 
                "64Bit OS", 
                "Runtime", 
                "FQDN", 
                "Machine Name", 
                "Installed RAM", 
                "Processor",
                "Processor Count",
                "User",
                "System Directory",
                "Current Directory"
            };

            var maxHeaderLength = headers.Max(h => h.Length);
            var formatter = "{0,-" + (maxHeaderLength + 1) + "}";

            var sectionIndex = builder.Length;
            Format(headers[8], Environment.UserDomainName + "\\" + Environment.UserName);
            Format(headers[0], Environment.OSVersion);
            Format(headers[1], Environment.Is64BitOperatingSystem);
            Format(headers[2], Environment.Version);
            Format(headers[4], Environment.MachineName);
            Format(headers[3], NetworkHelper.GetFQDN());
            Format(headers[6], GetProcessorName());
            Format(headers[7], Environment.ProcessorCount);
            Format(headers[5], GetInstalledMemoryInGigaBytes().ToString("N0") + "GB");
            Format(headers[9], Environment.SystemDirectory);
            Format(headers[10], Environment.CurrentDirectory);

            var maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("System", maxLineLength));

            void Format(string key, object value)
            {
                builder
                    .Append(LinePrefix)
                    .Append(Dot)
                    .Append(Space)
                    .AppendFormat(formatter, key)
                    .Append(Colon)
                    .Append(Space)
                    .AppendLine(value.ToString());
            }
        }

        private static void AddDrives(StringBuilder builder)
        {
            var headers = new[] { "Name", "Type", "Format", "Label", "Capacity(GB)", "Free(GB)", "Available(GB)" };
            var values = new List<string[]>();

            foreach (var d in DriveInfo.GetDrives())
            {
                var row = new string[7];
                row[0] = d.Name;
                row[1] = d.DriveType.ToString();

                var dashString = Dash.ToString();
                string driveFormat = dashString, volumeLabel = dashString;
                double capacity = 0, free = 0, available = 0;

                if (d.IsReady)
                {
                    driveFormat = d.DriveFormat;
                    volumeLabel = d.VolumeLabel;
                    capacity = UnitConverter.BytesToGigaBytes(d.TotalSize);
                    free = UnitConverter.BytesToGigaBytes(d.TotalFreeSpace);
                    available = UnitConverter.BytesToGigaBytes(d.AvailableFreeSpace);
                }

                row[2] = driveFormat;
                row[3] = volumeLabel;
                row[4] = capacity.ToString("N0");
                row[5] = free.ToString("N0");
                row[6] = available.ToString("N0");

                values.Add(row);
            }

            WrapInTable(builder, headers, values);
        }

        private static void AddNetworking(StringBuilder builder)
        {
            var result = ProcessHelper.ExecuteAsync("ipconfig", "/all").Result;

            var sectionIndex = builder.Length;
            foreach (var entry in result.StandardOutput)
            {
                var entryLength = entry.Length;
                if (entryLength == 0) { continue; }

                builder.Append(LinePrefix);

                var firstChar = entry[0];
                if (firstChar != Space)
                {
                    // Add extra line between the new title and the previous 
                    // lines only if it's not the first title
                    if (!entry.Equals("Windows IP Configuration", StringComparison.Ordinal))
                    {
                        builder.AppendLine().Append(LinePrefix);
                    }

                    if (entry[entryLength - 1] == Colon) { entryLength = entryLength - 1; }

                    builder.Append(Dot).Append(Space);
                    for (var k = 0; k < entryLength; k++) { builder.Append(entry[k]); }

                    builder.AppendLine().Append(LinePrefix);

                    builder.Append(Space).Append(Space);
                    for (var j = 0; j < entryLength; j++) { builder.Append(Dash); }
                } else
                {
                    builder.Append(Space).Append(Space);

                    if (entryLength > 4)
                    {
                        if (entry[3] == Space)
                        {
                            builder.Append(Space).Append(Space);
                        } else
                        {
                            builder.Append(Dot).Append(Space);
                        }

                        for (var i = 3; i < entryLength; i++)
                        {
                            builder.Append(entry[i]);
                        }
                        
                    } else
                    {
                        builder.Append("[Invalid Data]");
                    }
                }

                builder.AppendLine();
            }

            var maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Networking", maxLineLength));
        }

        private static void AddProcess(StringBuilder builder)
        {
            var headers = new[]
            {
                "Id",
                "Name",
                "Started",
                "Loaded In",
                "Debug Enabled",
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
                "CommandLine"
            };

            var maxHeaderLength = headers.Max(h => h.Length);
            var formatter = "{0,-" +(maxHeaderLength + 1) + "}";

            var sectionIndex = builder.Length;
            using (var p = Process.GetCurrentProcess())
            {
                var pVerInfo = p.MainModule.FileVersionInfo;
                Format(headers[0], p.Id);
                Format(headers[1], p.ProcessName);
                Format(headers[2], p.StartTime.ToString("dd-MM-yyyy HH:mm:ss.fff"));
                Format(headers[3], ApplicationHelper.GetProcessStartupDuration());
                Format(headers[17], Environment.UserInteractive);
                Format(headers[4], ApplicationHelper.IsDebugBuild);
                Format(headers[5], Environment.Is64BitProcess);
                Format(headers[6], ApplicationHelper.IsProcessLargeAddressAware());
                Format(headers[16], UnitConverter.BytesToMegaBytes(Environment.WorkingSet).ToString("N0") + "MB");
                Format(headers[12], pVerInfo.FileVersion);
                Format(headers[13], pVerInfo.ProductVersion);
                Format(headers[14], pVerInfo.Language);
                Format(headers[15], pVerInfo.LegalCopyright);
                Format(headers[10], pVerInfo.OriginalFilename);
                Format(headers[11], pVerInfo.FileName);
                Format(headers[7], p.MainModule.ModuleName);
                Format(headers[8], p.MainModule.FileName);
                Format(headers[9], pVerInfo.ProductName);

                var cmdArgs = Environment.GetCommandLineArgs();
                Format(headers[18], cmdArgs[0]);

                for (var i = 1; i < cmdArgs.Length; i++)
                {
                    builder
                        .Append(LinePrefix)
                        .Append(Space).Append(Space)
                        .AppendFormat(formatter, string.Empty)
                        .Append(Space).Append(Space)
                        .AppendLine(cmdArgs[i]);
                }

                var maxLineLength = GetMaximumLineLength(builder, sectionIndex);
                builder.Insert(sectionIndex, GetSeperator("Process", maxLineLength));

                void Format(string key, object value)
                {
                    builder
                        .Append(LinePrefix)
                        .Append(Dot)
                        .Append(Space)
                        .AppendFormat(formatter, key)
                        .Append(Colon)
                        .Append(Space)
                        .AppendLine(value.ToString());
                }
            }
        }

        private static void AddAssemblies(StringBuilder builder)
        {
            var sectionIndex = builder.Length;

            var assHeaders = new[] {"Name", "GAC", "64Bit", "Optimized", "Framework", "Location", "CodeBase"};
            var maxHeaderLength = assHeaders.Max(h => h.Length);

            var nameFormatter = "{0}{1:D2}{2} {3,-" + (maxHeaderLength + 1) + "}: {4}{5}";
            var formatter = "{0,-" + (maxHeaderLength + 1) + "}";

            var assCounter = 1;
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(ass => !ass.IsDynamic)
                .OrderByDescending(o => o.GlobalAssemblyCache)
                .ForEach(x =>
                {
                    builder.AppendFormat(
                        nameFormatter,
                        LinePrefix, assCounter, Pipe, assHeaders[0], x.FullName, NewLine);

                    Format(assHeaders[1], x.GlobalAssemblyCache);
                    Format(assHeaders[2], !x.Is32Bit());
                    Format(assHeaders[3], x.IsOptimized());
                    Format(assHeaders[4], x.GetFrameworkVersion());
                    Format(assHeaders[5], x.Location);
                    Format(assHeaders[6], x.CodeBase);
                    assCounter++;
                });

            var maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Assemblies", maxLineLength));

            void Format(string key, object value)
            {
                builder
                    .Append(LinePrefix)
                    .Append(Space).Append(Space)
                    .Append(Dot)
                    .Append(Space)
                    .AppendFormat(formatter, key)
                    .Append(Colon)
                    .Append(Space)
                    .AppendLine(value.ToString());
            }
        }

        private static void AddEnvironmentVariables(StringBuilder builder)
        {
            var envKeyVals = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .OrderBy(kv => kv.Key)
                .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());

            var sectionIndex = builder.Length;

            var envVarCounter = 1;
            var maxKeyLength = envKeyVals.Keys.Max(key => key.Length);
            var format = "{0}{1:D2}{2} {3,-" + maxKeyLength + "} : {4}{5}";
            envKeyVals.ForEach(kv =>
            {
                builder.AppendFormat(format, LinePrefix, envVarCounter, Dot, kv.Key, kv.Value, NewLine);
                envVarCounter++;
            });

            var maxLineLength = GetMaximumLineLength(builder, sectionIndex);
            builder.Insert(sectionIndex, GetSeperator("Environment-Variables", maxLineLength));
        }

        private static void WrapInTable(
            StringBuilder builder, string[] columnHeaders, List<string[]> values)
        {
            foreach (var row in values)
            {
                if (row.Length != columnHeaders.Length)
                {
                    throw new InvalidDataException("There should be a corresponding data for every column header");
                }
            }

            // init cellLengths first based on length of the headers
            var cellLengths = new int[columnHeaders.Length];
            for (var i = 0; i < columnHeaders.Length; i++)
            {
                var headerLength = columnHeaders[i].Length;
                cellLengths[i] = headerLength;
            }

            foreach (var row in values) 
            {
                for (var i = 0; i < columnHeaders.Length; i++)
                {
                    var cellVal = row[i];
                    if (cellVal.Length > cellLengths[i])
                    {
                        cellLengths[i] = cellVal.Length;
                    }
                }
            }

            for (var i = 0; i < cellLengths.Length; i++)
            {
                cellLengths[i] = cellLengths[i] + 2;
            }

            var headerBuilder = new StringBuilder();

            // insert headers
            headerBuilder.Append(LinePrefix);
            for (var i = 0; i < columnHeaders.Length; i++)
            {
                var headerVal = columnHeaders[i];
                var formatter = "{0} {1,-" + (cellLengths[i] - 2) + "} ";
                headerBuilder.AppendFormat(formatter, Pipe, headerVal);
            }
            headerBuilder.Append(Pipe).AppendLine();

            // insert headers underline
            headerBuilder.Append(LinePrefix);
            for (var i = 0; i < columnHeaders.Length; i++)
            {
                headerBuilder.Append(Pipe).Append(new string(Dash, cellLengths[i]));
            }
            
            var maxLineLengthInHeader = GetMaximumLineLength(headerBuilder);
            var beginAndEnd = $"{LinePrefix} {new string(Dash, maxLineLengthInHeader - LinePrefix.Length - 2)}{NewLine}";
            headerBuilder.Insert(0, beginAndEnd);

            var beginPos = builder.Length;

            // insert row values
            builder.Append(Pipe).AppendLine();
            foreach (var row in values)
            {
                builder.Append(LinePrefix);
                for (var j = 0; j < row.Length; j++)
                {
                    var formatter = "{0} {1,-" + (cellLengths[j] - 2) + "} ";
                    builder.AppendFormat(formatter, Pipe, row[j]);
                }
                builder.Append(Pipe).AppendLine();
            }

            builder.Insert(beginPos, headerBuilder.ToString());
            builder.Append(beginAndEnd);

            var maxLineLength = GetMaximumLineLength(builder, beginPos);
            builder.Insert(beginPos, GetSeperator("Drives", maxLineLength));
        }

        private static int GetMaximumLineLength(StringBuilder builder, int start = 0)
        {
            if (start >= builder.Length) { throw new IndexOutOfRangeException(); }

            int maxLength = 0, tmpLength = 0;
            var prevChar = '\0';

            for (var i = start; i < builder.Length; i++)
            {
                var currChar = builder[i];

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

        /// <summary>
        /// Returns the full CPU name using the registry. 
        /// See <see href="http://stackoverflow.com/questions/2376139/get-full-cpu-name-without-wmi"/>
        /// </summary>
        /// <returns>The CPU Name</returns>
        private static string GetProcessorName()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
            if (key == null)
            { return "Not Found"; }
            return key.GetValue("ProcessorNameString").ToString();
        }

        private static long GetInstalledMemoryInGigaBytes()
        {
            GetPhysicallyInstalledSystemMemory(out var installedMemoryKb);
            return (long)UnitConverter.KiloBytesToMegaBytes(installedMemoryKb).MegaBytesToGigaBytes();
        }

        private static string GetSeperator(string title, int count) 
            => $"{Pipe}{NewLine}{Pipe}{title}{Pipe}{new string(Dot, count - title.Length)}{NewLine}{Pipe}{NewLine}";

        /// <summary>
        /// <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/cc300158(v=vs.85).aspx"/>
        /// </summary>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);
    }
}