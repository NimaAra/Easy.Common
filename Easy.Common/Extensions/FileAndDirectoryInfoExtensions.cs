namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides a set of useful methods for working with <see cref="FileInfo"/> and <see cref="DirectoryInfo"/>.
    /// </summary>
    public static class FileAndDirectoryInfoExtensions
    {
        private const int DefaultBufferSize = 4096;
        private const char CarriageReturn = '\r';
        private const char NewLine = '\n';
        private const char Tab = '\t';

        /// <summary>
        /// Returns the size of the <paramref name="directoryInfo"/> and its sub-directories in bytes.
        /// </summary>
        [DebuggerStepThrough]
        public static long GetSizeInBytes(this DirectoryInfo directoryInfo)
        {
            var length = directoryInfo.GetFiles().AsParallel().Sum(file => file.Exists ? file.Length : 0);
            length += directoryInfo.GetDirectories().Sum(dir => dir.Exists ? dir.GetSizeInBytes() : 0);
            return length;
        }

        /// <summary>
        /// Indicates if a given <paramref name="directoryInfo"/> is hidden.
        /// </summary>
        /// <param name="directoryInfo">The <paramref name="directoryInfo"/> to check.</param>
        /// <returns>Boolean indicating if the <paramref name="directoryInfo"/> is hidden.</returns>
        [DebuggerStepThrough]
        public static bool IsHidden(this DirectoryInfo directoryInfo) 
            => (directoryInfo.Attributes & FileAttributes.Hidden) != 0;

        /// <summary>
        /// Indicates if a given <paramref name="fileInfo"/> is hidden.
        /// </summary>
        /// <param name="fileInfo">The <paramref name="fileInfo"/> to check.</param>
        /// <returns>
        /// Boolean indicating if the <paramref name="fileInfo"/> is hidden.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsHidden(this FileInfo fileInfo) 
            => (fileInfo.Attributes & FileAttributes.Hidden) != 0;

        /// <summary>
        /// Renames the given <paramref name="fileInfo"/> to <paramref name="newName"/> and returns the 
        /// renamed <see cref="FileInfo"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static FileInfo Rename(this FileInfo fileInfo, string newName)
        {
            Ensure.NotNull(fileInfo, nameof(fileInfo));
            Ensure.Exists(fileInfo);

            Ensure.NotNullOrEmptyOrWhiteSpace(newName);
            Ensure.That(newName.IsValidFileName(), $"Invalid file name: '{newName}'");

            var renamedFilePath = Path.Combine(fileInfo.DirectoryName 
                                               ?? throw new InvalidOperationException(), newName);
            fileInfo.MoveTo(renamedFilePath);
            return new FileInfo(renamedFilePath);
        }

        /// <summary>
        /// Opens a stream for reading with sequential optimization and providing 
        /// <see cref="FileShare.ReadWrite"/> for others.
        /// <remarks>
        /// A <see cref="FileNotFoundException"/> exception is 
        /// thrown if the file does not exist.
        /// </remarks>
        /// </summary>
        public static FileStream OpenSequentialRead(this FileSystemInfo file)
            => new FileStream(
                file.FullName,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                DefaultBufferSize,
                FileOptions.SequentialScan);

        /// <summary>
        /// Opens or creates a stream for reading with sequential optimization and providing 
        /// <see cref="FileShare.ReadWrite"/> for others.
        /// </summary>
        public static FileStream OpenOrCreateSequentialRead(this FileSystemInfo file)
            => new FileStream(
                file.FullName,
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.ReadWrite,
                DefaultBufferSize,
                FileOptions.SequentialScan);

        /// <summary>
        /// Opens or creates a stream for writing with sequential optimization and providing 
        /// <see cref="FileShare.Read"/> for others.
        /// </summary>
        public static FileStream OpenOrCreateSequentialWrite(this FileSystemInfo file)
            => new FileStream(
                file.FullName,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.Read,
                DefaultBufferSize,
                FileOptions.SequentialScan);

        /// <summary>
        /// Opens or creates a stream for reading and writing with sequential optimization and providing 
        /// <see cref="FileShare.ReadWrite"/> for others.
        /// </summary>
        public static FileStream OpenOrCreateSequentialReadWrite(this FileSystemInfo file)
            => new FileStream(
                file.FullName,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.ReadWrite,
                DefaultBufferSize,
                FileOptions.SequentialScan);

        /// <summary>
        /// Lazily reads every line in the <paramref name="file"/> without requiring a file lock.
        /// <remarks>
        /// This method is preferred over the <see cref="File.ReadLines(string)"/> which requires 
        /// a file lock that may result <see cref="IOException"/> if the file is opened exclusively 
        /// by another process such as <c>Excel</c>.
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<string> ReadLines(this FileInfo file) => file.ReadLines(Encoding.UTF8);

        /// <summary>
        /// Lazily reads every line in the <paramref name="file"/> without requiring a file lock.
        /// <remarks>
        /// This method is preferred over the <see cref="File.ReadLines(string)"/> which requires 
        /// a file lock that may result <see cref="IOException"/> if the file is opened exclusively 
        /// by another process such as <c>Excel</c>.
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<string> ReadLines(this FileInfo file, Encoding encoding)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNull(encoding, nameof(encoding));

            using (var fs = file.OpenOrCreateSequentialRead())
            using (var reader = new StreamReader(fs, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Enumerates every sub-directory inside the <paramref name="directory"/> in-parallel without 
        /// throwing <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<DirectoryInfo> EnumerateDirectoriesSafe(this DirectoryInfo directory,
            string searchPattern = "*", 
            SearchOption option = SearchOption.TopDirectoryOnly, 
            bool throwOnPathTooLong = false)
        {
            try
            {
                var directories = Enumerable.Empty<DirectoryInfo>();
                if (option == SearchOption.AllDirectories)
                {
                    directories = directory.EnumerateDirectories()
                        .SelectMany(d => d.EnumerateDirectoriesSafe(searchPattern, option, throwOnPathTooLong));
                }

                return directories.Concat(directory.EnumerateDirectories(searchPattern));
            }
            catch (Exception ex) when (
                ex is UnauthorizedAccessException || ex is PathTooLongException && !throwOnPathTooLong)
            {
                return Enumerable.Empty<DirectoryInfo>();
            }
        }

        /// <summary>
        /// Enumerates every file inside the <paramref name="directory"/> in-parallel without 
        /// throwing <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static IEnumerable<FileInfo> EnumerateFilesSafe(this DirectoryInfo directory,
            string searchPattern = "*", 
            SearchOption option = SearchOption.TopDirectoryOnly, 
            bool throwOnPathTooLong = false)
        {
            try
            {
                var files = Enumerable.Empty<FileInfo>();
                if (option == SearchOption.AllDirectories)
                {
                    files = directory.EnumerateDirectories()
                        .SelectMany(d => d.EnumerateFilesSafe(searchPattern, option, throwOnPathTooLong));
                }

                return files.Concat(directory.EnumerateFiles(searchPattern));
            }
            catch (Exception ex) when (
                ex is UnauthorizedAccessException || ex is PathTooLongException && !throwOnPathTooLong)
            {
                return Enumerable.Empty<FileInfo>();
            }
        }

        /// <summary>
        /// Determines whether the given <paramref name="file"/> is binary or a text file.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsBinary(this FileSystemInfo file)
        {
            var buffer = new char[256];

            using(var fs = file.OpenOrCreateSequentialRead())
            using (var reader = new StreamReader(fs))
            {
                var read = reader.ReadBlock(buffer, 0, buffer.Length);
                return ContainsBinary(buffer, read);
            }

            bool ContainsBinary(char[] bytes, int count)
            {
                for (var i = 0; i < count; i++)
                {
                    var c = bytes[i];
                    if (char.IsControl(c) && c != CarriageReturn && c != NewLine && c != Tab)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}