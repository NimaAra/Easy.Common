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
    private DateTime? _startTime;
    private DateTime? _exitTime;

    private EasyProcess(ProcessStartInfo startInfo, IReadOnlyDictionary<string, string>? envVars)
    {
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        if (envVars is not null)
        {
            foreach (var pair in envVars)
            {
                startInfo.Environment[pair.Key] = pair.Value;
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
    /// <exception cref="InvalidOperationException">
    /// Thrown when the process has not exited yet.
    /// </exception>
    /// <remarks>
    /// On some platforms such as Unix, the exit time cannot be queried once the
    /// process has exited hence the value is captured when the process exits.
    /// </remarks>
    public DateTime ExitTime
    {
        get
        {
            if (_exitTime is { } captured) { return captured; }

            try
            {
                // Covers the case where the process has already exited
                // but its exit time is yet to be captured.
                return _process.ExitTime;
            }
            catch (Exception e) when (e is InvalidOperationException || e is System.ComponentModel.Win32Exception)
            {
                throw new InvalidOperationException("The process has not exited yet.", e);
            }
        }
    }

    /// <summary>
    /// Gets the time that the associated process was started.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the process has not been started yet.
    /// </exception>
    /// <remarks>
    /// On some platforms such as Unix, the start time cannot be queried once the process
    /// has exited hence the value is captured when the process starts.
    /// </remarks>
    public DateTime StartTime => _startTime ?? throw new InvalidOperationException("The process has not been started yet.");

    /// <summary>
    /// Gets a value indicating whether the associated process has been terminated.
    /// </summary>
    public bool HasExited => _process.HasExited;

    /// <summary>
    /// Gets the execution time of the process.
    /// </summary>
    public TimeSpan ExecutionTime => ExitTime - StartTime;

    /// <summary>
    /// Starts the process and publishes output and error lines if any as events.
    /// </summary>
    public ChannelReader<ProcessOutputLine> Start(CancellationToken cToken = default)
    {
        _process.Start();
        _startTime = GetStartTime();
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
                if (!_process.HasExited)
                {
                    _process.Kill(true);
                }
            }
            finally
            {
                _exitTime = GetExitTime();
                _outputChannel.Writer.TryComplete();
            }
        }, cToken);

        return _outputChannel.Reader;
    }

    /// <summary>
    /// Releases all the resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        _process.Dispose();
        _outputChannel.Writer.TryComplete();
    }

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

    private DateTime GetStartTime()
    {
        try
        {
            return _process.StartTime;
        }
        catch (Exception e) when (e is InvalidOperationException || e is System.ComponentModel.Win32Exception)
        {
            // On some platforms such as Unix the start time cannot be queried
            // once the process has exited which can happen for short-lived processes.
            return DateTime.Now;
        }
    }

    private DateTime GetExitTime()
    {
        try
        {
            return _process.ExitTime;
        }
        catch (Exception e) when (e is InvalidOperationException || e is System.ComponentModel.Win32Exception)
        {
            // On some platforms such as Unix the exit time cannot be queried
            // once the process has exited.
            return DateTime.Now;
        }
    }
}

/// <summary>
/// An abstraction for representing the output of a process started by <see cref="EasyProcess"/>.
/// </summary>
public readonly record struct ProcessOutputLine(DateTimeOffset Timestamp, string Value, bool IsError);