namespace Easy.Common.Tests.Unit.ConfigReader
{
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class MappingValuesToAGivenValue : Context
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Given_a_config_reader_with_a_given_mapped_config();
        }

        [Test]
        public void When_reading_a_valid_set_of_values()
        {
            ConfigReader.ConfigFile.ShouldNotBeNull();
            ConfigReader.ConfigFile.Exists.ShouldBeTrue();
            ConfigReader.ConfigFile.Name.ShouldBe("Configuration.config");

            ConfigReader.Settings.ShouldNotBeNull();
            ConfigReader.Settings.ShouldBeEmpty();

            IDictionary<string, string> values;
            ConfigReader.TryRead("someSection", out values).ShouldBeTrue();
            values.ShouldNotBeNull();
            values.Count.ShouldBe(3);

            values["name"].ShouldBe("Foo");
            values["category"].ShouldBe("A");
            values["duration"].ShouldBe("50");

            values.ContainsKey("Name").ShouldBeFalse();
        }

        [Test]
        public void When_reading_an_invalid_set_of_values()
        {
            ConfigReader.ConfigFile.ShouldNotBeNull();
            ConfigReader.ConfigFile.Exists.ShouldBeTrue();
            ConfigReader.ConfigFile.Name.ShouldBe("Configuration.config");

            ConfigReader.Settings.ShouldNotBeNull();
            ConfigReader.Settings.ShouldBeEmpty();

            IDictionary<string, string> values;
            ConfigReader.TryRead("someMissingSection", out values).ShouldBeFalse();
            values.ShouldBeNull();
        }

        [Test]
        public void When_reading_an_empty_set_of_values()
        {
            ConfigReader.ConfigFile.ShouldNotBeNull();
            ConfigReader.ConfigFile.Exists.ShouldBeTrue();
            ConfigReader.ConfigFile.Name.ShouldBe("Configuration.config");

            ConfigReader.Settings.ShouldNotBeNull();
            ConfigReader.Settings.ShouldBeEmpty();

            IDictionary<string, string> values;
            ConfigReader.TryRead("emptySection", out values).ShouldBeFalse();
            values.ShouldBeNull();
        }

        [Test]
        public void When_reading_a_file_with_duplicate_keys()
        {
            ConfigReader.ConfigFile.ShouldNotBeNull();
            ConfigReader.ConfigFile.Exists.ShouldBeTrue();
            ConfigReader.ConfigFile.Name.ShouldBe("Configuration.config");

            ConfigReader.Settings.ShouldNotBeNull();
            ConfigReader.Settings.ShouldBeEmpty();

            IDictionary<string, string> values;

            Should.Throw<InvalidDataException>(() => ConfigReader.TryRead("duplicateSection", out values))
                .Message.ShouldBe("Multiple keys with the name: duplicateSection was found.");
        }
    }
}