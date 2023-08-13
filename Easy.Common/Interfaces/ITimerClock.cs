namespace Easy.Common.Interfaces;

using System;

/// <summary>
/// An interface representing a clock which ticks at a given interval.
/// </summary>
public interface ITimerClock : IDisposable
{
    /// <summary>
    /// Gets the event raised on every tick of the clock.
    /// </summary>
    event EventHandler<EventArgs> Tick;

    /// <summary>
    /// Gets the clock used by this instance of <see cref="ITimerClock"/>.
    /// </summary>
    IClock Clock { get; }

    /// <summary>
    /// Gets the frequency at which the <see cref="Tick"/> is raised.
    /// </summary>
    TimeSpan TickInterval { get; }

    /// <summary>
    /// Gets the flag indicating whether the clock is enabled or not.
    /// </summary>
    bool Enabled { get; set; }
}