namespace Easy.Common.Interfaces;

/// <summary>
/// Provides the contract for representing an <see cref="IEnum{TId}"/>.
/// </summary>
public interface IEnum<out TId>
{
    /// <summary>
    /// Gets the Id.
    /// </summary>
    TId Id { get; }
}

/// <summary>
/// Provides the contract for representing an <see cref="IEnum"/>.
/// </summary>
public interface IEnum : IEnum<uint>
{
    /// <summary>
    /// Gets the Name.
    /// </summary>
    string Name { get; }
}