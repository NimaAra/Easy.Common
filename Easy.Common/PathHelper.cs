namespace Easy.Common
{
    using System.IO;
    using Easy.Common.Extensions;

    /// <summary>
    /// Provides helper methods for working with paths.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Makes the given <paramref name="fileName"/> safe for use within a URL.
        /// </summary>
        public static string MakeFileNameSafeForUrls(string fileName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(fileName);

            var extension = Path.GetExtension(fileName);
            var safeFileName = Path.GetFileNameWithoutExtension(fileName).ToSlug();
            return Path.Combine(Path.GetDirectoryName(fileName), safeFileName + extension);
        }

        /// <summary>
        /// Checks if a given <paramref name="fileName"/> is valid.
        /// </summary>
        public static bool IsValidFilename(string fileName)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(fileName);

            var invalidFileNameChars = new string(Path.GetInvalidFileNameChars());
            if (fileName.Contains(invalidFileNameChars)) { return false; }
            // other checks for UNC, drive-path format, etc
            return true;
        }
    }
}