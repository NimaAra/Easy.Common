namespace Easy.Common.Extensions
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides a set of helper methods for working with <see cref="byte"/>.
    /// </summary>
    public static class ByteExtensions
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, long count);

        /// <summary>
        /// Compares the given <paramref name="left"/> with <paramref name="right"/>.
        /// </summary>
        /// <returns><c>True</c> if the two are equal otherwise <c>False</c></returns>
        public static bool Compare(this byte[] left, byte[] right)
        {
            Ensure.NotNull(left, nameof(left));
            Ensure.NotNull(right, nameof(right));
            
            // Validate buffers are the same length.
            // This also ensures that the count does not exceed the length of either buffer.  
            return left.Length == right.Length && memcmp(left, right, left.Length) == 0;
        }
    }
}