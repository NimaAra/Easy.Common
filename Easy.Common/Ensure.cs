namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Easy.Common.Extensions;

    /// <summary>
    /// Helper class that will <see langword="throw"/> exceptions when conditions are not satisfied.
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Ensures that the given expression is <see langword="true"/>.
        /// </summary>
        /// <typeparam name="TException">Type of exception to throw</typeparam>
        /// <param name="condition">Condition to test/ensure</param>
        /// <param name="message">Message for the exception</param>
        /// <exception>
        ///     Thrown when <cref>TException</cref> <paramref name="condition"/> is <see langword="false"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static void That<TException>(bool condition, string message = "The given condition is false.") where TException : Exception
        {
            if (!condition) { throw (TException)Activator.CreateInstance(typeof(TException), message); }
        }

        /// <summary>
        /// Ensures given <paramref name="condition"/> is <see langword="true"/>.
        /// </summary>
        /// <param name="condition">Condition to test</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="condition"/> is <see langword="false"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static void That(bool condition, string message = "The given condition is false.")
        {
            That<ArgumentException>(condition, message);
        }

        /// <summary>
        /// Ensures given <paramref name="condition"/> is <see langword="false"/>.
        /// </summary>
        /// <typeparam name="TException">Type of exception to throw</typeparam>
        /// <param name="condition">Condition to test</param>
        /// <param name="message">Message for the exception</param>
        /// <exception> 
        ///     Thrown when <paramref name="condition"/> is <see langword="false"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static void Not<TException>(bool condition, string message = "The given condition is true.") where TException : Exception
        {
            That<TException>(!condition, message);
        }

        /// <summary>
        /// Ensures given <paramref name="condition"/> is <see langword="false"/>.
        /// </summary>
        /// <param name="condition">Condition to test</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="condition"/> is <see langword="false"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static void Not(bool condition, string message = "The given condition is true.")
        {
            Not<ArgumentException>(condition, message);
        }

        /// <summary>
        /// Ensures given <see langword="object"/> is not null.
        /// </summary>
        /// <typeparam name="T">Type of the given <see langword="object"/> .</typeparam>
        /// <param name="value"> Value of the <see langword="object"/> to check for <see langword="null"/> reference.</param>
        /// <param name="argName"> Name of the argument.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="value"/> is null
        /// </exception>
        /// <returns> The <see cref="T"/>.</returns>
        [DebuggerStepThrough]
        public static T NotNull<T>(T value, string argName) where T : class
        {
            if (argName.IsNullOrEmptyOrWhiteSpace()) { argName = "Invalid"; }

            That<ArgumentNullException>(value != null, argName);
            return value;
        }

        /// <summary>
        /// Ensures given objects are equal.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare for equality</typeparam>
        /// <param name="left">Left <see langword="object"/>.</param>
        /// <param name="right">Right <see langword="object"/>.</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref cref="left"/> not equal to <paramref cref="right"/>
        /// </exception>
        /// <remarks>Null values will cause an exception to be thrown</remarks>
        [DebuggerStepThrough]
        public static void Equal<T>(T left, T right, string message = "Values must be equal.")
        {
            That<ArgumentException>(left.Equals(right), message);
        }

        /// <summary>
        /// Ensures given objects are not equal.
        /// </summary>
        /// <typeparam name="T">Type of objects to compare for equality</typeparam>
        /// <param name="left">First Value to Compare</param>
        /// <param name="right">Second Value to Compare</param>
        /// <param name="message">Message for the exception</param>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref cref="left"/> equal to <paramref cref="right"/>
        /// </exception>
        /// <remarks>Null values will cause an exception to be thrown</remarks>
        [DebuggerStepThrough]
        public static void NotEqual<T>(T left, T right, string message = "Values must not be equal.")
        {
            That<ArgumentException>(!left.Equals(right), message);
        }

        /// <summary>
        /// Ensures a given <paramref name="collection"/> is not null or empty.
        /// </summary>
        /// <typeparam name="T">Collection type.</typeparam>
        /// <param name="collection">Collection to check.</param>
        /// <param name="message">Message for the exception</param>
        /// <returns>The evaluated collection.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="collection"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="collection"/> is empty.
        /// </exception>
        [DebuggerStepThrough]
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> collection, string message = "Collection must not be null or empty.")
        {
            NotNull(collection, nameof(collection));
            Not<ArgumentException>(!collection.Any(), message);
            return collection;
        }

        /// <summary>
        /// Ensures the given string is not <see langword="null"/> or empty or whitespace.
        /// </summary>
        /// <param name="value"><c>String</c> <paramref name="value"/> to check.</param>
        /// <param name="message">Message for the exception</param>
        /// <returns>Value to return if it is not null, empty or whitespace.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref cref="value"/> is null or empty or whitespace.
        /// </exception>
        [DebuggerStepThrough]
        public static string NotNullOrEmptyOrWhiteSpace(string value, string message = "String must not be null, empty or whitespace.")
        {
            That<ArgumentException>(value.IsNotNullOrEmptyOrWhiteSpace(), message);
            return value;
        }

        /// <summary>
        /// Ensures given <see cref="DirectoryInfo"/> exists.
        /// </summary>
        /// <param name="directoryInfo">DirectoryInfo object representing the directory to check for existence.</param>
        /// <returns>DirectoryInfo to return if the directory exists.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="directoryInfo"/> is null.
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        ///     Thrown when <paramref cref="directoryInfo"/> is not found.
        /// </exception>
        /// <exception cref="IOException">
        ///     A device such as a disk drive is not ready.
        /// </exception>
        [DebuggerStepThrough]
        public static DirectoryInfo Exists(DirectoryInfo directoryInfo)
        {
            NotNull(directoryInfo, nameof(directoryInfo));

            directoryInfo.Refresh();
            That<DirectoryNotFoundException>(directoryInfo.Exists, "The given directory cannot be found.");
            return directoryInfo;
        }

        /// <summary>
        /// Ensures given <paramref name="fileInfo"/> exists.
        /// </summary>
        /// <param name="fileInfo">FileInfo object representing the file to check for existence.</param>
        /// <returns>FileInfo to return if the file exists.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <paramref name="fileInfo"/> is null.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Thrown when <paramref name="fileInfo"/> does not exist.
        /// </exception>
        [DebuggerStepThrough]
        public static FileInfo Exists(FileInfo fileInfo)
        {
            NotNull(fileInfo, nameof(fileInfo));

            fileInfo.Refresh();
            That<FileNotFoundException>(fileInfo.Exists, "The given file cannot be found.");
            return fileInfo;
        }
    }

}