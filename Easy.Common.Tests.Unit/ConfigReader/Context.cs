namespace Easy.Common.Tests.Unit.ConfigReader
{
    using System;
    using System.IO;
    using Easy.Common.Interfaces;
    using ConfigReader = Easy.Common.ConfigReader;

    public class Context
    {
        protected IConfigReader ConfigReader;

        protected void Given_a_config_reader_with_default_application_configuration_file()
        {
            ConfigReader = new ConfigReader();
        }

        protected void Given_a_config_reader_with_a_given_mapped_config()
        {
            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigReader", "Configuration.config"));
            ConfigReader = new ConfigReader(configFile);
        }

        protected void Given_a_config_reader_with_custom_configuration_file_with_specified_settings()
        {
            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigReader", "Configuration.config"));
            ConfigReader = new ConfigReader(configFile, "add", "key", "format");
        }
    }
}