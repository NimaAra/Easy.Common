namespace Easy.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extension methods for <see cref="ICollection{T}"/>
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Generates an array of <see cref="TreeNode{T}"/> from the given <paramref name="collection"/>
    /// based on parent-child relationship.
    /// </summary>
    /// <typeparam name="T">The type of each value in the node.</typeparam>
    /// <typeparam name="TId">The type of the key used for determining parent-child relationship.</typeparam>
    public static TreeNode<T>[] ToTree<T, TId>(this ICollection<T> collection,
        Func<T, TId> idSelector,
        Func<T, TId> parentIdSelector,
        TId? rootId = default) 
        => collection.Where(x => EqualityComparer<TId?>.Default.Equals(parentIdSelector(x), rootId))
            .Select(x => new TreeNode<T>(x, collection.ToTree(idSelector, parentIdSelector, idSelector(x))))
            .ToArray();
}

/// <summary>
/// Represents a node of a tree.
/// </summary>
/// <typeparam name="T">The type of the value in each node.</typeparam>
public sealed class TreeNode<T>
{
    /// <summary>
    /// Gets the value of the node.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets the children of this node.
    /// </summary>
    public TreeNode<T>[] Children { get; }

    /// <summary>
    /// Creates an instance of the <see cref="TreeNode{T}"/>.
    /// </summary>
    public TreeNode(T value, TreeNode<T>[] children)
    {
        Value = value;
        Children = children;
    }
}