namespace Easy.Common
{
    using System;

    /// <summary>
    /// Provides a set of methods to help work with <see cref="Guid"/>.
    /// </summary>
    public static class GuidHelper
    {
        // see: 
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Creates a sequential UUID.
        /// <remarks>
        /// This is used by <c>MSSQL</c> and it is much faster than using <see cref="Guid.NewGuid()"/>.
        /// <see Href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa379322%28v=vs.85%29.aspx"/>
        /// </remarks>
        /// </summary>
        public static Guid CreateSequentialUUID()
        {
            const int RpcSOk = 0;

            var result = NativeMethods.UuidCreateSequential(out Guid guid);
            return result == RpcSOk ? guid : Guid.NewGuid();
        }

        /// <summary>
        /// Generates a sequential Guid.
        /// </summary>
        /// <remarks>
        /// Taken from the NHibernate project, original contribution of Donald Mull.
        /// See: https://github.com/nhibernate/nhibernate-core/blob/5e71e83ac45439239b9028e6e87d1a8466aba551/src/NHibernate/Id/GuidCombGenerator.cs
        /// </remarks>
        public static Guid CreateComb()
        {
            var guidArray = CreateSequentialUUID().ToByteArray();

            var baseDate = new DateTime(0x76c, 1, 1);
            var now = DateTime.UtcNow;

            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array 
            // SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the Guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}