namespace Easy.Common
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// A set of helpful methods
    /// </summary>
    public static class ApplicationHelper
    {
        /// <summary>
        /// Returns the time taken to start the current process.
        /// </summary>
        public static TimeSpan GetProcessStartupDuration() => 
            DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);

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
    }
}