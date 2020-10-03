namespace Easy.Common
{
    using System;
    using System.Diagnostics;
    using Easy.Common.Interfaces;

    /// <summary>
    /// This class provides a high resolution clock by using the new API available in <c>Windows 8</c>/ 
    /// <c>Windows Server 2012</c> and higher.
    /// </summary>
    public sealed class Clock : IClock
    {
        /// <summary>
        /// Returns the single instance of the <see cref="Clock"/>.
        /// </summary>
        public static Clock Instance { get; } = new Clock();

        /// <summary>
        /// Creates an instance of the <see cref="Clock"/>.
        /// </summary>
        [DebuggerStepThrough]
        private Clock()
        {
            try
            {
                NativeMethods.GetSystemTimePreciseAsFileTime(out _);
                IsPrecise = true;
            }
            catch (Exception e) when (e is EntryPointNotFoundException || e is DllNotFoundException)
            {
                IsPrecise = false;
            }
        }

        /// <summary>
        /// Gets the flag indicating whether the instance of <see cref="Clock"/> provides high resolution time.
        /// <remarks>
        /// <para>
        /// Only returns <c>True</c> on <c>Windows 8</c>/<c>Windows Server 2012</c> and higher.
        /// </para>
        /// </remarks>
        /// </summary>
        public bool IsPrecise { get; }

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> object that is set to the current date and time on the current computer,
        /// with the offset set to the local time's offset from Coordinated Universal Time (UTC).
        /// </summary>
        public DateTimeOffset Now
        {
            get
            {
                if (IsPrecise)
                {
                    NativeMethods.GetSystemTimePreciseAsFileTime(out long preciseTime);
                    return DateTimeOffset.FromFileTime(preciseTime);
                }

                return DateTimeOffset.Now;
            }
        }
    }

    /// <summary>
    /// This class provides a fake clock to be used for testing of cases when an <see cref="IClock"/> is used.
    /// </summary>
    public sealed class FakeClock : IClock
    {
        /// <summary>
        /// Creates an instance of the <see cref="FakeClock"/>.
        /// </summary>
        public FakeClock(DateTimeOffset offset, bool isPrecise = true)
        {
            Now = offset;
            IsPrecise = isPrecise;
        }

        /// <summary>
        /// Gets the flag indicating whether the instance of <see cref="FakeClock"/> provides high resolution time.
        /// </summary>
        public bool IsPrecise {get; }

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> object that is set to the current date and time on the current computer,
        /// with the offset set to the local time's offset from Coordinated Universal Time (UTC).
        /// </summary>
        public DateTimeOffset Now { get; }
    }
}