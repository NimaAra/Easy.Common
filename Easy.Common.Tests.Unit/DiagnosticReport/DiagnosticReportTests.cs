namespace Easy.Common.Tests.Unit.DiagnosticReport
{
    using NUnit.Framework;
    using Shouldly;
    using DiagnosticReport = Easy.Common.DiagnosticReport;

    [TestFixture]
    internal sealed class DiagnosticReportTests
    {
        [Test]
        public void When_generating_full_report()
        {
            var report = DiagnosticReport.Generate();
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(1000);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_full_report_with_flag()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            var report = DiagnosticReport.Generate(DiagnosticReportType.Full);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(1000);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_system_report_with_flag()
        {
            var report = DiagnosticReport.Generate(DiagnosticReportType.System);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_process_report_with_flag()
        {
            var report = DiagnosticReport.Generate(DiagnosticReportType.Process);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldNotContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_drives_report_with_flag()
        {
            var report = DiagnosticReport.Generate(DiagnosticReportType.Drives);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldNotContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_assemblies_report_with_flag()
        {
            var report = DiagnosticReport.Generate(DiagnosticReportType.Assemblies);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldNotContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_environment_variables_report_with_flag()
        {
            var report = DiagnosticReport.Generate(DiagnosticReportType.EnvironmentVariables);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);
            
            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldNotContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_networking_report_with_flag()
        {
            var report = DiagnosticReport.Generate(DiagnosticReportType.Networking);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);
            
            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldNotContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_system_and_process_report()
        {
            var flags = DiagnosticReportType.System | DiagnosticReportType.Process;
            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_system_and_drives_report()
        {
            var flags = DiagnosticReportType.System | DiagnosticReportType.Drives;
            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_system_and_assemblies_report()
        {
            var flags = DiagnosticReportType.System | DiagnosticReportType.Assemblies;
            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_system_and_environment_variables_report()
        {
            var flags = DiagnosticReportType.System | DiagnosticReportType.EnvironmentVariables;
            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldNotContain("\r\n|\r\n|Networking|...");
            report.ShouldNotContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_system_and_networking_report()
        {
            var flags = DiagnosticReportType.System | DiagnosticReportType.Networking;
            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(100);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldNotContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldNotContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }


        [Test]
        public void When_generating_system_and_process_and_drives_and_assemblies_and_environment_variables_and_networking_report()
        {
            var flags = DiagnosticReportType.System
                | DiagnosticReportType.Process
                | DiagnosticReportType.Drives
                | DiagnosticReportType.Assemblies
                | DiagnosticReportType.EnvironmentVariables
                | DiagnosticReportType.Networking;
            
            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(1000);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_process_and_assemblies_and_networking_report()
        {
            var flags = DiagnosticReportType.Process 
                | DiagnosticReportType.Assemblies 
                | DiagnosticReportType.Networking;

            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(500);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldNotContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldNotContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldNotContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }

        [Test]
        public void When_generating_process_and_assemblies_and_networking_and_full_report()
        {
            var flags = DiagnosticReportType.Process
                        | DiagnosticReportType.Assemblies
                        | DiagnosticReportType.Networking
                        | DiagnosticReportType.Full;

            var report = DiagnosticReport.Generate(flags);
            report.ShouldNotBeNull();
            report.Length.ShouldBeGreaterThan(1000);

            report.ShouldStartWith("/\r\n|Diagnostic Report generated at:");
            report.ShouldContain("\r\n|\r\n|System|...");
            report.ShouldContain("\r\n|\r\n|Process|...");
            report.ShouldContain("\r\n|\r\n|Drives|...");
            report.ShouldContain("\r\n|\r\n|Assemblies|...");
            report.ShouldContain("\r\n|\r\n|Environment-Variables|...");
            report.ShouldContain("\r\n|\r\n|Networking|...");
            report.ShouldContain("|\r\n|\t. Windows IP Configuration\r\n|");
            report.ShouldEndWith("\\");
        }
    }
}