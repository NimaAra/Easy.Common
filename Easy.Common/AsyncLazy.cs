namespace Easy.Common;

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

/// <summary>
/// An abstraction for representing asynchronous initialization.
/// <see href="https://devblogs.microsoft.com/pfxteam/asynclazyt/"/>
/// </summary>
public sealed class AsyncLazy<T> : Lazy<Task<T>>
{
    /// <summary>
    /// Creates an instance of the <see cref="AsyncLazy{T}"/>.
    /// </summary>
    public AsyncLazy(Func<T> valueFactory) :
        base(() => Task.Factory.StartNew(valueFactory))
    { }

    /// <summary>
    /// Creates an instance of the <see cref="AsyncLazy{T}"/>.
    /// </summary>
    public AsyncLazy(Func<Task<T>> taskFactory) :
        base(() => Task.Factory.StartNew(taskFactory).Unwrap())
    { }

    /// <summary>
    /// Gets an awaiter used to await the result.
    /// </summary>
    /// <returns></returns>
    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}