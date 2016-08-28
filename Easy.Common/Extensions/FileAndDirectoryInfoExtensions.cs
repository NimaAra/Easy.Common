namespace Easy.Common.Extensions
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides a set of useful methods for working with <see cref="FileInfo"/> and <see cref="DirectoryInfo"/>.
    /// </summary>
    public static class FileAndDirectoryInfoExtensions
    {
        /// <summary>
        /// Returns the size in bytes of the <paramref name="directoryInfo"/> represented by the <paramref name="directoryInfo"/> instance.
        /// </summary>
        /// <param name="directoryInfo">The <paramref name="directoryInfo"/> to get the size of.</param>
        /// <returns>The size of <paramref name="directoryInfo"/> in bytes.</returns>
        public static long GetSizeInByte(this DirectoryInfo directoryInfo)
        {
            long length = 0;

            foreach (var nextfile in directoryInfo.GetFiles())
            {
                length += nextfile.Exists ? nextfile.Length : 0;
            }

            foreach (var nextdir in directoryInfo.GetDirectories())
            {
                length += nextdir.Exists ? nextdir.GetSizeInByte() : 0;
            }
            return length;
        }

        /// <summary>
        /// Returns the size in bytes of a file at the given <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path to the file to get the size of.</param>
        /// <returns>The size of <paramref name="filePath"/> in bytes.</returns>
        public static long FileSize(this string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        /// <summary>
        /// Indicates if a given <paramref name="directoryInfo"/> is hidden.
        /// </summary>
        /// <param name="directoryInfo">The <paramref name="directoryInfo"/> to check.</param>
        /// <returns>Boolean indicating if the <paramref name="directoryInfo"/> is hidden.</returns>
        public static bool IsHidden(this DirectoryInfo directoryInfo)
        {
            return (directoryInfo.Attributes & FileAttributes.Hidden) != 0;
        }

        /// <summary>
        /// Indicates if a given <paramref name="fileInfo"/> is hidden.
        /// </summary>
        /// <param name="fileInfo">The <paramref name="fileInfo"/> to check.</param>
        /// <returns>
        /// Boolean indicating if the <paramref name="fileInfo"/> is hidden.
        /// </returns>
        public static bool IsHidden(this FileInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.Hidden) != 0;
        }

        /// <summary>
        /// Renames the given <paramref name="fileInfo"/> to <paramref name="newName"/>.
        /// </summary>
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            Ensure.NotNull(fileInfo, nameof(fileInfo));
            Ensure.NotNullOrEmptyOrWhiteSpace(newName);
            Ensure.That(PathHelper.IsValidFilename(newName), "Invalid file name: " + newName);

            fileInfo.Refresh();
            if (fileInfo.DirectoryName != null && fileInfo.Directory != null
                && fileInfo.Directory.Exists && fileInfo.Exists)
            {
                fileInfo.MoveTo(Path.Combine(fileInfo.DirectoryName, newName));
            }
            else
            {
                var errMsg = "Unable to rename the file: {0} to: {1}".FormatWith(fileInfo.FullName, newName);
                throw new InvalidOperationException(errMsg);
            }
        }
    }
}