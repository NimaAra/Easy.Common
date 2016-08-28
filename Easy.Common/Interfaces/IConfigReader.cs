namespace Easy.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// A contact defining how configuration values should be returned as static objects.
    /// </summary>
    public interface IConfigReader
    {
        /// <summary>
        /// Gets all of the settings retrieved from the configuration.
        /// </summary>
        Dictionary<string, string> Settings { get; }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out string value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out short value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out int value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out long value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out ushort value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out uint value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out ulong value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out float value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out double value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out decimal value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// <remarks>
        /// The following values can be parsed (case-insensitive): 
        ///     <c>True/False</c>,
        ///     <c>Yes/No</c>,
        ///     <c>0/1</c>
        /// </remarks>
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out bool value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="separator">The <see cref="string"/> separating the values</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryReadStringAsCsv(string key, string separator, out string[] value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetTicks(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetMilliseconds(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetSeconds(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetMinutes(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetHours(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetDays(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryGetWeeks(string key, out TimeSpan value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="formatSpecifier">The format used to parse the value as <paramref name="value"/></param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, string formatSpecifier, out DateTime value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out FileInfo value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out DirectoryInfo value);

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        bool TryRead(string key, out Uri value);
    }
}