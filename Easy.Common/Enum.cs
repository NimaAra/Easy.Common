namespace Easy.Common;

using Easy.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// An abstraction for representing a class based enum.
/// </summary>
public abstract record class Enum<T, TId>(TId Id) : IEnum<TId> where T : IEnum<TId>
{
    /// <summary>
    /// Retrieves a list of the values of the constants in a specified enumeration of type <typeparamref name="T"/>.
    /// </summary>
    public static IReadOnlyList<T> Values() =>
        typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Select(static f => f.GetValue(null))
            .Cast<T>()
            .ToArray();
}

/// <summary>
/// An abstraction for representing a class based enum.
/// </summary>
public abstract record class Enum<T>(uint Id, string Name) : Enum<T, uint>(Id), IEnum where T : IEnum
{
    // ReSharper disable once StaticMemberInGenericType
    private static uint _counter;

    /// <summary>
    /// Creates an instance of the <see cref="Enum{T}"/> class.
    /// </summary>
    protected Enum(string name) : this(_counter++, name)
    {
    }
}