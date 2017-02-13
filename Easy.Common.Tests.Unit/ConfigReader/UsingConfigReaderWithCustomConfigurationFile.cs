namespace Easy.Common.Tests.Unit.ConfigReader
{
    using System;
    using System.IO;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class UsingConfigReaderWithCustomConfigurationFile : Context
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Given_a_config_reader_with_custom_configuration_file();
        }

        [Test]
        public void Run()
        {
            ConfigReader.Settings.ShouldNotBeNull();
            ConfigReader.Settings.Count.ShouldBe(58);

            ConfigReader.Settings.ShouldContainKey("should-not-be-ignored");

            ConfigReader.Settings.ShouldNotContainKey("string-with-no-value");
            ConfigReader.Settings.ShouldNotContainKey("non-existent-key");
            ConfigReader.Settings.ShouldNotContainKey("ignore");

            string stringVal;
            ConfigReader.TryRead("string", out stringVal).ShouldBeTrue();
            stringVal.ShouldBe("bar");

            ConfigReader.TryRead("StRiNg", out stringVal).ShouldBeFalse();
            stringVal.ShouldBeNull("Because keys are case-sensetive");

            ConfigReader.TryRead("empty-string", out stringVal).ShouldBeTrue();
            stringVal.ShouldBe(string.Empty);

            short shortVal;
            ConfigReader.TryRead("pos-short", out shortVal).ShouldBeTrue();
            shortVal.ShouldBe((short)1);

            ConfigReader.TryRead("non-existent-key", out shortVal).ShouldBeFalse();
            shortVal.ShouldBe(default(short));

            ConfigReader.TryRead("neg-short", out shortVal).ShouldBeTrue();
            shortVal.ShouldBe((short)-1);

            ushort ushortVal;
            ConfigReader.TryRead("pos-ushort", out ushortVal).ShouldBeTrue();
            ushortVal.ShouldBe((ushort)1);

            ConfigReader.TryRead("neg-ushort", out ushortVal).ShouldBeFalse();
            ushortVal.ShouldBe((ushort)0);

            ConfigReader.TryRead("non-existent-key", out ushortVal).ShouldBeFalse();
            ushortVal.ShouldBe(default(ushort));

            int intVal;
            ConfigReader.TryRead("pos-int", out intVal).ShouldBeTrue();
            intVal.ShouldBe(1);

            ConfigReader.TryRead("non-existent-key", out intVal).ShouldBeFalse();
            intVal.ShouldBe(default(int));

            ConfigReader.TryRead("neg-int", out intVal).ShouldBeTrue();
            intVal.ShouldBe(-1);

            uint uintVal;
            ConfigReader.TryRead("pos-int", out uintVal).ShouldBeTrue();
            uintVal.ShouldBe((uint)1);

            ConfigReader.TryRead("neg-int", out uintVal).ShouldBeFalse();
            uintVal.ShouldBe((uint)0);

            ConfigReader.TryRead("non-existent-key", out uintVal).ShouldBeFalse();
            uintVal.ShouldBe(default(uint));

            long longVal;
            ConfigReader.TryRead("pos-long", out longVal).ShouldBeTrue();
            longVal.ShouldBe(1);

            ConfigReader.TryRead("neg-long", out longVal).ShouldBeTrue();
            longVal.ShouldBe(-1);

            ConfigReader.TryRead("non-existent-key", out longVal).ShouldBeFalse();
            longVal.ShouldBe(default(long));

            ulong ulongVal;
            ConfigReader.TryRead("pos-int", out ulongVal).ShouldBeTrue();
            ulongVal.ShouldBe((ulong)1);

            ConfigReader.TryRead("non-existent-key", out ulongVal).ShouldBeFalse();
            ulongVal.ShouldBe(default(ulong));

            ConfigReader.TryRead("neg-int", out ulongVal).ShouldBeFalse();
            ulongVal.ShouldBe((ulong)0);

            float floatVal;
            ConfigReader.TryRead("float", out floatVal).ShouldBeTrue();
            floatVal.ShouldBe(1.1f);

            ConfigReader.TryRead("non-existent-key", out floatVal).ShouldBeFalse();
            floatVal.ShouldBe(default(float));

            double doubleVal;
            ConfigReader.TryRead("double", out doubleVal).ShouldBeTrue();
            doubleVal.ShouldBe(1.12d);

            ConfigReader.TryRead("non-existent-key", out doubleVal).ShouldBeFalse();
            doubleVal.ShouldBe(default(double));

            decimal decimalVal;
            ConfigReader.TryRead("decimal", out decimalVal).ShouldBeTrue();
            decimalVal.ShouldBe(1.123m);

            ConfigReader.TryRead("non-existent-key", out decimalVal).ShouldBeFalse();
            decimalVal.ShouldBe(default(decimal));

            bool boolVal;
            ConfigReader.TryRead("bool-true-1", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("non-existent-key", out boolVal).ShouldBeFalse();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-true-2", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("bool-true-3", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("bool-yes-1", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("bool-yes-2", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("bool-yes-3", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("bool-1", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeTrue();

            ConfigReader.TryRead("bool-false-1", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-false-2", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-false-3", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-no-1", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-no-2", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-no-3", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-0", out boolVal).ShouldBeTrue();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-invalid", out boolVal).ShouldBeFalse();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-invalid-number", out boolVal).ShouldBeFalse();
            boolVal.ShouldBeFalse();

            ConfigReader.TryRead("bool-invalid-empty", out boolVal).ShouldBeFalse();
            ConfigReader.TryRead("bool-invalid-space", out boolVal).ShouldBeFalse();

            string[] csvVal;
            ConfigReader.TryReadStringAsCsv("pipe-delimited", "|", out csvVal).ShouldBeTrue();
            csvVal.ShouldBe(new []{"A", " B", " C", "D"});

            ConfigReader.TryReadStringAsCsv("pipe-delimited", ",", out csvVal).ShouldBeTrue();
            csvVal.ShouldBe(new[] { "A| B| C|D" });

            ConfigReader.TryReadStringAsCsv("non-existent-key", "|", out csvVal).ShouldBeFalse();
            csvVal.ShouldBeNull();

            TimeSpan timeSpanVal;
            ConfigReader.TryGetTicks("timespan-tick", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(156.Ticks());

            ConfigReader.TryGetTicks("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            ConfigReader.TryGetMilliseconds("timespan-millisec", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(267.Milliseconds());

            ConfigReader.TryGetMilliseconds("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            ConfigReader.TryGetSeconds("timespan-sec", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(378.Seconds());

            ConfigReader.TryGetSeconds("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            ConfigReader.TryGetMinutes("timespan-min", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(489.Minutes());

            ConfigReader.TryGetMinutes("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            ConfigReader.TryGetHours("timespan-hour", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(591.Hours());

            ConfigReader.TryGetHours("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            ConfigReader.TryGetDays("timespan-day", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(602.Days());

            ConfigReader.TryGetDays("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            ConfigReader.TryGetWeeks("timespan-week", out timeSpanVal).ShouldBeTrue();
            timeSpanVal.ShouldBe(6.Weeks());

            ConfigReader.TryGetWeeks("non-existent-key", out timeSpanVal).ShouldBeFalse();
            timeSpanVal.ShouldBe(default(TimeSpan));

            DateTime dateTimeVal;
            ConfigReader.TryRead("datetime", "yyyy-MM-dd HH:mm:ss.fff", out dateTimeVal).ShouldBeTrue();
            dateTimeVal.ShouldBe(new DateTime(2014, 01, 03, 13, 15, 24, 532, DateTimeKind.Unspecified));

            ConfigReader.TryRead("datetime", "MM-dd-yyyy HH:mm:ss.fff", out dateTimeVal).ShouldBeFalse();
            dateTimeVal.ShouldBe(default(DateTime));

            ConfigReader.TryRead("non-existent-key", "not-important", out dateTimeVal).ShouldBeFalse();
            dateTimeVal.ShouldBe(default(DateTime));

            FileInfo fileInfoVal;
            ConfigReader.TryRead("file-path-1", out fileInfoVal).ShouldBeTrue();
            fileInfoVal.FullName.ShouldBe(@"C:\some-folder\some-file");

            ConfigReader.TryRead("file-path-2", out fileInfoVal).ShouldBeTrue();
            fileInfoVal.FullName.ShouldBe(@"C:\some-folder\some-file.txt");

            ConfigReader.TryRead("file-path-invalid", out fileInfoVal).ShouldBeFalse();
            fileInfoVal.ShouldBe(default(FileInfo));

            ConfigReader.TryRead("non-existent-key", out fileInfoVal).ShouldBeFalse();
            fileInfoVal.ShouldBe(default(FileInfo));

            DirectoryInfo directoryInfoVal;
            ConfigReader.TryRead("directory-path-1", out directoryInfoVal).ShouldBeTrue();
            directoryInfoVal.FullName.ShouldBe(@"C:\some-folder");

            ConfigReader.TryRead("directory-path-2", out directoryInfoVal).ShouldBeTrue();
            directoryInfoVal.FullName.ShouldBe(@"C:\some-folder\");

            ConfigReader.TryRead("directory-path-invalid", out directoryInfoVal).ShouldBeFalse();
            directoryInfoVal.ShouldBe(default(DirectoryInfo));

            ConfigReader.TryRead("non-existent-key", out directoryInfoVal).ShouldBeFalse();
            directoryInfoVal.ShouldBe(default(DirectoryInfo));

            Uri uri;
            ConfigReader.TryRead("uri-1", out uri).ShouldBeTrue();
            uri.Scheme.ShouldBe("http");
            uri.Port.ShouldBe(80);
            uri.Query.ShouldBe(string.Empty);
            uri.PathAndQuery.ShouldBe("/api");
            uri.AbsoluteUri.ShouldBe("http://www.google.com/api");

            ConfigReader.TryRead("uri-2", out uri).ShouldBeTrue();
            uri.Scheme.ShouldBe("http");
            uri.Port.ShouldBe(80);
            uri.Query.ShouldBe(string.Empty);
            uri.PathAndQuery.ShouldBe("/api/");
            uri.AbsoluteUri.ShouldBe("http://www.google.com/api/");

            ConfigReader.TryRead("uri-3", out uri).ShouldBeTrue();
            uri.Scheme.ShouldBe("http");
            uri.Port.ShouldBe(8080);
            uri.Query.ShouldBe(string.Empty);
            uri.PathAndQuery.ShouldBe("/");
            uri.AbsoluteUri.ShouldBe("http://www.google.com:8080/");

            ConfigReader.TryRead("uri-4", out uri).ShouldBeTrue();
            uri.Scheme.ShouldBe("https");
            uri.Port.ShouldBe(8080);
            uri.Query.ShouldBe(string.Empty);
            uri.PathAndQuery.ShouldBe("/api/1");
            uri.AbsoluteUri.ShouldBe("https://www.google.com:8080/api/1");

            ConfigReader.TryRead("uri-5", out uri).ShouldBeTrue();
            uri.Scheme.ShouldBe("ftp");
            uri.Port.ShouldBe(8080);
            uri.Query.ShouldBe(string.Empty);
            uri.PathAndQuery.ShouldBe("/api/1/");
            uri.AbsoluteUri.ShouldBe("ftp://www.google.com:8080/api/1/");

            ConfigReader.TryRead("uri-6", out uri).ShouldBeTrue();
            uri.Scheme.ShouldBe("http");
            uri.Port.ShouldBe(8080);
            uri.Query.ShouldBe("?v=foo_bar");
            uri.PathAndQuery.ShouldBe("/param?v=foo_bar");
            uri.AbsoluteUri.ShouldBe("http://www.google.com:8080/param?v=foo_bar");

            ConfigReader.TryRead("uri-invalid", out uri).ShouldBeFalse();
            uri.ShouldBe(default(Uri));
        }
    }
}