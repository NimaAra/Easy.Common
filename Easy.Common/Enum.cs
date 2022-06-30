namespace Easy.Common;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Easy.Common.Interfaces;

/// <summary>
/// An abstraction for representing a class based enum.
/// </summary>
public abstract record class Enum<T>(int Id, string Name) : IEnum where T : IEnum
{
    /// <summary>
    /// Retrieves a list of the values of the constants in a specified enumeration of type <typeparamref name="T"/>.
    /// </summary>
    public static IReadOnlyList<T> Values() =>
        typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>()
            .ToList();
}