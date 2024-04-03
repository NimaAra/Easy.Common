namespace Easy.Common;

using Easy.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// An abstraction for representing a class based enum.
/// </summary>
public abstract record class Enum<T> : IEnum where T : IEnum
{
    // ReSharper disable once StaticMemberInGenericType
    private static uint _id;

    /// <summary>
    /// Creates an instance of the <see cref="Enum{T}"/> class.
    /// </summary>
    /// <param name="name"></param>
    protected Enum(string name)
    {
        Id = _id++;
        Name = name;
    }

    /// <summary>
    /// Gets the Id.
    /// </summary>
    public uint Id { get; }
	
    /// <summary>
    /// Gets the Name.
    /// </summary>
    public string Name { get; }

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