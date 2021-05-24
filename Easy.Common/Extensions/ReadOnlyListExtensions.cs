namespace Easy.Common.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods for <see cref="IReadOnlyList{T}"/>
    /// </summary>
    public static class ReadOnlyListExtensions
    {
        /// <summary>
        /// Searches for the specified <paramref name="element"/> and returns the index of
        /// its first occurrence in <paramref name="self"/>.
        /// </summary>
        public static int IndexOf<T>(this IReadOnlyList<T> self, T element)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < self.Count; i++)
            {
                T item = self[i];
                if (comparer.Equals(item, element))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}