namespace Easy.Common;

using Easy.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// An abstraction for representing a class based enum.
/// </summary>
public abstract record class Enum<T>(int Id, [CallerMemberName] string Name = default!) : IEnum where T : IEnum
{
    /// <summary>
    /// Retrieves a list of the values of the constants in a specified enumeration of type <typeparamref name="T"/>.
    /// </summary>
    public static IReadOnlyList<T> Values() =>
        typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>()
            .ToList();

    /// <summary>
    /// Returns the textual representation of the enum.
    /// </summary>
    public sealed override string ToString() => $"[{Id}] {Name}";
}