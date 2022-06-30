namespace Easy.Common.Interfaces;

/// <summary>
/// Provides the contract for representing an <see cref="IEnum"/>.
/// </summary>
public interface IEnum
{
    /// <summary>
    /// Gets the Id.
    /// </summary>
    int Id { get; }
    
    /// <summary>
    /// Gets the name.
    /// </summary>
    string Name { get; }
}