namespace Easy.Common.Interfaces
{
    using System;

    /// <summary>
    /// Provides the contract for representing a clock.
    /// </summary>
    public interface IClock : IDisposable
    {
        /// <summary>
        /// Gets the flag indicating whether the instance of <see cref="IClock"/> provides high resolution time.
        /// <remarks>
        /// <para>
        /// This only returns <c>True</c> on <c>Windows 8</c>/<c>Windows Server 2012</c> and higher.
        /// </para>
        /// </remarks>
        /// </summary>
        bool IsPrecise { get; }

        /// <summary>
        /// Gets the date and time in <c>UTC</c>.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the local date time.
        /// </summary>
        DateTime Now { get; }
    }
}