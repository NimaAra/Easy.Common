namespace Easy.Common
{
    /// <summary>
    /// A set of methods to convert between different units.
    /// </summary>
    public static class UnitConverter
    {
        /// <summary>
        /// Converts Bytes to Megabytes.
        /// </summary>
        public static double BytesToMegabytes(this double bytes)
        {
            return bytes / 1024f / 1024f;
        }

        /// <summary>
        /// Converts Bytes to Gigabytes.
        /// </summary>
        public static double BytesToGigabytes(this double bytes)
        {
            return (bytes / 1024f / 1024f) / 1024f;
        }

        /// <summary>
        /// Converts Kilobytes to Megabytes.
        /// </summary>
        public static double KilobytesToMegabytes(this double kilobytes)
        {
            return kilobytes / 1024f;
        }

        /// <summary>
        /// Converts Megabytes to Gigabytes.
        /// </summary>
        public static double MegabytesToGigabytes(this double megabytes)
        {
            return megabytes / 1024f;
        }

        /// <summary>
        /// Converts Megabytes to Terabytes.
        /// </summary>
        public static double MegabytesToTerabytes(this double megabytes)
        {
            return megabytes / (1024f * 1024f);
        }

        /// <summary>
        /// Converts Gigabytes to Megabytes.
        /// </summary>
        public static double GigabytesToMegabytes(this double gigabytes)
        {
            return gigabytes * 1024f;
        }

        /// <summary>
        /// Converts Gigabytes to Terabytes.
        /// </summary>
        public static double GigabytesToTerabytes(this double gigabytes)
        {
            return gigabytes / 1024f;
        }

        /// <summary>
        /// Converts Terabytes to Megabytes.
        /// </summary>
        public static double TerabytesToMegabytes(this double terabytes)
        {
            return terabytes * (1024f * 1024f);
        }

        /// <summary>
        /// Converts Terabytes to Gigabytes.
        /// </summary>
        public static double TerabytesToGigabytes(this double terabytes)
        {
            return terabytes * 1024f;
        }
    }
}