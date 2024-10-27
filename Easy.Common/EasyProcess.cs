namespace Easy.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

/// <summary>
/// Provides an abstraction to simplify executing a process asynchronously.
/// </summary>
public sealed class EasyProcess : IDisposable
{
    private readonly Process _process;
    private readonly Channel<ProcessOutputLine> _outputChannel;

    private EasyProcess(ProcessStartInfo startInfo, IReadOnlyDictionary<string, string>? envVars)
    {
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        if (envVars is not null)
        {
            startInfo.Environment.Clear();
            foreach (var item in envVars)
            {
                startInfo.Environment.Add(item.Key, item.Value);
            }
        }

        _outputChannel = Channel.CreateUnbounded<ProcessOutputLine>(new UnboundedChannelOptions()
        {
            SingleReader = false,
            SingleWriter = true,
            AllowSynchronousContinuations = false
        });

        _process = new()
        {
            EnableRaisingEvents = true,
            StartInfo = startInfo
        };

        _process.OutputDataReceived += OnOutputData;
        _process.ErrorDataReceived += OnErrorData;
    }

    /// <summary>
    /// Creates an instance of the <see cref="EasyProcess"/>.
    /// </summary>
    public EasyProcess(string processPath, string args, IReadOnlyDictionary<string, string>? envVars = null) :
        this(new ProcessStartInfo(processPath, args), envVars) =>
            Ensure.NotNullOrEmptyOrWhiteSpace(processPath);

    /// <summary>
    /// Creates an instance of the <see cref="EasyProcess"/>.
    /// </summary>
    public EasyProcess(FileInfo processPath, string args, IReadOnlyDictionary<string, string>? envVars = null) :
        this(new ProcessStartInfo(processPath.FullName, args), envVars) =>
            Ensure.NotNull(processPath, nameof(processPath));

    /// <summary>
    /// Creates an instance of the <see cref="EasyProcess"/>.
    /// </summary>
    public EasyProcess(FileInfo processPath, DirectoryInfo workingDirectory, string args, IReadOnlyDictionary<string, string>? envVars = null) :
        this(new ProcessStartInfo(processPath.FullName, args) { WorkingDirectory = workingDirectory.FullName }, envVars)
    {
        ArgumentNullException.ThrowIfNull(processPath, nameof(processPath));
        ArgumentNullException.ThrowIfNull(workingDirectory, nameof(workingDirectory));
    }

    /// <summary>
    /// Gets the process Id.
    /// </summary>
    public int Id => _process.Id;

    /// <summary>
    /// Gets the value that the associated process specified when it terminated.
    /// </summary>
    public int ExitCode => _process.ExitCode;

    /// <summary>
    /// Gets the time that the associated process exited.
    /// </summary>
    public DateTime ExitTime => _process.ExitTime;

    /// <summary>
    /// Gets the time that the associated process was started.
    /// </summary>
    public DateTime StartTime => _process.StartTime;

    /// <summary>
    /// Gets a value indicating whether the associated process has been terminated.
    /// </summary>
    public bool HasExited => _process.HasExited;

    /// <summary>
    /// Gets the execution time of the process.
    /// </summary>
    public TimeSpan ExecutionTime => _process.ExitTime - _process.StartTime;

    /// <summary>
    /// Starts the process and publishes output and error lines if any as events.
    /// </summary>
    public ChannelReader<ProcessOutputLine> Start(CancellationToken cToken = default)
    {
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        _ = Task.Run(async () =>
        {
            try
            {
                await _process.WaitForExitAsync(cToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _process.Kill(true);
            }
            finally
            {
                _outputChannel.Writer.Complete();
            }
        }, cToken);

        return _outputChannel.Reader;
    }

    /// <summary>
    /// Releases all the resources used by this instance.
    /// </summary>
    public void Dispose() => _process.Dispose();

    private void OnOutputData(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            ProcessOutputLine line = new(DateTimeOffset.Now, e.Data, false);
            _outputChannel.Writer.TryWrite(line);
        }
    }

    private void OnErrorData(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            ProcessOutputLine line = new(DateTimeOffset.Now, e.Data, true);
            _outputChannel.Writer.TryWrite(line);
        }
    }
}

/// <summary>
/// An abstraction for representing the output of a process started by <see cref="EasyProcess"/>.
/// </summary>
public readonly record struct ProcessOutputLine(DateTimeOffset Timestamp, string Value, bool IsError);