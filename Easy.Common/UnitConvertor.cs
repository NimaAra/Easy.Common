namespace Easy.Common;

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
        var abs = System.Math.Abs(bytes);

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
    /// Converts <c>Bytes</c> to <c>Megabytes</c>.
    /// </summary>
    public static double BytesToMegaBytes(this double bytes) => bytes / MegaBytes;

    /// <summary>
    /// Converts <c>Bytes</c> to <c>Gigabytes</c>.
    /// </summary>
    public static double BytesToGigaBytes(this double bytes) => bytes / MegaBytes / KiloBytes;

    /// <summary>
    /// Converts <c>Kilobytes</c> to <c>Megabytes</c>.
    /// </summary>
    public static double KiloBytesToMegaBytes(this double kiloBytes) => kiloBytes / KiloBytes;

    /// <summary>
    /// Converts <c>Megabytes</c> to <c>Gigabytes</c>.
    /// </summary>
    public static double MegaBytesToGigaBytes(this double megaBytes) => megaBytes / KiloBytes;

    /// <summary>
    /// Converts <c>Megabytes</c> to <c>Terabytes</c>.
    /// </summary>
    public static double MegaBytesToTeraBytes(this double megaBytes) => megaBytes / MegaBytes;

    /// <summary>
    /// Converts <c>Gigabytes</c> to <c>Megabytes</c>.
    /// </summary>
    public static double GigaBytesToMegaBytes(this double gigaBytes) => gigaBytes * KiloBytes;

    /// <summary>
    /// Converts <c>Gigabytes</c> to <c>Terabytes</c>.
    /// </summary>
    public static double GigaBytesToTeraBytes(this double gigaBytes) => gigaBytes / KiloBytes;

    /// <summary>
    /// Converts <c>Terabytes</c> to <c>Megabytes</c>.
    /// </summary>
    public static double TeraBytesToMegaBytes(this double teraBytes) => teraBytes * MegaBytes;

    /// <summary>
    /// Converts <c>Terabytes</c> to <c>Gigabytes</c>.
    /// </summary>
    public static double TeraBytesToGigaBytes(this double teraBytes) => teraBytes * KiloBytes;
}