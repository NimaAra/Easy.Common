namespace Easy.Common.Interfaces;

/// <summary>
/// Provides the contract for representing an <see cref="IEnum"/>.
/// </summary>
public interface IEnum
{
    /// <summary>
    /// Gets the Id.
    /// </summary>
    uint Id { get; }
    
    /// <summary>
    /// Gets the Name.
    /// </summary>
    string Name { get; }
}