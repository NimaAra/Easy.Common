namespace Easy.Common.Extensions
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a set of useful methods for working with <see cref="Lazy{T}"/>.
    /// </summary>
    public static class LazyExtensions
    {
        /// <summary>
        /// Allows asynchronously waiting for the value of the <paramref name="lazy"/>.
        /// </summary>
        public static TaskAwaiter<T> GetAwaiter<T>(this Lazy<Task<T>> lazy) => lazy.Value.GetAwaiter();
    }
}