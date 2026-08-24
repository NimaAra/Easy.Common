namespace Easy.Common.Tests.Unit.DiagnosticReport;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using DiagnosticReport = Easy.Common.DiagnosticReport;

[TestFixture]
internal sealed class DiagnosticReportTests
{
    private static readonly string NewLine = Environment.NewLine;

    [Test]
    public void When_generating_full_report()
    {
        var report = DiagnosticReport.Generate();
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.Full);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeEmpty();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldNotBeNull();
        report.EnvironmentVariables.ShouldNotBeEmpty();
        report.NetworkDetails.ShouldNotBeNull();

        report.SystemDetails.CPU.ShouldNotBe("<INVALID>");

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(1000);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_full_report_with_flag()
    {
        // ReSharper disable once RedundantArgumentDefaultValue
        var report = DiagnosticReport.Generate(DiagnosticReportType.Full);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.Full);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeEmpty();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldNotBeNull();
        report.EnvironmentVariables.ShouldNotBeEmpty();
        report.NetworkDetails.ShouldNotBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(1000);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_system_report_with_flag()
    {
        var report = DiagnosticReport.Generate(DiagnosticReportType.System);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.System);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_process_report_with_flag()
    {
        var report = DiagnosticReport.Generate(DiagnosticReportType.Process);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.Process);
        report.SystemDetails.ShouldBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_drives_report_with_flag()
    {
        var report = DiagnosticReport.Generate(DiagnosticReportType.Drives);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.Drives);
        report.SystemDetails.ShouldBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeEmpty();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_assemblies_report_with_flag()
    {
        var report = DiagnosticReport.Generate(DiagnosticReportType.Assemblies);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.Assemblies);
        report.SystemDetails.ShouldBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_environment_variables_report_with_flag()
    {
        var report = DiagnosticReport.Generate(DiagnosticReportType.EnvironmentVariables);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.EnvironmentVariables);
        report.SystemDetails.ShouldBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldNotBeNull();
        report.EnvironmentVariables.ShouldNotBeEmpty();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);
            
        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_networking_report_with_flag()
    {
        var report = DiagnosticReport.Generate(DiagnosticReportType.Networks);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(DiagnosticReportType.Networks);
        report.SystemDetails.ShouldBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldNotBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);
            
        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_system_and_process_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.System 
                                           | DiagnosticReportType.Process;

        var report = DiagnosticReport.Generate(Flags);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_system_and_drives_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.System 
                                           | DiagnosticReportType.Drives;
            
        var report = DiagnosticReport.Generate(Flags);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeEmpty();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_system_and_assemblies_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.System 
                                           | DiagnosticReportType.Assemblies;
            
        var report = DiagnosticReport.Generate(Flags);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_system_and_environment_variables_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.System 
                                           | DiagnosticReportType.EnvironmentVariables;
            
        var report = DiagnosticReport.Generate(Flags);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldNotBeNull();
        report.EnvironmentVariables.ShouldNotBeEmpty();
        report.NetworkDetails.ShouldBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldNotContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_system_and_networking_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.System 
                                           | DiagnosticReportType.Networks;
            
        var report = DiagnosticReport.Generate(Flags);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldBeNull();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldNotBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(100);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }
        
    [Test]
    public void When_generating_system_and_process_and_drives_and_assemblies_and_environment_variables_and_networking_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.System
                                           | DiagnosticReportType.Process
                                           | DiagnosticReportType.Drives
                                           | DiagnosticReportType.Assemblies
                                           | DiagnosticReportType.EnvironmentVariables
                                           | DiagnosticReportType.Networks;
            
        var report = DiagnosticReport.Generate();
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeEmpty();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldNotBeNull();
        report.EnvironmentVariables.ShouldNotBeEmpty();
        report.NetworkDetails.ShouldNotBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(1000);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_process_and_assemblies_and_networking_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.Process 
                                           | DiagnosticReportType.Assemblies 
                                           | DiagnosticReportType.Networks;

        var report = DiagnosticReport.Generate(Flags);
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldBeNull();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldBeNull();
        report.NetworkDetails.ShouldNotBeNull();

        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(500);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldNotContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }

    [Test]
    public void When_generating_process_and_assemblies_and_networking_and_full_report()
    {
        const DiagnosticReportType Flags = DiagnosticReportType.Process
                                           | DiagnosticReportType.Assemblies
                                           | DiagnosticReportType.Networks
                                           | DiagnosticReportType.Full;
            
        var report = DiagnosticReport.Generate();
        report.ShouldNotBeNull();
        report.Timestamp.ShouldBeLessThanOrEqualTo(DateTimeOffset.Now);
        report.TimeTaken.ShouldBeGreaterThan(0.Milliseconds());
        report.Type.ShouldBe(Flags);
        report.SystemDetails.ShouldNotBeNull();
        report.ProcessDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeNull();
        report.DriveDetails.ShouldNotBeEmpty();
        report.Assemblies.ShouldNotBeNull();
        report.Assemblies.ShouldNotBeEmpty();
        report.EnvironmentVariables.ShouldNotBeNull();
        report.EnvironmentVariables.ShouldNotBeEmpty();
        report.NetworkDetails.ShouldNotBeNull();
            
        var formattedReport = report.ToString();
        formattedReport.ShouldNotBeNull();
        formattedReport.Length.ShouldBeGreaterThan(1000);

        formattedReport.ShouldStartWith($"/{NewLine}|Diagnostic Report generated at:");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|System|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Process|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Drives|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Assemblies|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Environment-Variables|...");
        formattedReport.ShouldContain($"{NewLine}|{NewLine}|Networks|...");
        formattedReport.ShouldContain($"|{NewLine}|\t. Windows IP Configuration{NewLine}|");
        formattedReport.ShouldEndWith("\\");
    }
}