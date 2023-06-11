namespace Easy.Common;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// An abstraction representing an <see cref="IDisposable"/> object which executes an action on disposal.
/// </summary>
public sealed class Disposable : IDisposable
{
    /// <summary>
    /// Creates a disposable that invokes the specified <paramref name="onDispose"/> upon disposal.
    /// </summary>
    /// <param name="onDispose">The action to execute during <see cref="IDisposable.Dispose"/>.</param>
    /// <returns>An <see cref="IDisposable"/> which represents the scope.</returns>
    public static IDisposable Create(Action onDispose) => new Disposable(onDispose);

    private Action? _onDispose;

    private Disposable(Action onDispose) => Interlocked.Exchange(ref _onDispose, onDispose);

    /// <summary>
    /// Disposes the instance and executes the provided logic.
    /// </summary>
    public void Dispose() => Interlocked.Exchange(ref _onDispose, null)?.Invoke();
}

/// <summary>
/// An abstraction representing an <see cref="IAsyncDisposable"/> object which executes an action on disposal.
/// </summary>
public sealed class AsyncDisposable : IAsyncDisposable
{
    /// <summary>
    /// Creates a disposable that invokes the specified <paramref name="onDispose"/> asynchronously upon disposal.
    /// </summary>
    /// <param name="onDispose">The action to execute during <see cref="IAsyncDisposable.DisposeAsync"/>.</param>
    /// <returns>An <see cref="IAsyncDisposable"/> which represents the scope.</returns>
    public static IAsyncDisposable Create(Func<ValueTask> onDispose) => new AsyncDisposable(onDispose);

    private Func<ValueTask>? _onDispose;
    
    private AsyncDisposable(Func<ValueTask> onDispose) => Interlocked.Exchange(ref _onDispose, onDispose);

    /// <summary>
    /// Disposes the instance and executes the provided logic.
    /// </summary>
    public ValueTask DisposeAsync() =>
        Interlocked.Exchange(ref _onDispose, null)?.Invoke() ?? ValueTask.CompletedTask;
}