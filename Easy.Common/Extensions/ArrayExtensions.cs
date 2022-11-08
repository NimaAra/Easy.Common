namespace Easy.Common.Extensions;

using System;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// A set of extension methods for arrays.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Returns a copy of the given <paramref name="array"/> and fills it with <paramref name="value"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T[] FillAndCopy<T>(this T[] array, T value, int startIdx = 0)
    {
        T[] tmpArr = array.ToArray();

        foreach (ref T item in tmpArr.AsSpan(startIdx))
        {
            item = value;
        }

        return tmpArr;
    }

    /// <summary>
    /// Returns the given <paramref name="array"/> and fills it with <paramref name="value"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T[] Fill<T>(this T[] array, T value, int startIdx = 0)
    {
        foreach (ref T item in array.AsSpan(startIdx))
        {
            item = value;
        }

        return array;
    }

    /// <summary>
    /// Returns the given <paramref name="array"/> and fills it with values returned by the <paramref name="mapper"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T[] Map<T>(this T[] array, Func<T, T> mapper, int startIdx = 0)
    {
        foreach (ref T item in array.AsSpan(startIdx))
        {
            item = mapper(item);
        }

        return array;
    }

    /// <summary>
    /// Returns a copy of the given <paramref name="array"/> and fills it with values returned by the <paramref name="mapper"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static T[] MapAndCopy<T>(this T[] array, Func<T, T> mapper, int startIdx = 0)
    {
        T[] tmpArr = array.ToArray();
        foreach (ref T item in tmpArr.AsSpan(startIdx))
        {
            item = mapper(item);
        }

        return tmpArr;
    }
}