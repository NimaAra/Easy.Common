namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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
        /// <remarks>
        /// This constructor searches for default config file of the calling assembly.
        /// </remarks>
        /// </summary>
        public ConfigReader()
        {
            var assLocalPath = new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath;
            var configFile = new FileInfo(assLocalPath + ".config");
            
            Ensure.Exists(configFile);

            Init(configFile, "add", "key", "value");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigReader"/> class. 
        /// by loading <paramref name="configFile"/> and reading the values from it.
        /// </summary>
        /// <param name="configFile">Path to the configuration file</param>
        /// <param name="element">Name of the node which stores the key value pairs</param>
        /// <param name="keyAttribute">Attribute identifying the key</param>
        /// <param name="valueAttribute">Attribute identifying the value</param>
        public ConfigReader(FileInfo configFile, XName element, string keyAttribute = "key", string valueAttribute = "value") => 
            Init(configFile, element, keyAttribute, valueAttribute);

        private void Init(FileInfo configFile, XName element, string keyAttribute, string valueAttribute)
        {
            Ensure.NotNull(configFile, nameof(configFile));
            Ensure.NotNull(element, nameof(element));
            Ensure.NotNullOrEmptyOrWhiteSpace(keyAttribute);
            Ensure.NotNullOrEmptyOrWhiteSpace(valueAttribute);

            ConfigFile = Ensure.Exists(configFile);
            Settings = new Dictionary<string, string>();

            using FileStream fs = ConfigFile.OpenRead();
            
            foreach (var item in fs.GetElements(element))
            {
                XAttribute itemKey = item.Attribute(keyAttribute);
                XAttribute itemVal = item.Attribute(valueAttribute);

                if (itemKey is null || itemVal is null) { continue; }

                var key = itemKey.Value;

                if (key.IsNullOrEmptyOrWhiteSpace()) { continue; }

                Settings[key] = itemVal.Value;
            }
        }

        /// <summary>
        /// Gets the file storing the config entries.
        /// </summary>
        public FileInfo ConfigFile { get; private set; }

        /// <summary>
        /// Gets all of the settings retrieved from the configuration.
        /// </summary>
        public Dictionary<string, string> Settings { get; private set; }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="values"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="values">The set of values associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out IDictionary<string, string> values)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(key);

            using FileStream fs = ConfigFile.OpenRead();
            
            var result = new Dictionary<string, string>();

            var elements = fs.GetElements(key).ToArray();

            if (!elements.Any())
            {
                values = null;
                return false;
            }

            if (elements.Length > 1)
            {
                throw new InvalidDataException($"Multiple keys with the name: {key} was found.");
            }

            foreach (var item in elements[0].Attributes())
            {
                var attrName = item.Name.ToString();
                var attrValue = item.Value;

                result[attrName] = attrValue;
            }

            if (result.Any())
            {
                values = result;
                return true;
            }

            values = null;
            return false;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out string value)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(key);
            return Settings.TryGetValue(key, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out short value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return short.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out int value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return int.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out long value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return long.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out ushort value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return ushort.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out uint value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return uint.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out ulong value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return ulong.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out float value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return float.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out double value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return double.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out decimal value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return decimal.TryParse(valStr, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// <remarks>
        /// The following values can be parsed (case-insensitive): 
        ///     <c>True/False</c>,
        ///     <c>Yes/No</c>,
        ///     <c>0/1</c>
        /// </remarks>
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out bool value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            return valStr.TryParseAsBool(out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="separator">The <see cref="string"/> separating the values</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryReadStringAsCSV(string key, string separator, out string[] value)
        {
            if (!TryGetString(key, out value, out var valStr))
            {
                return false;
            }

            value = valStr.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetTicks(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out var _) || !TryRead(key, out long valLong))
            {
                return false;
            }

            value = TimeSpan.FromTicks(valLong);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetMilliseconds(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out _) || !TryRead(key, out double valDouble))
            {
                return false;
            }

            value = TimeSpan.FromMilliseconds(valDouble);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetSeconds(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out _) || !TryRead(key, out double valDouble))
            {
                return false;
            }

            value = TimeSpan.FromSeconds(valDouble);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetMinutes(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out _) || !TryRead(key, out double valDouble))
            {
                return false;
            }

            value = TimeSpan.FromMinutes(valDouble);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetHours(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out _) || !TryRead(key, out double valDouble))
            {
                return false;
            }

            value = TimeSpan.FromHours(valDouble);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetDays(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out _) || !TryRead(key, out double valDouble))
            {
                return false;
            }

            value = TimeSpan.FromDays(valDouble);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryGetWeeks(string key, out TimeSpan value)
        {
            if (!TryGetString(key, out value, out _) || !TryRead(key, out double valDouble))
            {
                return false;
            }

            value = TimeSpan.FromDays(valDouble * 7);
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="formatSpecifier">The format used to parse the value as <paramref name="value"/></param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, string formatSpecifier, out DateTime value)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(formatSpecifier);

            if (!TryGetString(key, out value, out string valStr))
            {
                return false;
            }

            return DateTime.TryParseExact(valStr, formatSpecifier, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out FileInfo value)
        {
            if (!TryRead(key, out string valStr))
            {
                value = null;
                return false;
            }

            value = new FileInfo(Path.GetFullPath(valStr));
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out DirectoryInfo value)
        {
            if (!TryRead(key, out string valStr))
            {
                value = null;
                return false;
            }

            value = new DirectoryInfo(Path.GetFullPath(valStr));
            return true;
        }

        /// <summary>
        /// Attempts to read a config value specified as <paramref name="key"/> into <paramref name="value"/>.
        /// </summary>
        /// <param name="key">The key to retrieve from the configuration</param>
        /// <param name="value">The value associated with the <paramref name="key"/></param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        public bool TryRead(string key, out Uri value)
        {
            if (!TryRead(key, out string valStr))
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
            defaultVal = default;

            if (!TryRead(key, out string valStr))
            {
                value = null;
                return false;
            }

            value = valStr;
            return true;
        }
    }
}