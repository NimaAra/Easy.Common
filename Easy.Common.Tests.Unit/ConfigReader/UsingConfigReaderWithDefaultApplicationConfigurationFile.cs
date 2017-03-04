namespace Easy.Common.Tests.Unit.ConfigReader
{
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class UsingConfigReaderWithDefaultApplicationConfigurationFile : Context
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Given_a_config_reader_with_default_application_configuration_file();
        }

        [Test]
        public void Run()
        {
            ConfigReader.ConfigFile.ShouldNotBeNull();
            ConfigReader.ConfigFile.Exists.ShouldBeTrue();
            ConfigReader.ConfigFile.Name.ShouldStartWith("Easy.Common.Tests.Unit.dll.config");

            ConfigReader.Settings.ShouldNotBeNull();
            ConfigReader.Settings.Count.ShouldBe(3);

            ConfigReader.Settings["key-A"].ShouldBe("val-A");
            ConfigReader.Settings["key-B"].ShouldBe("val-B");
            ConfigReader.Settings["key-C"].ShouldBe("val-C");
        }
    }
}