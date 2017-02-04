namespace Easy.Common.Extensions
{
    /// <summary>
    /// Provides a set of helper methods for working with <see cref="byte"/>.
    /// </summary>
    public static class ByteExtensions
    {
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
            return left.Length == right.Length && NativeMethods.MemCopy(left, right, left.Length) == 0;
        }
    }
}