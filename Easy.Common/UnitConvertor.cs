namespace Easy.Common
{
    using System;

    /// <summary>
    /// A set of methods to convert between different units.
    /// </summary>
    public static class UnitConverter
    {
        private const double KiloBytes = 1024;
        private const double MegaBytes = KiloBytes * KiloBytes;
        private const double GigaBytes = KiloBytes * MegaBytes;

        /// <summary>
        /// Returns a more human friendly textual representation of the given <paramref name="bytes"/>.
        /// </summary>
        public static string Humanize(double bytes)
        {
            var abs = Math.Abs(bytes);

            if (abs >= GigaBytes)
            {
                return $"{bytes / GigaBytes:#,#.##} GBytes";
            }

            if (abs >= MegaBytes)
            {
                return $"{bytes / MegaBytes:#,#.##} MBytes";
            }

            if (abs >= KiloBytes)
            {
                return $"{bytes / KiloBytes:#,#.##} KBytes";
            }

            return $"{bytes:#,#} Bytes";
        }

        /// <summary>
        /// Converts Bytes to Megabytes.
        /// </summary>
        public static double BytesToMegaBytes(this double bytes)
        {
            return bytes / MegaBytes;
        }

        /// <summary>
        /// Converts Bytes to Gigabytes.
        /// </summary>
        public static double BytesToGigaBytes(this double bytes)
        {
            return bytes / MegaBytes / KiloBytes;
        }

        /// <summary>
        /// Converts Kilobytes to Megabytes.
        /// </summary>
        public static double KiloBytesToMegaBytes(this double kiloBytes)
        {
            return kiloBytes / KiloBytes;
        }

        /// <summary>
        /// Converts Megabytes to Gigabytes.
        /// </summary>
        public static double MegaBytesToGigaBytes(this double megaBytes)
        {
            return megaBytes / KiloBytes;
        }

        /// <summary>
        /// Converts Megabytes to Terabytes.
        /// </summary>
        public static double MegaBytesToTeraBytes(this double megaBytes)
        {
            return megaBytes / MegaBytes;
        }

        /// <summary>
        /// Converts Gigabytes to Megabytes.
        /// </summary>
        public static double GigaBytesToMegaBytes(this double gigaBytes)
        {
            return gigaBytes * KiloBytes;
        }

        /// <summary>
        /// Converts Gigabytes to Terabytes.
        /// </summary>
        public static double GigaBytesToTeraBytes(this double gigaBytes)
        {
            return gigaBytes / KiloBytes;
        }

        /// <summary>
        /// Converts Terabytes to Megabytes.
        /// </summary>
        public static double TeraBytesToMegaBytes(this double teraBytes)
        {
            return teraBytes * MegaBytes;
        }

        /// <summary>
        /// Converts Terabytes to Gigabytes.
        /// </summary>
        public static double TeraBytesToGigaBytes(this double teraBytes)
        {
            return teraBytes * KiloBytes;
        }
    }
}