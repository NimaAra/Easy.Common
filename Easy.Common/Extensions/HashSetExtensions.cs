namespace Easy.Common.Extensions;

using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Extension methods for <see cref="HashSet{T}"/>
/// </summary>
public static class HashSetExtensions
{
    /// <summary>
    /// Adds the given <paramref name="items"/> to the given <paramref name="set"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            set.Add(item);
        }
    }
}