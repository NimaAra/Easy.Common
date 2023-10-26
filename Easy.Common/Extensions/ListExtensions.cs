namespace Easy.Common.Extensions;

using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Extension methods for <see cref="IList{T}"/>
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Adds the given <paramref name="items"/> to the given <paramref name="list"/>.
    /// <remarks>This method is used to duck-type <see cref="IList{T}"/> with multiple items.</remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static void Add<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            list.Add(item);
        }
    }
}