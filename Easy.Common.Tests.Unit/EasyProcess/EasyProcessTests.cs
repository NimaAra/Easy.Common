namespace Easy.Common.Tests.Unit.EasyProcess;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EasyProcess = Easy.Common.EasyProcess;

[TestFixture]
internal sealed class EasyProcessTests
{
    private readonly ConcurrentBag<string> _outputLines = new();
    private readonly ConcurrentBag<string> _errorLines = new();
    
    [SetUp]
    public void TestSetUp()
    {
        _outputLines.Clear();
        _errorLines.Clear();
    }

    [Test]
    public async Task When_starting_a_process_with_no_errors()
    {
        using EasyProcess easyProc = new("ping", "-n 3 localhost -4");
        
        easyProc.OnOutput += (_, s) => _outputLines.Add(s);
        easyProc.OnError += (_, s) => _errorLines.Add(s);

        DateTime startTime = DateTime.Now;
        
        await easyProc.Start(CancellationToken.None);

        DateTime endTime = DateTime.Now;

        _errorLines.ShouldBeEmpty();
        _outputLines.Count.ShouldBeGreaterThan(3);

        _outputLines.Count(s => s.Contains("Reply from 127.0.0.1:")).ShouldBe(3);

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
        
        easyProc.OnOutput += (_, s) => _outputLines.Add(s);
        easyProc.OnError += (_, s) => _errorLines.Add(s);

        DateTime startTime = DateTime.Now;
        
        await easyProc.Start(CancellationToken.None);

        DateTime endTime = DateTime.Now;

        _outputLines.ShouldBeEmpty();
        _errorLines.Count.ShouldBe(1);

        _errorLines.First().ShouldStartWith("Couldn't find a project to run.");

        easyProc.HasExited.ShouldBeTrue();
        easyProc.StartTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExitTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExecutionTime.ShouldBeInRange(1.Milliseconds(), 1.Minutes());
        easyProc.ExitCode.ShouldBe(1);
        easyProc.Id.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task When_starting_a_process_then_cancelling_execution_after_a_while()
    {
        using EasyProcess easyProc = new("ping", "-t localhost -4");
        
        easyProc.OnOutput += (_, s) => _outputLines.Add(s);
        easyProc.OnError += (_, s) => _errorLines.Add(s);

        using CancellationTokenSource cts = new(2_000);

        DateTime startTime = DateTime.Now;
        
        await easyProc.Start(cts.Token);

        DateTime endTime = DateTime.Now;

        _errorLines.ShouldBeEmpty();
        _outputLines.Count(s => s.Contains("Reply from 127.0.0.1:")).ShouldBeGreaterThan(1);

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
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Pass("windows only");
            return;
        }

        using EasyProcess proc1 = new("cmd", "/c set SOME_ENV");
        proc1.OnOutput += (_, s) => _outputLines.Add(s);
        proc1.OnError += (_, s) => _errorLines.Add(s);
        
        await proc1.Start();

        _errorLines.Count.ShouldBe(1);
        _outputLines.ShouldBeEmpty();
        _errorLines.ShouldContain(x => x == "Environment variable SOME_ENV not defined");

        proc1.HasExited.ShouldBeTrue();
        proc1.ExitCode.ShouldBe(1);

        _errorLines.Clear();
        _outputLines.Clear();

        Dictionary<string, string> envVars = new()
        {
            ["SOME_ENV"] = "some-value"
        };

        using EasyProcess proc2 = new("cmd", "/c set SOME_ENV", envVars);
        proc2.OnOutput += (_, s) => _outputLines.Add(s);
        proc2.OnError += (_, s) => _errorLines.Add(s);
        
        await proc2.Start();

        _errorLines.ShouldBeEmpty();
        _outputLines.Count.ShouldBe(1);
        _outputLines.ShouldContain(x => x == "SOME_ENV=some-value");

        proc2.HasExited.ShouldBeTrue();
        proc2.ExitCode.ShouldBe(0);
    }
}