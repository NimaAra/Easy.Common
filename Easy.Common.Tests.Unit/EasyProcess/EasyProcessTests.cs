namespace Easy.Common.Tests.Unit.EasyProcess;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using EasyProcess = Easy.Common.EasyProcess;

[TestFixture]
internal sealed class EasyProcessTests
{
    private readonly List<string> _outputLines = new();
    private readonly List<string> _errorLines = new();
    
    [SetUp]
    public void TestSetUp()
    {
        _outputLines.Clear();
        _errorLines.Clear();
    }

    [Test]
    public async Task When_starting_a_process_with_no_errors()
    {
        using EasyProcess easyProc = new("ping", "-n 3 localhost");
        
        easyProc.OnOutput += (_, s) => _outputLines.Add(s);
        easyProc.OnError += (_, s) => _errorLines.Add(s);

        DateTime startTime = DateTime.Now;
        
        await easyProc.Start(CancellationToken.None);

        DateTime endTime = DateTime.Now;

        _errorLines.ShouldBeEmpty();
        _outputLines.Count.ShouldBeGreaterThan(3);

        _outputLines.Count(s => s.Contains("Reply from ::1:")).ShouldBe(3);

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
        using EasyProcess easyProc = new("dir", "localhost");
        
        easyProc.OnOutput += (_, s) => _outputLines.Add(s);
        easyProc.OnError += (_, s) => _errorLines.Add(s);

        DateTime startTime = DateTime.Now;
        
        await easyProc.Start(CancellationToken.None);

        DateTime endTime = DateTime.Now;

        _outputLines.ShouldBeEmpty();
        _errorLines.Count.ShouldBe(1);

        _errorLines[0].ShouldBe("dir: cannot access 'localhost': No such file or directory");

        easyProc.HasExited.ShouldBeTrue();
        easyProc.StartTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExitTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExecutionTime.ShouldBeInRange(1.Milliseconds(), 1.Minutes());
        easyProc.ExitCode.ShouldBe(2);
        easyProc.Id.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task When_starting_a_process_then_cancelling_execution_after_a_while()
    {
        using EasyProcess easyProc = new("ping", "-t localhost");
        
        easyProc.OnOutput += (_, s) => _outputLines.Add(s);
        easyProc.OnError += (_, s) => _errorLines.Add(s);

        using CancellationTokenSource cts = new(2_000);

        DateTime startTime = DateTime.Now;
        
        await easyProc.Start(cts.Token);

        DateTime endTime = DateTime.Now;

        _errorLines.ShouldBeEmpty();
        _outputLines.Count(s => s.Contains("Reply from ::1:")).ShouldBeGreaterThan(1);

        easyProc.HasExited.ShouldBeTrue();
        easyProc.StartTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExitTime.ShouldBeInRange(startTime, endTime);
        easyProc.ExecutionTime.ShouldBeInRange(1.Milliseconds(), 1.Minutes());
        easyProc.ExitCode.ShouldBe(-1);
        easyProc.Id.ShouldBeGreaterThan(0);
    }
}