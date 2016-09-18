namespace Easy.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of the <c>Producer/Consumer</c> pattern using <c>TPL</c>.
    /// </summary>
    /// <typeparam name="T">Type of the item to produce/consume</typeparam>
    public sealed class ProducerConsumerQueue<T>
    {
        private readonly BlockingCollection<T> _queue;
        private readonly CancellationTokenSource _shutDownCancellationTokenSource;
        private bool _isShutdownRequested;
        private readonly Task[] _tasks;

        /// <summary>
        /// Creates an unbounded instance of <see cref="ProducerConsumerQueue{T}"/>
        /// </summary>
        /// <param name="consumer">The action to be executed when consuming the item.</param>
        /// <param name="maxConcurrencyLevel">Maximum number of consumers.</param>
        public ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel)
            : this(consumer, maxConcurrencyLevel, -1) { }

        /// <summary>
        /// Creates an instance of <see cref="ProducerConsumerQueue{T}"/>
        /// </summary>
        /// <param name="consumer">The action to be executed when consuming the item.</param>
        /// <param name="maxConcurrencyLevel">Maximum number of consumers.</param>
        /// <param name="boundedCapacity">The bounded capacity of the queue.
        /// Any more items added will block until there is more space available.
        /// For unbounded enter a negative number</param>
        public ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel, uint boundedCapacity)
            : this(consumer, maxConcurrencyLevel, (int)boundedCapacity) { }

        private ProducerConsumerQueue(Action<T> consumer, uint maxConcurrencyLevel, int boundedCapacity)
        {
            Ensure.NotNull(consumer, nameof(consumer));
            Ensure.That(maxConcurrencyLevel > 0, $"{nameof(maxConcurrencyLevel)} should be greater than zero.");
            Ensure.That(boundedCapacity != 0, $"{nameof(boundedCapacity)} should be greater than zero.");

            WorkerCount = maxConcurrencyLevel;

            _queue = boundedCapacity == -1 ? new BlockingCollection<T>() : new BlockingCollection<T>(boundedCapacity);
            _shutDownCancellationTokenSource = new CancellationTokenSource();

            _tasks = SetupConsumer(consumer, maxConcurrencyLevel).ToArray();
        }

        /// <summary>
        /// Gets the number of consumer threads.
        /// </summary>
        public uint WorkerCount { get; }

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
        /// Note, the items are valid as the snapshot at the time of invocation.
        /// </remarks>
        /// </summary>
        public T[] PendingItems => _queue.ToArray();

        /// <summary>
        /// Gets whether <see cref="ProducerConsumerQueue{T}"/> has started to shutdown.
        /// </summary>
        public bool ShutdownRequested => Volatile.Read(ref _isShutdownRequested);

        /// <summary>
        /// Thrown when an error occurs during consumption of the work.
        /// </summary>
        public event EventHandler<ProducerConsumerQueueException> OnException;

        /// <summary>
        /// Adds the specified item to the <see cref="ProducerConsumerQueue{T}"/>. 
        /// This method blocks if the queue is full and until there is more room.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void Add(T item)
        {
            try
            {
                _queue.Add(item);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred when adding item.", e));
            }
        }

        /// <summary>
        /// Attempts to add the specified item to the <see cref="ProducerConsumerQueue{T}"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>
        /// <c>True</c> if item could be added; otherwise <c>False</c>. 
        /// If the item is a duplicate, and the underlying collection does 
        /// not accept duplicate items, then an InvalidOperationException is thrown.
        /// </returns>
        public bool TryAdd(T item)
        {
            try
            {
                return _queue.TryAdd(item);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred when adding item.", e));
                return false;
            }
        }

        /// <summary>
        /// Attempts to add the specified item to the <see cref="ProducerConsumerQueue{T}"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="timeout">
        /// A <c>TimeSpan</c> that represents the time to wait before giving up.
        /// </param>
        /// <returns>
        /// <c>True</c> if the item could be added to the collection within the specified time span; otherwise, <c>False</c>.
        /// </returns>
        public bool TryAdd(T item, TimeSpan timeout)
        {
            try
            {
                return _queue.TryAdd(item, timeout);
            }
            catch (Exception e)
            {
                OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred when adding item.", e));
                return false;
            }
        }

        /// <summary>
        /// Marks the <see cref="ProducerConsumerQueue{T}"/> instance as not accepting 
        /// any new items. Any outstanding items will be consumed for as long as <paramref name="waitFor"/>.
        /// </summary>
        /// <param name="waitFor">The maximum time to wait for any pending items to be processed.</param>
        /// <returns>Flag indicating whether all the workers were shutdown and dismantled in time.</returns>
        public bool Shutdown(TimeSpan waitFor)
        {
            Volatile.Write(ref _isShutdownRequested, true);
            _queue.CompleteAdding();
            var finishedInTime = Task.WaitAll(_tasks, waitFor);
            
            _shutDownCancellationTokenSource.Cancel();
            _shutDownCancellationTokenSource.Dispose();
            _queue.Dispose();
            return finishedInTime;
        }
        
        private IEnumerable<Task> SetupConsumer(Action<T> consumer, uint maximumConcurrencyLevel)
        {
            var cToken = _shutDownCancellationTokenSource.Token;

            for (var i = 0; i < maximumConcurrencyLevel; i++)
            {
                yield return Task.Factory.StartNew(() =>
                {
                    foreach (var item in _queue.GetConsumingEnumerable(cToken))
                    {
                        cToken.ThrowIfCancellationRequested();
                        try
                        {
                            consumer(item);
                        }
                        catch (Exception e)
                        {
                            OnException?.Invoke(this, new ProducerConsumerQueueException("Exception occurred.", e));
                        }
                    }
                }, cToken, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
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
}