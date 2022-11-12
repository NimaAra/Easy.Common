namespace Easy.Common;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides an abstraction to simplify executing a process asynchronously.
/// </summary>
public sealed class EasyProcess : IDisposable
{
    private readonly Process _process;
    
    private EasyProcess(ProcessStartInfo startInfo)
    {
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        
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
    public EasyProcess(string processPath, string args) : this(new ProcessStartInfo(processPath, args)) =>
        Ensure.NotNullOrEmptyOrWhiteSpace(processPath);

    /// <summary>
    /// Creates an instance of the <see cref="EasyProcess"/>.
    /// </summary>
    public EasyProcess(FileInfo processPath, string args) : this(new ProcessStartInfo(processPath.FullName, args)) =>
        Ensure.NotNull(processPath, nameof(processPath));

    /// <summary>
    /// Creates an instance of the <see cref="EasyProcess"/>.
    /// </summary>
    public EasyProcess(FileInfo processPath, DirectoryInfo workingDirectory, string args) : 
        this(new ProcessStartInfo(processPath.FullName, args) { WorkingDirectory = workingDirectory.FullName })
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
    /// Raised on every process output line.
    /// </summary>
    public event EventHandler<string> OnOutput;
    
    /// <summary>
    /// Raised on every process error line.
    /// </summary>
    public event EventHandler<string> OnError;

    /// <summary>
    /// Starts the process and publishes output and error lines if any as events.
    /// </summary>
    public async Task Start(CancellationToken cToken = default)
    {
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        try
        {
            await _process.WaitForExitAsync(cToken).ConfigureAwait(false);
        } catch (TaskCanceledException)
        {
            _process.Kill(true);
        }
    }

    /// <summary>
    /// Releases all the resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        _process.OutputDataReceived -= OnOutputData;
        _process.ErrorDataReceived -= OnErrorData;
        _process.Dispose();
    }

    private void OnOutputData(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            OnOutput?.Invoke(this, e.Data);
        }
    }

    private void OnErrorData(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not null)
        {
            OnError?.Invoke(this, e.Data);
        }
    }
}