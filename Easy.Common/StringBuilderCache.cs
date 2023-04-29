namespace Easy.Common;

using System;
using System.Diagnostics;
using System.Text;

/// <summary>
/// Provides a cached reusable instance of <see cref="StringBuilder"/> per thread 
/// it is an optimization that reduces the number of instances constructed and collected.
/// <remarks>
/// <para>A StringBuilder instance is cached in <c>Thread Local Storage</c> and so there is one per thread.</para>
/// </remarks>
/// </summary>
public static class StringBuilderCache
{
    [ThreadStatic]
    private static StringBuilder? _cache;

    /// <summary>
    /// Acquires a cached instance of <see cref="StringBuilder"/> if one exists otherwise a new instance.
    /// </summary>
    /// <returns>An instance of <see cref="StringBuilder"/></returns>
    [DebuggerStepThrough]
    public static StringBuilder Acquire()
    {
        StringBuilder? result = _cache;
        if (result is null) { return new StringBuilder(); }

        result.Clear();
        _cache = null; // of that if caller forgets to release and return it is not kept alive by this class
        return result;
    }

    /// <summary>
    /// Gets the string representation of the <paramref name="builder"/> and releases it to the cache.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/></param>
    /// <returns>The string representation of the <paramref name="builder"/></returns>
    [DebuggerStepThrough]
    public static string GetStringAndRelease(StringBuilder builder)
    {
        string result = builder.ToString();
        _cache = builder;
        return result;
    }
}