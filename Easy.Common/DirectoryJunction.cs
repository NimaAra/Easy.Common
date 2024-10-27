﻿using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace Easy.Common;

/// <summary>
/// An abstraction to provide access for the NTFS junction points.
/// </summary>
public static class DirectoryJunction
{
    /// <summary>
    /// Creates a junction point from the specified directory to the specified target directory.
    /// </summary>
    /// <remarks>
    /// Only works on NTFS.
    /// </remarks>
    /// <param name="junctionPoint">The junction point path</param>
    /// <param name="targetDir">The target directory</param>
    /// <param name="overwrite">If true overwrites an existing reparse point or empty directory</param>
    /// <exception cref="IOException">Thrown when the junction point could not be created or when
    /// an existing directory was found and <paramref name="overwrite" /> if false</exception>
    public static void Create(string junctionPoint, string targetDir, bool overwrite)
    {
        targetDir = Path.GetFullPath(targetDir);

        if (!Directory.Exists(targetDir))
        {
            throw new IOException("Target path does not exist or is not a directory.");
        }

        if (Directory.Exists(junctionPoint))
        {
            if (!overwrite)
            {
                throw new IOException("Directory already exists and overwrite parameter is false.");
            }
        }
        else
        {
            Directory.CreateDirectory(junctionPoint);
        }

        using SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite);
        byte[] targetDirBytes = System.Text.Encoding.Unicode.GetBytes(NonInterpretedPathPrefix + Path.GetFullPath(targetDir));

        REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

        reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
        reparseDataBuffer.ReparseDataLength = (ushort)(targetDirBytes.Length + 12);
        reparseDataBuffer.SubstituteNameOffset = 0;
        reparseDataBuffer.SubstituteNameLength = (ushort)targetDirBytes.Length;
        reparseDataBuffer.PrintNameOffset = (ushort)(targetDirBytes.Length + 2);
        reparseDataBuffer.PrintNameLength = 0;
        reparseDataBuffer.PathBuffer = new byte[0x3ff0];
        Array.Copy(targetDirBytes, reparseDataBuffer.PathBuffer, targetDirBytes.Length);

        int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
        IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);

        try
        {
            Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

            bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_SET_REPARSE_POINT,
                inBuffer, targetDirBytes.Length + 20, IntPtr.Zero, 0, out _, IntPtr.Zero);

            if (!result)
            {
                ThrowLastWin32Error("Unable to create junction point.");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(inBuffer);
        }
    }

    /// <summary>
    /// Deletes a junction point at the specified source directory along with the directory itself.
    /// Does nothing if the junction point does not exist.
    /// </summary>
    /// <remarks>
    /// Only works on NTFS.
    /// </remarks>
    /// <param name="junctionPoint">The junction point path</param>
    public static void Delete(string junctionPoint)
    {
        if (!Directory.Exists(junctionPoint))
        {
            if (File.Exists(junctionPoint))
            {
                throw new IOException("Path is not a junction point.");
            }

            return;
        }

        using SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericWrite);
        REPARSE_DATA_BUFFER reparseDataBuffer = default;

        reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
        reparseDataBuffer.ReparseDataLength = 0;
        reparseDataBuffer.PathBuffer = new byte[0x3ff0];

        int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
        IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);
        try
        {
            Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

            bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
                inBuffer, 8, IntPtr.Zero, 0, out _, IntPtr.Zero);

            if (!result)
            {
                ThrowLastWin32Error("Unable to delete junction point.");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(inBuffer);
        }

        try
        {
            Directory.Delete(junctionPoint);
        }
        catch (IOException ex)
        {
            throw new IOException("Unable to delete junction point.", ex);
        }
    }

    /// <summary>
    /// Determines whether the specified path exists and refers to a junction point.
    /// </summary>
    /// <param name="path">The junction point path</param>
    /// <returns>True if the specified path represents a junction point</returns>
    /// <exception cref="IOException">Thrown if the specified path is invalid
    /// or some other error occurs</exception>
    public static bool Exists(string path)
    {
        if (!Directory.Exists(path))
        {
            return false;
        }

        using SafeFileHandle handle = OpenReparsePoint(path, EFileAccess.GenericRead);
        return InternalGetTarget(handle).Length > 0;
    }

    /// <summary>
    /// Gets the target of the specified junction point.
    /// </summary>
    /// <remarks>
    /// Only works on NTFS.
    /// </remarks>
    /// <param name="junctionPoint">The junction point path</param>
    /// <returns>The target of the junction point</returns>
    /// <exception cref="IOException">Thrown when the specified path does not
    /// exist, is invalid, is not a junction point, or some other error occurs</exception>
    public static string GetTarget(string junctionPoint)
    {
        using SafeFileHandle handle = OpenReparsePoint(junctionPoint, EFileAccess.GenericRead);
        string target = InternalGetTarget(handle);
        if (target.Length == 0)
        {
            throw new IOException("Path is not a junction point.");
        }

        return target;
    }

    private static string InternalGetTarget(SafeFileHandle handle)
    {
        int outBufferSize = Marshal.SizeOf(typeof(REPARSE_DATA_BUFFER));
        IntPtr outBuffer = Marshal.AllocHGlobal(outBufferSize);

        try
        {
            bool result = DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT, IntPtr.Zero, 0,
                outBuffer, outBufferSize, out _, IntPtr.Zero);

            if (!result)
            {
                int error = Marshal.GetLastWin32Error();
                if (error == ERROR_NOT_A_REPARSE_POINT)
                {
                    return string.Empty;
                }

                ThrowLastWin32Error("Unable to get information about junction point.");
            }

            REPARSE_DATA_BUFFER reparseDataBuffer = (REPARSE_DATA_BUFFER)
                Marshal.PtrToStructure(outBuffer, typeof(REPARSE_DATA_BUFFER))!;

            if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT)
            {
                return string.Empty;
            }

            string targetDir = System.Text.Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
                reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);

            if (targetDir.StartsWith(NonInterpretedPathPrefix))
            {
                targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);
            }

            return targetDir;
        }
        finally
        {
            Marshal.FreeHGlobal(outBuffer);
        }
    }

    private static SafeFileHandle OpenReparsePoint(string reparsePoint, EFileAccess accessMode)
    {
        SafeFileHandle reparsePointHandle = new(
            CreateFile(reparsePoint, accessMode, EFileShare.Read | EFileShare.Write | EFileShare.Delete, IntPtr.Zero,
                ECreationDisposition.OpenExisting, EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, IntPtr.Zero), true);

        if (Marshal.GetLastWin32Error() != 0)
        {
            ThrowLastWin32Error("Unable to open reparse point.");
        }

        return reparsePointHandle;
    }

    private static void ThrowLastWin32Error(string message) =>
        throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

    /// <summary>
    /// The file or directory is not a reparse point.
    /// </summary>
    private const int ERROR_NOT_A_REPARSE_POINT = 4390;

    /// <summary>
    /// The reparse point attribute cannot be set because it conflicts with an existing attribute.
    /// </summary>
    private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

    /// <summary>
    /// The data present in the reparse point buffer is invalid.
    /// </summary>
    private const int ERROR_INVALID_REPARSE_DATA = 4392;

    /// <summary>
    /// The tag present in the reparse point buffer is invalid.
    /// </summary>
    private const int ERROR_REPARSE_TAG_INVALID = 4393;

    /// <summary>
    /// There is a mismatch between the tag specified in the request and the tag present in the reparse point.
    /// </summary>
    private const int ERROR_REPARSE_TAG_MISMATCH = 4394;

    /// <summary>
    /// Command to set the reparse point data block.
    /// </summary>
    private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

    /// <summary>
    /// Command to get the reparse point data block.
    /// </summary>
    private const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

    /// <summary>
    /// Command to delete the reparse point data base.
    /// </summary>
    private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

    /// <summary>
    /// Reparse point tag used to identify mount points and junction points.
    /// </summary>
    private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

    /// <summary>
    /// This prefix indicates to NTFS that the path is to be treated as a non-interpreted
    /// path in the virtual file system.
    /// </summary>
    private const string NonInterpretedPathPrefix = @"\??\";

    private enum ECreationDisposition : uint
    {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5,
    }

    [Flags]
    private enum EFileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,
    }

    [Flags]
    private enum EFileShare : uint
    {
        None = 0x00000000,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004,
    }

    [Flags]
    private enum EFileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        Write_Through = 0x80000000,
        Overlapped = 0x40000000,
        NoBuffering = 0x20000000,
        RandomAccess = 0x10000000,
        SequentialScan = 0x08000000,
        DeleteOnClose = 0x04000000,
        BackupSemantics = 0x02000000,
        PosixSemantics = 0x01000000,
        OpenReparsePoint = 0x00200000,
        OpenNoRecall = 0x00100000,
        FirstPipeInstance = 0x00080000
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct REPARSE_DATA_BUFFER
    {
        /// <summary>
        /// Reparse point tag. Must be a Microsoft reparse point tag.
        /// </summary>
        public uint ReparseTag;

        /// <summary>
        /// Size, in bytes, of the data after the Reserved member. This can be calculated by:
        /// (4 * sizeof(ushort)) + SubstituteNameLength + PrintNameLength + 
        /// (namesAreNullTerminated ? 2 * sizeof(char) : 0);
        /// </summary>
        public ushort ReparseDataLength;

        /// <summary>
        /// Reserved; do not use. 
        /// </summary>
        public ushort Reserved;

        /// <summary>
        /// Offset, in bytes, of the substitute name string in the PathBuffer array.
        /// </summary>
        public ushort SubstituteNameOffset;

        /// <summary>
        /// Length, in bytes, of the substitute name string. If this string is null-terminated,
        /// SubstituteNameLength does not include space for the null character.
        /// </summary>
        public ushort SubstituteNameLength;

        /// <summary>
        /// Offset, in bytes, of the print name string in the PathBuffer array.
        /// </summary>
        public ushort PrintNameOffset;

        /// <summary>
        /// Length, in bytes, of the print name string. If this string is null-terminated,
        /// PrintNameLength does not include space for the null character. 
        /// </summary>
        public ushort PrintNameLength;

        /// <summary>
        /// A buffer containing the unicode-encoded path string. The path string contains
        /// the substitute name string and print name string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
        public byte[] PathBuffer;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
        IntPtr InBuffer, int nInBufferSize,
        IntPtr OutBuffer, int nOutBufferSize,
        out int pBytesReturned, IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFile(
        string lpFileName,
        EFileAccess dwDesiredAccess,
        EFileShare dwShareMode,
        IntPtr lpSecurityAttributes,
        ECreationDisposition dwCreationDisposition,
        EFileAttributes dwFlagsAndAttributes,
        IntPtr hTemplateFile);
}