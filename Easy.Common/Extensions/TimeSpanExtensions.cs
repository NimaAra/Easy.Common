namespace Easy.Common.Extensions;

using System;

/// <summary>
/// Provides a set of helper methods on <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Returns a more human friendly textual representation of the given <paramref name="span"/>.
    /// </summary>
    public static string Humanize(this TimeSpan span)
    {
        if (span == TimeSpan.Zero)
        {
            return "0";
        }

        if (span == TimeSpan.MinValue)
        {
            return "-\u221e";
        }

        if (span == TimeSpan.MaxValue)
        {
            return "+\u221e";
        }

        if (span >= TimeSpan.FromDays(365))
        {
            return $"{span.TotalDays / 365}y";
        }

        if (span >= TimeSpan.FromDays(31))
        {
            return $"{span.TotalDays / 31}M";
        }

        if (span >= TimeSpan.FromDays(7))
        {
            return $"{span.TotalDays / 7}w";
        }

        if (span >= TimeSpan.FromDays(1))
        {
            return $"{span.TotalDays}d";
        }

        if (span >= TimeSpan.FromHours(1))
        {
            return $"{span.TotalHours}h";
        }

        if (span >= TimeSpan.FromMinutes(1))
        {
            return $"{span.TotalMinutes}m";
        }

        if (span >= TimeSpan.FromSeconds(1))
        {
            return $"{span.TotalSeconds}s";
        }

        if (span >= TimeSpan.FromMilliseconds(1))
        {
            return $"{span.TotalMilliseconds}ms";
        }

        if (span >= TimeSpan.FromMilliseconds(.001))
        {
            return $"{span.TotalMilliseconds * 1_000}µs";
        }

        return $"{span.TotalMilliseconds * 1_000 * 1_000}ns";
    }
}