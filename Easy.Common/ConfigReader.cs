namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;
    using Easy.Common.Extensions;
    using Easy.Common.Interfaces;

    /// <summary>
    /// A class returning configuration values as static objects.
    /// </summary>
    public sealed class ConfigReader : IConfigReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigReader"/> class. 
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        /// Could not retrieve a <see cref="T:System.Collections.Specialized.NameValueCollection"/> object with the application settings data.
        /// </exception>
        public ConfigReader()
        {
            Settings = ConfigurationManager.AppSettings.ToDictionary();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigReader"/> class. 
        /// by loading <paramref name="configFile"/> and reading the values from it.
        /// </summary>
        /// <param name="configFile">Path to the configuration file</param>
        /// <param name="element">Name of the node which stores the key value pairs</param>
        /// <param name="keyAttribute">Attribute identifying the key</param>
        /// <param name="valueAttribute">Attribute identifying the value</param>
        public ConfigReader(FileInfo configFile, XName element, string keyAttribute = "key", string valueAttribute = "value")
        {
            Ensure.Exists(configFile);
            Ensure.NotNull(element, nameof(element));
            Ensure.NotNullOrEmptyOrWhiteSpace(keyAttribute);
            Ensure.NotNullOrEmptyOrWhiteSpace(valueAttribute);

            Settings = new Dictionary<string, string>();

            using (var fs = File.Open(configFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                foreach (var item in fs.GetElements(element))
                {
                    var itemKey = item.Attribute(keyAttribute);
                    var itemVal = item.Attribute(valueAttribute);

                    if (itemKey == null || itemVal == null) { continue; }

                    var key = itemKey.Value;

                    if (key.IsNullOrEmptyOrWhiteSpace()) { continue; }

                    Settings[key] = itemVal.Value;
                }
            }
        }

        /// <summary>
        /// Gets all of the settings retrieved from the configuration.
        /// </summary>
        public Dictionary<string, string> Settings { get; }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out string value)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(key);
            return Settings.TryGetValue(key, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out short value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return short.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out int value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return int.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out long value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return long.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out ushort value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return ushort.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out uint value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return uint.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out ulong value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return ulong.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out float value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return float.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out double value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return double.TryParse(valStr, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out decimal value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return decimal.TryParse(valStr, out value);
        }

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
        public bool TryRead(string key, out bool value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return valStr.TryParseAsBool(out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="separator">The <see cref="string"/> separating the values</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryReadStringAsCsv(string key, string separator, out string[] value)
        {
            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            value = valStr.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetTicks(string key, out TimeSpan value)
        {
            string valStr;
            long valLong;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valLong))
            {
                return false;
            }

            value = TimeSpan.FromTicks(valLong);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetMilliseconds(string key, out TimeSpan value)
        {
            string valStr;
            double valDouble;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valDouble))
            {
                return false;
            }

            value = TimeSpan.FromMilliseconds(valDouble);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetSeconds(string key, out TimeSpan value)
        {
            string valStr;
            double valDouble;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valDouble))
            {
                return false;
            }

            value = TimeSpan.FromSeconds(valDouble);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetMinutes(string key, out TimeSpan value)
        {
            string valStr;
            double valDouble;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valDouble))
            {
                return false;
            }

            value = TimeSpan.FromMinutes(valDouble);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetHours(string key, out TimeSpan value)
        {
            string valStr;
            double valDouble;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valDouble))
            {
                return false;
            }

            value = TimeSpan.FromHours(valDouble);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetDays(string key, out TimeSpan value)
        {
            string valStr;
            double valDouble;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valDouble))
            {
                return false;
            }

            value = TimeSpan.FromDays(valDouble);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetWeeks(string key, out TimeSpan value)
        {
            string valStr;
            double valDouble;
            if (!TryGetString(key, out value, out valStr) || !TryRead(key, out valDouble))
            {
                return false;
            }

            value = TimeSpan.FromDays(valDouble * 7);
            return true;
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="formatSpecifier">The format used to parse the value as <paramref name="value"/></param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, string formatSpecifier, out DateTime value)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(formatSpecifier);

            string valStr;
            if (!TryGetString(key, out value, out valStr))
            {
                return false;
            }

            return DateTime.TryParseExact(valStr, formatSpecifier, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out FileInfo value)
        {
            string valStr;
            if (!TryRead(key, out valStr))
            {
                value = null;
                return false;
            }

            try
            {
                value = new FileInfo(Path.GetFullPath(valStr));
                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out DirectoryInfo value)
        {
            string valStr;
            if (!TryRead(key, out valStr))
            {
                value = null;
                return false;
            }

            try
            {
                value = new DirectoryInfo(Path.GetFullPath(valStr));
                return true;
            }
            catch (Exception)
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated to the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out Uri value)
        {
            string valStr;
            if (!TryRead(key, out valStr))
            {
                value = null;
                return false;
            }

            try
            {
                value = new Uri(valStr);
                return true;
            }
            catch (UriFormatException)
            {
                value = null;
                return false;
            }
        }

        private bool TryGetString<T>(string key, out T defaultVal, out string value)
        {
            defaultVal = default(T);

            string valStr;
            if (!TryRead(key, out valStr))
            {
                value = null;
                return false;
            }

            value = valStr;
            return true;
        }
    }

}