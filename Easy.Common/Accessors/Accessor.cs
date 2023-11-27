// ReSharper disable once CheckNamespace
namespace Easy.Common;

using System;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// An abstraction for building a <see cref="ObjectAccessor"/> and
/// <see cref="GenericAccessor{TInstance}"/>.
/// </summary>
public abstract class Accessor
{
    /// <summary>
    /// Creates an instance of the <see cref="Accessor"/> class.
    /// </summary>
    /// <param name="type">
    /// The type of the object instance to access.
    /// </param>
    /// <param name="ignoreCase">
    /// The flag indicating whether property names should be treated case insensitively
    /// </param>
    /// <param name="includeNonPublic">
    /// The flag indicating whether non-public properties should be accessible or not
    /// </param>
    protected Accessor(IReflect type, bool ignoreCase, bool includeNonPublic)
    {
        Type = type;
        IgnoreCase = ignoreCase;
        IncludesNonPublic = includeNonPublic;

        Comparer = IgnoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        if (IncludesNonPublic) 
        {
            flags |= BindingFlags.NonPublic;
        }

        Properties = Type.GetProperties(flags);
    }

    /// <summary>
    /// Gets the <see cref="StringComparer"/> used by the <see cref="Accessor"/> to find 
    /// the properties on the given instance. 
    /// </summary>
    protected StringComparer Comparer { get; }

    /// <summary>
    /// Gets the type of the object this instance supports.
    /// </summary>
    public IReflect Type { get; }

    /// <summary>
    /// Gets the flag indicating whether property names are treated in a case sensitive manner.
    /// </summary>
    public bool IgnoreCase { get; }

    /// <summary>
    /// Gets the flag indicating whether non-public properties are supported by this instance.
    /// </summary>
    public bool IncludesNonPublic { get; }

    /// <summary>
    /// Gets the properties to which this instance can provide access to.
    /// </summary>
    public PropertyInfo[] Properties { get; }

    /// <summary>
    /// Builds an <see cref="ObjectAccessor"/> which provides easy access to all of 
    /// the <see cref="PropertyInfo"/> of the given <paramref name="instance"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static ObjectAccessor Build(object instance, bool ignoreCase = false, bool includeNonPublic = false)
    {
        Ensure.NotNull(instance, nameof(instance));
        return Build(instance.GetType(), ignoreCase, includeNonPublic);
    }

    /// <summary>
    /// Builds an <see cref="ObjectAccessor"/> which provides easy access to all of 
    /// the <see cref="PropertyInfo"/> of the given <paramref name="instanceType"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static ObjectAccessor Build(Type instanceType, bool ignoreCase = false, bool includeNonPublic = false)
    {
        Ensure.NotNull(instanceType, nameof(instanceType));
        return new ObjectAccessor(instanceType, ignoreCase, includeNonPublic);
    }

    /// <summary>
    /// Builds an <see cref="GenericAccessor{TInstance}"/> which provides easy access to all of 
    /// the <see cref="PropertyInfo"/> of the given <typeparamref name="TInstance"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static GenericAccessor<TInstance> Build<TInstance>(
        bool ignoreCase = false, bool includeNonPublic = false) where TInstance : class => new(ignoreCase, includeNonPublic);
}