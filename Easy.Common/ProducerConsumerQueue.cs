namespace Easy.Common;

using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// An implementation of the <c>Producer/Consumer</c> pattern using <c>TPL</c>.
/// </summary>
/// <typeparam name="T">Type of the item to produce/consume</typeparam>
public sealed class ProducerConsumerQueue<T> : IDisposable
{
    private readonly BlockingCollection<T> _queue;

    /// <summary>
    /// Creates an unbounded instance of <see cref="ProducerConsumerQueue{T}"/>.
    /// </summary>
    /// <param name="consumer">The action to be executed when consuming the queued items</param>
    /// <param name="maxConcurrencyLevel">Maximum number of consumers</param>
    public ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel)
        : this(consumer, maxConcurrencyLevel, -1) { }

    /// <summary>
    /// Creates an instance of <see cref="ProducerConsumerQueue{T}"/>.
    /// </summary>
    /// <param name="consumer">The action to be executed when consuming the queued items</param>
    /// <param name="maxConcurrencyLevel">Maximum number of consumers</param>
    /// <param name="boundedCapacity">
    /// The bounded capacity of the queue. Any more items added will block the publisher 
    /// until there is more space available.
    /// </param>
    public ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel, uint boundedCapacity)
        : this(consumer, maxConcurrencyLevel, (int)boundedCapacity) { }

    private ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel, int boundedCapacity)
    {
        Ensure.NotNull(consumer, nameof(consumer));
        Ensure.That(maxConcurrencyLevel > 0, $"{nameof(maxConcurrencyLevel)} should be greater than zero.");
        Ensure.That(boundedCapacity != 0, $"{nameof(boundedCapacity)} should be greater than zero.");

        _queue = boundedCapacity < 0 ? new BlockingCollection<T>() : new BlockingCollection<T>(boundedCapacity);

        MaximumConcurrencyLevel = maxConcurrencyLevel;
        Completion = Configure(consumer);
    }

    /// <summary>
    /// Gets the maximum number of consumer threads.
    /// </summary>
    public uint MaximumConcurrencyLevel { get; }

    /// <summary>
    /// Gets the bounded capacity of the underlying queue. -1 for unbounded.
    /// </summary>
    public int Capacity => _queue.BoundedCapacity;

    /// <summary>
    /// Gets the count of items that are pending consumption.
    /// </summary>
    public uint PendingCount => (uint)_queue.Count;

    /// <summary>
    /// Gets the pending items in the queue. 
    /// <remarks>
    /// Note, the items are valid as a snapshot at the time of invocation.
    /// </remarks>
    /// </summary>
    public T[] PendingItems => _queue.ToArray();

    /// <summary>
    /// Gets the <see cref="Task"/> which completes when all the consumers have finished their work.
    /// </summary>
    public Task<bool> Completion { get; }

    /// <summary>
    /// Thrown when an error occurs during the consumption or publication of items.
    /// </summary>
    public event EventHandler<ProducerConsumerQueueException>? OnException;

    /// <summary>
    /// Adds the specified item to the <see cref="ProducerConsumerQueue{T}"/>. 
    /// This method blocks if the queue is full and until there is more room.
    /// </summary>
    /// <param name="item">The item to be added to the collection. The value can be a null reference.</param>
    public void Add(T item) => Add(item, CancellationToken.None);

    /// <summary>
    /// Adds the specified item to the <see cref="ProducerConsumerQueue{T}"/>. 
    /// This method blocks if the queue is full and until there is more room.
    /// </summary>
    /// <param name="item">The item to be added to the collection. The value can be a null reference.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public void Add(T item, CancellationToken cancellationToken)
    {
        try
        {
            _queue.Add(item, cancellationToken);
        } catch (Exception e)
        {
            OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred when adding item.", e));
        }
    }

    /// <summary>
    /// Tries to add the specified item to the <see cref="ProducerConsumerQueue{T}"/>.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <returns>
    /// <c>True</c> if <paramref name="item"/> could be added to the collection within the specified time, 
    /// otherwise <c>False</c>. If the item is a duplicate, and the underlying collection does 
    /// not accept duplicate items, then an <see cref="InvalidOperationException"/> is thrown wrapped 
    /// in a <see cref="ProducerConsumerQueueException"/>.
    /// </returns>
    public bool TryAdd(T item) => TryAdd(item, TimeSpan.Zero, CancellationToken.None);

    /// <summary>
    /// Tries to add the specified item to the <see cref="ProducerConsumerQueue{T}"/> within the specified time period.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <param name="timeout">Represents the time to wait.</param>
    /// <returns>
    /// <c>True</c> if <paramref name="item"/> could be added to the collection within the specified time, 
    /// otherwise <c>False</c>. If the item is a duplicate, and the underlying collection does 
    /// not accept duplicate items, then an <see cref="InvalidOperationException"/> is thrown wrapped 
    /// in a <see cref="ProducerConsumerQueueException"/>.
    /// </returns>
    public bool TryAdd(T item, TimeSpan timeout) => TryAdd(item, timeout, CancellationToken.None);

    /// <summary>
    /// Tries to add the specified item to the <see cref="ProducerConsumerQueue{T}"/> within the specified time period.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    /// <param name="timeout">Represents the time to wait.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>
    /// <c>True</c> if <paramref name="item"/> could be added to the collection within the specified time, 
    /// otherwise <c>False</c>. If the item is a duplicate, and the underlying collection does 
    /// not accept duplicate items, then an <see cref="InvalidOperationException"/> is thrown wrapped 
    /// in a <see cref="ProducerConsumerQueueException"/>.
    /// </returns>
    public bool TryAdd(T item, TimeSpan timeout, CancellationToken cancellationToken)
    {
        try
        {
            return _queue.TryAdd(item, (int)timeout.TotalMilliseconds, cancellationToken);
        }
        catch (Exception e)
        {
            OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred when adding item.", e));
            return false;
        }
    }

    /// <summary>
    /// Marks the <see cref="ProducerConsumerQueue{T}"/> instance as not accepting any new items.
    /// </summary>
    public void CompleteAdding() => _queue.CompleteAdding();

    /// <summary>
    /// Releases all the resources used by this instance.
    /// </summary>
    public void Dispose() => _queue.Dispose();

    private Task<bool> Configure(Action<T> consumer)
    {
        var workers = Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(
                    Partitioner.Create(
                        _queue.GetConsumingEnumerable(), 
                        EnumerablePartitionerOptions.NoBuffering), 
                    new ParallelOptions { MaxDegreeOfParallelism = (int)MaximumConcurrencyLevel }, 
                    WrapConsumer);
            }, 
            CancellationToken.None, 
            TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, 
            TaskScheduler.Default);

        var tcs = new TaskCompletionSource<bool>();
        workers.ContinueWith(_ => tcs.SetResult(false), 
            TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

        workers.ContinueWith(_ => tcs.SetResult(true), 
            TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

        return tcs.Task;

        void WrapConsumer(T x)
        {
            try
            {
                consumer(x);
            } catch (Exception e)
            {
                OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred.", e));
            }
        }
    }
}

/// <summary>
/// The <see cref="Exception"/> thrown by the <see cref="ProducerConsumerQueue{T}"/>.
/// </summary>
[Serializable]
public sealed class ProducerConsumerQueueException : Exception
{
    /// <summary>
    /// Creates an instance of the <see cref="ProducerConsumerQueueException"/>.
    /// </summary>
    internal ProducerConsumerQueueException() { }

    /// <summary>
    /// Creates an instance of the <see cref="ProducerConsumerQueueException"/>.
    /// </summary>
    /// <param name="message">The message for the <see cref="Exception"/></param>
    internal ProducerConsumerQueueException(string message) : base(message) { }

    /// <summary>
    /// Creates an instance of the <see cref="ProducerConsumerQueueException"/>.
    /// </summary>
    /// <param name="message">The message for the <see cref="Exception"/></param>
    /// <param name="innerException">The inner exception</param>
    internal ProducerConsumerQueueException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Creates an instance of the <see cref="ProducerConsumerQueueException"/>.
    /// </summary>
    /// <param name="info">The serialization information</param>
    /// <param name="context">The streaming context</param>
    internal ProducerConsumerQueueException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}