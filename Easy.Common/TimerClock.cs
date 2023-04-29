namespace Easy.Common;

using Easy.Common.Interfaces;
using System;
using System.Threading;

/// <summary>
/// A class representing a clock which ticks at a given interval.
/// </summary>
public sealed class TimerClock : ITimerClock
{
    private DateTimeOffset _nextSchedule;
    private DateTimeOffset NextSchedule
    {
        get
        {
            Thread.MemoryBarrier();
            return _nextSchedule;
        }
        set
        {
            _nextSchedule = value;
            Thread.MemoryBarrier();
        }
    }

    /// <summary>
    /// Gets the clock used by this instance of <see cref="TimerClock"/>.
    /// </summary>
    public IClock Clock { get; }

    /// <summary>
    /// The event is raised at every clock tick specified by the <see cref="TickInterval"/>.
    /// </summary>
    public event EventHandler<EventArgs>? Tick;

    /// <summary>
    /// Gets the interval at which to raise the <see cref="Tick"/> event.
    /// </summary>
    public TimeSpan TickInterval { get; }

    /// <summary>
    /// Creates an instance of the <see cref="TimerClock"/>.
    /// </summary>
    /// <param name="interval">The interval at which to raise the <see cref="Tick"/> event</param>
    public TimerClock(TimeSpan interval)
    {
        Clock = Easy.Common.Clock.Instance;
        TickInterval = interval;
        NextSchedule = DateTimeOffset.MaxValue;
        InternalTimer.Tick += OnTick;
    }

    /// <summary>
    /// Enables or disables the clock which controls whether 
    /// the <see cref="Tick"/> should be raised.
    /// <remarks>
    /// <para>
    /// Note this does not <c>Pause</c> the clock but resets the timer every time it is set to <c>True</c>.
    /// </para>
    /// </remarks>
    /// </summary>
    public bool Enabled
    {
        get => NextSchedule != DateTimeOffset.MaxValue;
        set => NextSchedule = value ? Clock.Now.Add(TickInterval) : DateTimeOffset.MaxValue;
    }

    /// <summary>
    /// The dispose
    /// </summary>
    public void Dispose()
    {
        NextSchedule = DateTimeOffset.MaxValue;
        InternalTimer.Tick -= OnTick;
    }

    /// <summary>
    /// Converts the value of the <see cref="TimerClock"/> to its equivalent <see cref="string"/>.
    /// </summary>
    /// <returns>The <see cref="string"/> representation of the <see cref="TimerClock"/></returns>
    public override string ToString() => 
        $"Interval: {TickInterval.ToString()} - DateTime: {Clock.Now.ToString("yyyy-MM-dd HH:mm.ss.fff")}";

    private void OnTick(object? sender, EventArgs args)
    {
        if (NextSchedule == DateTimeOffset.MaxValue) { return; }

        DateTimeOffset now = Clock.Now;
        if (now < NextSchedule) { return; }

        NextSchedule = now.Add(TickInterval);

        ThreadPool.UnsafeQueueUserWorkItem(_ => Tick?.Invoke(this, EventArgs.Empty), null);
    }

    private static class InternalTimer
    {
        internal static event EventHandler<EventArgs>? Tick;

        static InternalTimer()
        {
            AsyncFlowControl flowControl = ExecutionContext.SuppressFlow();
            new Thread(() =>
                {
                    var spinner = new SpinWait();
                    while (true)
                    {
                        spinner.SpinOnce();
                        Tick?.Invoke(null, EventArgs.Empty);
                    }
                    // ReSharper disable once FunctionNeverReturns
                })
                {
                    Priority = ThreadPriority.Highest,
                    Name = "InternalTimer",
                    IsBackground = true
                }
                .Start();
            flowControl.Undo();
        }
    }
}