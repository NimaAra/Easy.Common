namespace Easy.Common.XAML.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Provides a set of helper methods for working with <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an <see cref="ObservableCollection{T}"/> from the given <paramref name="sequence"/>.
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> sequence)
            => new ObservableCollection<T>(sequence);
    }
}