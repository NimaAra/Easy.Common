namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods for <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly Type[] SimpleTypes =
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                typeof(bool),
                typeof(string),
                typeof(char),
                typeof(Guid),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(byte[])
            };

        private static readonly Dictionary<string, SequenceType> NonGenericCollectionsToSequenceTypeMapping = new Dictionary<string, SequenceType>(StringComparer.Ordinal)
            {
                { "System.String", SequenceType.String },
                { "System.Collections.ArrayList", SequenceType.ArrayList },
                { "System.Collections.Queue", SequenceType.Queue },
                { "System.Collections.Stack", SequenceType.Stack },
                { "System.Collections.BitArray", SequenceType.BitArray },
                { "System.Collections.SortedList", SequenceType.SortedList },
                { "System.Collections.Hashtable", SequenceType.Hashtable },
                { "System.Collections.Specialized.ListDictionary", SequenceType.ListDictionary },
                { "System.Collections.IList", SequenceType.IList },
                { "System.Collections.ICollection", SequenceType.ICollection },
                { "System.Collections.IDictionary", SequenceType.IDictionary },
                { "System.Collections.IEnumerable", SequenceType.IEnumerable }
            };

        /// <summary>
        /// It avoids materializing any attribute instances. <see href="http://stackoverflow.com/a/2282254/1226568"/>
        /// </summary>
        /// <typeparam name="T">Type of <c>Attribute</c> which has decorated the properties.</typeparam>
        /// <param name="type">Type of <c>Object</c> which has properties decorated with <typeparamref name="T"/>.</param>
        /// <param name="inherit">If <c>true</c> it also searches the ancestors for the <typeparamref name="T"/>.</param>
        /// <returns>A sequence containing properties decorated with <typeparamref name="T"/>.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type, bool inherit = true) where T : Attribute
        {
            Ensure.NotNull(type, nameof(type));
            return type.GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(T)) && (prop.DeclaringType == type || inherit));
        }

        /// <summary>
        /// Returns a mapping of <typeparamref name="T"/> attribute to <see cref="PropertyInfo"/> for a given <paramref name="type"/>
        /// </summary>
        /// <typeparam name="T">Type of attribute which will be used as the key</typeparam>
        /// <param name="type">Type whose properties will be mapped to the <typeparamref name="T"/> attributes</param>
        /// <param name="inherit">If <c>true</c> it also searches the ancestors for the <typeparamref name="T"/>.</param>
        /// <returns>A mapping between the attributes defined on the properties and the property infos</returns>
        [DebuggerStepThrough]
        public static Dictionary<T, PropertyInfo> GetAttributeToPropertyMapping<T>(this Type type, bool inherit = true) where T : Attribute
        {
            Ensure.NotNull(type, nameof(type));

            var properties = type.GetProperties();
            var result = new Dictionary<T, PropertyInfo>(properties.Length);
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<T>();
                var attr = attributes.FirstOrDefault();
                if (attr == null) { continue; }

                result[attr] = property;
            }

            return result;
        }

        /// <summary>
        /// Tries to get attributes of type <typeparamref name="T"/> defined on the given <paramref name="type"/>
        /// </summary>
        /// <typeparam name="T">The type of the attribute to get</typeparam>
        /// <param name="type">The type on which the attribute has been defined</param>
        /// <param name="attributes">All of the attributes found on the given type</param>
        /// <param name="inherit">If <c>true</c> it also searches the ancestors for the <typeparamref name="T"/>.</param>
        /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
        [DebuggerStepThrough]
        public static bool TryGetAttributes<T>(this Type type, out T[] attributes, bool inherit = true) where T : Attribute
        {
            var result = Attribute.GetCustomAttributes(type, typeof(T), inherit);

            if (result.Length > 0)
            {
                attributes = result as T[];
                return true;
            }

            attributes = null;
            return false;
        }

        /// <summary>
        /// Tries to get the generic type arguments for the given <paramref name="type"/>.
        /// <example>For a type of List{int} the generic type is int</example>
        /// </summary>
        /// <param name="type">The type for which generic type should be retrieved</param>
        /// <param name="genericArguments">The result</param>
        /// <returns><c>True</c> if generic types can be retrieved otherwise <c>False</c></returns>
        [DebuggerStepThrough]
        public static bool TryGetGenericArguments(this Type type, out Type[] genericArguments)
        {
            Ensure.NotNull(type, nameof(type));

            if (type.IsArray)
            {
                genericArguments = new[] { type.GetElementType() };
                return true;
            }

            if (!type.IsGenericType)
            {
                genericArguments = null;
                return false;
            }

            genericArguments = type.GetGenericArguments();
            return true;
        }

        /// <summary>
        /// Determines if the given <paramref name="type"/> is a sequence of elements.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <param name="sequenceType">The determined type of the sequence</param>
        /// <returns><c>True</c> if <paramref name="type"/> is a sequence otherwise <c>False</c></returns>
        [DebuggerStepThrough]
        // ReSharper disable once CyclomaticComplexity
        public static bool IsSequence(this Type type, out SequenceType sequenceType)
        {
            Ensure.NotNull(type, nameof(type));

            if (type.IsArray)
            {
                sequenceType = SequenceType.Array;
                return true;
            }

            if (NonGenericCollectionsToSequenceTypeMapping.TryGetValue(type.FullName, out sequenceType))
            {
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.List`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.HashSet`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericHashSet;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.ObjectModel.Collection`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericCollection;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.LinkedList`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericLinkedList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.Stack`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericStack;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.Queue`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericQueue;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.ICollection`1[[System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericICollectionKeyValue;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.ICollection`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericICollection;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIEnumerableKeyValue;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIEnumerable;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.Dictionary`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.SortedDictionary`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericSortedDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.SortedList`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericSortedList;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.IDictionary`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Generic.ICollection`2", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericIDictionary;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Concurrent.BlockingCollection`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericBlockingCollection;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Concurrent.ConcurrentBag`1", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericConcurrentBag;
                return true;
            }

            if (type.FullName.StartsWith("System.Collections.Concurrent.ConcurrentDictionary`2[[", StringComparison.Ordinal))
            {
                sequenceType = SequenceType.GenericConcurrentDictionary;
                return true;
            }

            var interfaces = type.GetInterfaces().ToArray();

            if (interfaces.Any(i => i.Name.StartsWith("IEnumerable`1", StringComparison.Ordinal)))
            {
                sequenceType = SequenceType.GenericCustom;
                return true;
            }

            if (interfaces.Any(i => i.Name.StartsWith("IEnumerable", StringComparison.Ordinal)))
            {
                sequenceType = SequenceType.Custom;
                return true;
            }

            sequenceType = SequenceType.Invalid;
            return false;
        }

        /// <summary>
        /// Determines whether the <paramref name="type"/> implements <typeparamref name="T"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static bool Implements<T>(this Type type)
        {
            Ensure.NotNull(type, nameof(type));
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether the given <paramref name="type"/> has a default constructor.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><c>True</c> if <paramref name="type"/> has a default constructor, <c>False</c> otherwise.</returns>
        [DebuggerStepThrough]
        public static bool HasDefaultConstructor(this Type type)
        {
            Ensure.NotNull(type, nameof(type));
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// Determines whether the given <paramref name="type"/> is of simple type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>True</c> if it is simple type otherwise <c>False</c>.</returns>
        [DebuggerStepThrough]
        public static bool IsSimpleType(this Type type)
        {
            Ensure.NotNull(type, nameof(type));
            var underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;

            return Array.IndexOf(SimpleTypes, type) > -1 || type.IsEnum;
        }

        /// <summary>
        /// Determines whether the given <paramref name="type"/> an array of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The Type of the elements in the array.</typeparam>
        /// <param name="type">The type of object.</param>
        /// <returns><c>True</c> or <c>False</c></returns>
        [DebuggerStepThrough]
        public static bool IsArrayOf<T>(this Type type)
        {
            return type == typeof(T[]);
        }

        /// <summary>
        /// Determines whether the given <paramref name="type"/> is a generic list
        /// </summary>
        /// <param name="type">The type to evaluate</param>
        /// <returns><c>True</c> if is generic otherwise <c>False</c></returns>
        [DebuggerStepThrough]
        public static bool IsGenericList(this Type type)
        {
            if (!type.IsGenericType) { return false; }

            var typeDef = type.GetGenericTypeDefinition();

            if (typeDef == typeof(List<>) || typeDef == typeof(IList<>)) { return true; }

            return false;
        }
    }

    /// <summary>
    /// Enum representing the possible types of a sequence.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SequenceType
    {
        /// <summary>
        /// Represents an invalid type.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Represents a <see cref="String"/>.
        /// </summary>
        String,

        /// <summary>
        /// Represents an Array.
        /// </summary>
        Array,

        /// <summary>
        /// Represents a <see cref="BitArray"/>. This type is non generic.
        /// </summary>
        BitArray,

        /// <summary>
        /// Represents an <see cref="ArrayList"/>. This type is non generic.
        /// </summary>
        ArrayList,

        /// <summary>
        /// Represents a <see cref="Queue"/>. This type is non generic.
        /// </summary>
        Queue,

        /// <summary>
        /// Represents a <see cref="Stack"/>. This type is non generic.
        /// </summary>
        Stack,

        /// <summary>
        /// Represents a <see cref="Hashtable"/>. This type is non generic.
        /// </summary>
        Hashtable,

        /// <summary>
        /// Represents a <see cref="SortedList"/>. This type is non generic.
        /// </summary>
        SortedList,

        /// <summary>
        /// Represents a <see cref="Dictionary"/>. This type is non generic.
        /// </summary>
        Dictionary,

        /// <summary>
        /// Represents a <see cref="ListDictionary"/>. This type is non generic.
        /// </summary>
        ListDictionary,

        /// <summary>
        /// Represents an <see cref="IList"/>. This interface type is non generic.
        /// </summary>
        IList,

        /// <summary>
        /// Represents an <see cref="ICollection"/>. This interface type is non generic.
        /// </summary>
        ICollection,

        /// <summary>
        /// Represents an <see cref="IDictionary"/>. This interface type is non generic.
        /// </summary>
        IDictionary,

        /// <summary>
        /// Represents an <see cref="IEnumerable"/>. This interface type is non generic.
        /// </summary>
        IEnumerable,

        /// <summary>
        /// Represents a custom implementation of <see cref="IEnumerable"/>.
        /// </summary>
        Custom,

        /// <summary>
        /// Represents a <see cref="List{T}"/>.
        /// </summary>
        GenericList,

        /// <summary>
        /// Represents a <see cref="LinkedList{T}"/>.
        /// </summary>
        GenericLinkedList,

        /// <summary>
        /// Represents a <see cref="Collection{T}"/>.
        /// </summary>
        GenericCollection,

        /// <summary>
        /// Represents a <see cref="Queue{T}"/>.
        /// </summary>
        GenericQueue,

        /// <summary>
        /// Represents a <see cref="Stack{T}"/>.
        /// </summary>
        GenericStack,

        /// <summary>
        /// Represents a <see cref="HashSet{T}"/>.
        /// </summary>
        GenericHashSet,

        /// <summary>
        /// Represents a <see cref="SortedList{TKey,TValue}"/>.
        /// </summary>
        GenericSortedList,

        /// <summary>
        /// Represents a <see cref="Dictionary{TKey,TValue}"/>.
        /// </summary>
        GenericDictionary,

        /// <summary>
        /// Represents a <see cref="SortedDictionary{TKey, TValue}"/>.
        /// </summary>
        GenericSortedDictionary,

        /// <summary>
        /// Represents a <see cref="BlockingCollection{T}"/>.
        /// </summary>
        GenericBlockingCollection,

        /// <summary>
        /// Represents a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// </summary>
        GenericConcurrentDictionary,

        /// <summary>
        /// Represents a <see cref="ConcurrentBag{T}"/>.
        /// </summary>
        GenericConcurrentBag,

        /// <summary>
        /// Represents an <see cref="IList{T}"/>.
        /// </summary>
        GenericIList,

        /// <summary>
        /// Represents an <see cref="ICollection{T}"/>.
        /// </summary>
        GenericICollection,

        /// <summary>
        /// Represents an <see cref="IEnumerable{T}"/>.
        /// </summary>
        GenericIEnumerable,

        /// <summary>
        /// Represents an <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        GenericIDictionary,

        /// <summary>
        /// Represents an <see> <cref>ICollection{KeyValuePair{TKey, TValue}}</cref></see>.
        /// </summary>
        GenericICollectionKeyValue,

        /// <summary>
        /// Represents an <see> <cref>IEnumerable{KeyValuePair{TKey, TValue}}</cref></see>.
        /// </summary>
        GenericIEnumerableKeyValue,

        /// <summary>
        /// Represents a custom implementation of <see cref="IEnumerable{T}"/>.
        /// </summary>
        GenericCustom
    }
}