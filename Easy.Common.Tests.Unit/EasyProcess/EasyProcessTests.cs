namespace Easy.Common.Tests.Unit.EasyProcess;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EasyProcess = Easy.Common.EasyProcess;

[TestFixture]
internal sealed class EasyProcessTests
{
    [Test]
    public async Task When_starting_a_process_with_no_errors()
    {
        using EasyProcess easyProc = new("dotnet", "--info");

        DateTime startTime = DateTime.Now;

        ChannelReader<ProcessOutputLine> reader = easyProc.Start(CancellationToken.None);

        List<ProcessOutputLine> lines = new();
        await foreach (ProcessOutputLine line in reader.ReadAllAsync())
        {
            lines.Add(line);
        }

        DateTime endTime = DateTime.Now;

        lines.Count(x => x.IsError).ShouldBe(0);
        lines.Count(x => !x.IsError).ShouldBeGreaterThan(1);

        lines.Count(s => s.Value.Contains(".NET SDKs installed:")).ShouldBe(1);
        lines.Count(s => s.Value.Contains(".NET runtimes installed:")).ShouldBe(1);

        easyProc.HasExited.ShouldBeTrue();
        easyProc.StartTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExitTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExecutionTime.ShouldBeInRange(1.Milliseconds(), 1.Minutes());
        easyProc.ExitCode.ShouldBe(0);
        easyProc.Id.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task When_starting_a_process_with_errors()
    {
        using EasyProcess easyProc = new("dotnet", "run foo");

        DateTime startTime = DateTime.Now;

        ChannelReader<ProcessOutputLine> reader = easyProc.Start(CancellationToken.None);

        List<ProcessOutputLine> lines = new();
        await foreach (ProcessOutputLine line in reader.ReadAllAsync())
        {
            lines.Add(line);
        }

        DateTime endTime = DateTime.Now;

        lines.Count(x => !x.IsError).ShouldBe(0);
        lines.Count(x => x.IsError).ShouldBe(1);

        lines[0].Value.ShouldStartWith("Couldn't find a project to run.");

        easyProc.HasExited.ShouldBeTrue();
        easyProc.StartTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExitTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExecutionTime.ShouldBeInRange(1.Milliseconds(), 1.Minutes());
        easyProc.ExitCode.ShouldBe(1);
        easyProc.Id.ShouldBeGreaterThan(0);
    }

    [Test, Ignore(reason: "Blocks AppVeyor")]
    public async Task When_starting_a_process_then_cancelling_execution_after_a_while()
    {
        string arg = OperatingSystem.IsLinux() ? "127.0.0.1 -4" : "127.0.0.1 -n 30 -4";
        using EasyProcess easyProc = new("ping", arg);

        using CancellationTokenSource cts = new(2_000);

        DateTime startTime = DateTime.Now;
        ChannelReader<ProcessOutputLine> reader = easyProc.Start(cts.Token);

        List<ProcessOutputLine> lines = new();
        await foreach (ProcessOutputLine line in reader.ReadAllAsync())
        {
            lines.Add(line);
            string prefix = line.IsError ? "Error" : "Info";
            Console.WriteLine($"[Test] [{prefix}] {line.Value}");
        }

        DateTime endTime = DateTime.Now;

        lines.Count(x => x.IsError).ShouldBe(0);
        lines.Count(s => s.Value.Contains("Reply from 127.0.0.1:")).ShouldBeGreaterThan(1);

        easyProc.HasExited.ShouldBeTrue();
        easyProc.StartTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExitTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExecutionTime.ShouldBeInRange(1.Milliseconds(), 1.Minutes());
        easyProc.ExitCode.ShouldBe(-1);
        easyProc.Id.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task When_starting_a_process_with_env_var_then_should_be_able_to_read_it()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Pass("windows only");
            return;
        }

        using EasyProcess proc1 = new("cmd", "/c set SOME_ENV");
        ChannelReader<ProcessOutputLine> reader1 = proc1.Start();

        List<ProcessOutputLine> proc1Lines = new();
        await foreach (ProcessOutputLine line in reader1.ReadAllAsync())
        {
            proc1Lines.Add(line);
        }

        proc1Lines.Count(x => x.IsError).ShouldBe(1);
        proc1Lines.Count(x => !x.IsError).ShouldBe(0);
        proc1Lines.ShouldContain(x => x.Value == "Environment variable SOME_ENV not defined");

        proc1.HasExited.ShouldBeTrue();
        proc1.ExitCode.ShouldBe(1);

        Dictionary<string, string> envVars = new()
        {
            ["SOME_ENV"] = "some-value"
        };

        using EasyProcess proc2 = new("cmd", "/c set SOME_ENV", envVars);
        ChannelReader<ProcessOutputLine> reader2 = proc2.Start();

        List<ProcessOutputLine> proc2Lines = new();
        await foreach (ProcessOutputLine line in reader2.ReadAllAsync())
        {
            proc2Lines.Add(line);
        }

        proc2Lines.Count(x => x.IsError).ShouldBe(0);
        proc2Lines.Count(x => !x.IsError).ShouldBe(1);
        proc2Lines.ShouldContain(x => x.Value == "SOME_ENV=some-value");

        proc2.HasExited.ShouldBeTrue();
        proc2.ExitCode.ShouldBe(0);
    }
}