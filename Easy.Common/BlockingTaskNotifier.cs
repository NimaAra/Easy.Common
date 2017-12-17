namespace Easy.Common
{
    using System;
    using System.Diagnostics.Tracing;
    using System.Threading.Tasks;

    /// <summary>
    /// An abstraction for notifying when an incomplete <see cref="Task"/> is blocked.
    /// </summary>
    public sealed class BlockingTaskNotifier : EventListener
    {
        private static readonly Guid Guid = new Guid("2e5dba47-a3d2-4d16-8ee0-6671ffdcd7b5");

        private static readonly Lazy<BlockingTaskNotifier> Listener
            = new Lazy<BlockingTaskNotifier>(() => new BlockingTaskNotifier());

        private BlockingTaskNotifier() { }

        /// <summary>
        /// Invoked when an incomplete task is blocked.
        /// </summary>
        public static event EventHandler<string> OnDetection;

        /// <summary>
        /// Starts the alerter.
        /// </summary>
        public static void Start()
        {
            var _ = Listener.Value;
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Guid == Guid)
            {
                // 3 == Task|TaskTransfer
                EnableEvents(eventSource, EventLevel.Informational, (EventKeywords) 3);
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventId == 10 && // TASKWAITBEGIN_ID
                eventData.Payload != null &&
                eventData.Payload.Count > 3 &&
                eventData.Payload[3] is int value && // Behavior
                value == 1) // TaskWaitBehavior.Synchronous
            {
                OnDetection?.Invoke(this, Environment.StackTrace);
            }
        }
    }
}