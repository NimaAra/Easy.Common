// ReSharper disable once CheckNamespace
namespace Easy.Common;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// An abstraction for gaining fast access to all the <see cref="PropertyInfo"/> of 
/// the given <typeparamref name="TInstance"/>.
/// </summary>
public sealed class GenericAccessor<TInstance> : ObjectAccessor where TInstance : class
{
    private readonly SortedList<string, PropertyInfo> _propDic;

    private readonly Hashtable
        _genericInstanceGettersCache,
        _genericInstanceSettersCache,
        _genericPropertiesGettersCache,
        _genericPropertiesSettersCache;

    [DebuggerStepThrough]
    internal GenericAccessor(bool ignoreCase, bool includeNonPublic)
        : base(typeof(TInstance), ignoreCase, includeNonPublic)
    {
        _propDic = new SortedList<string, PropertyInfo>(Properties.Length, Comparer);

        _genericPropertiesGettersCache = new Hashtable(Properties.Length, Comparer);
        _genericPropertiesSettersCache = new Hashtable(Properties.Length, Comparer);
        _genericInstanceGettersCache = new Hashtable(Properties.Length, Comparer);
        _genericInstanceSettersCache = new Hashtable(Properties.Length, Comparer);

        foreach (PropertyInfo prop in Properties)
        {
            string propName = prop.Name;

            if (prop.CanRead)
            {
                _genericPropertiesGettersCache[propName] = AccessorBuilder.BuildGetter<TInstance>(prop, IncludesNonPublic);
            }

            if (prop.CanWrite)
            {
                _genericPropertiesSettersCache[propName] = AccessorBuilder.BuildSetter<TInstance>(prop, IncludesNonPublic);
            }

            _propDic[propName] = prop;
        }
    }

    /// <summary>
    /// Gets or sets the value of the given <paramref name="propertyName"/> for 
    /// the given <paramref name="instance"/>.
    /// </summary>
    public object? this[TInstance instance, string propertyName]
    {
        get
        {
            if (_genericPropertiesGettersCache[propertyName] is Func<TInstance, object?> getter)
            {
                return getter(instance);
            }

            throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports reading.");
        }

        set
        {
            if (_genericPropertiesSettersCache[propertyName] is Action<TInstance, object?> setter)
            {
                setter(instance, value);
            }
            else
            {
                throw new ArgumentException($"Type: `{instance.GetType().FullName}` does not have a property named: `{propertyName}` that supports writing.");
            }
        }
    }

    /// <summary>
    /// Attempts to get the value of a property selected by the given <paramref name="propertyName"/> 
    /// for the given <paramref name="instance"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGet<TProperty>(TInstance instance, string propertyName, [NotNullWhen(true)]out TProperty? value)
    {
        Hashtable cache = _genericInstanceGettersCache;

        Func<TInstance, TProperty>? getter = cache[propertyName] as Func<TInstance, TProperty>;
        if (getter is null)
        {
            if (!_propDic.TryGetValue(propertyName, out PropertyInfo? prop))
            {
                value = default;
                return false;
            }

            lock (cache)
            {
                getter = cache[propertyName] as Func<TInstance, TProperty>;
                if (getter is null)
                {
                    try
                    {
                        getter = AccessorBuilder.BuildGetter<TInstance, TProperty>(prop.Name, IncludesNonPublic);
                        cache[prop.Name] = getter;
                    }
                    catch (ArgumentException)
                    {
                        value = default;
                        return false;
                    }
                }
            }
        }

        value = getter(instance);
        return value is not null;
    }

    /// <summary>
    /// Attempts to set the value of the given <paramref name="propertyName"/>
    /// for the given <paramref name="instance"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySet<TProperty>(TInstance instance, string propertyName, TProperty value)
    {
        Hashtable cache = _genericInstanceSettersCache;

        Action<TInstance, TProperty>? setter = cache[propertyName] as Action<TInstance, TProperty>;
        if (setter is null)
        {
            if (!_propDic.TryGetValue(propertyName, out PropertyInfo? prop)) { return false; }

            lock (cache)
            {
                setter = cache[propertyName] as Action<TInstance, TProperty>;
                if (setter is null)
                {
                    try
                    {
                        setter = AccessorBuilder.BuildSetter<TInstance, TProperty>(prop.Name, IncludesNonPublic);
                        cache[prop.Name] = setter;
                    }
                    catch (ArgumentException)
                    {
                        return false;
                    }
                }
            }
        }

        setter(instance, value);
        return true;
    }
}