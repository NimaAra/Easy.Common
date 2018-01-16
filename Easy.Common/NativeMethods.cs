namespace Easy.Common
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains all the native methods used by this library.
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        internal static extern int UuidCreateSequential(out Guid guid);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetSystemTimePreciseAsFileTime")]
        internal static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "memcmp")]
        internal static extern int MemoryCompare(byte[] b1, byte[] b2, long count);
    }
}