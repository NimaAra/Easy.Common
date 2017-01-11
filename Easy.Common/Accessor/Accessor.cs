 // ReSharper disable once CheckNamespace
namespace Easy.Common
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using Easy.Common.Extensions;

    /// <summary>
    /// Provides a very fast and efficient property setter and getter access as well as object creation.
    /// </summary>
    public static class Accessor
    {
        /// <summary>
        /// Creates an <see cref="ObjectAccessor"/> which provides easy access to all of the <see cref="PropertyInfo"/> of the given <paramref name="type"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static ObjectAccessor CreateAccessor(Type type, bool ignoreCase = false, bool includeNonPublic = false)
        {
            Ensure.NotNull(type, nameof(type));
            return new ObjectAccessor(type, ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// Creates an <see cref="GenericAccessor{TInstance}"/> which provides easy access to all of the <see cref="PropertyInfo"/> of the given <typeparamref name="{TInstance}"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static GenericAccessor<TInstance> CreateAccessor<TInstance>(bool ignoreCase = false, bool includeNonPublic = false) where TInstance : class
        {
            return new GenericAccessor<TInstance>(ignoreCase, includeNonPublic);
        }

        /// <summary>
        /// Creates a property setter for a given instance type of <typeparamref name="TInstance"/> and property type of <typeparamref name="TProperty"/> with the name of <paramref name="propertyName"/>.
        /// <remarks>
        /// The setters for a <typeparamref name="{TInstance}"/> of <see lang="struct"/> are 
        /// intentionally not supported as changing the values of immutable types is a bad practice.
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static Action<TInstance, TProperty> CreateSetter<TInstance, TProperty>(string propertyName, bool includeNonPublic = false) where TInstance : class
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

            PropertyInfo propInfo;
            var found = typeof(TInstance).TryGetInstanceProperty(propertyName, out propInfo);
            ThrowIfNotFound(found, propertyName);
            return CreateSetter<TInstance, TProperty>(propInfo, includeNonPublic);
        }

        /// <summary>
        /// Creates a property setter for a given instance type of <typeparamref name="TInstance"/> and property type of <typeparamref name="TProperty"/>.
        /// <remarks>
        /// The setters for a <typeparamref name="{TInstance}"/> of <see lang="struct"/> are 
        /// intentionally not supported as changing the values of immutable types is a bad practice.
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static Action<TInstance, TProperty> CreateSetter<TInstance, TProperty>(PropertyInfo propertyInfo, bool includeNonPublic = false) where TInstance : class
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));
            var setMethod = propertyInfo.GetSetMethod(includeNonPublic);
            return (Action<TInstance, TProperty>)Delegate.CreateDelegate(typeof(Action<TInstance, TProperty>), setMethod);
        }

        /// <summary>
        /// Creates a property getter for a given instance type of <typeparamref name="TInstance"/> and property type of <typeparamref name="TProperty"/> with the name of <paramref name="propertyName"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Func<TInstance, TProperty> CreateGetter<TInstance, TProperty>(string propertyName, bool includeNonPublic = false) where TInstance : class
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

            PropertyInfo propInfo;
            var found = typeof(TInstance).TryGetInstanceProperty(propertyName, out propInfo);
            ThrowIfNotFound(found, propertyName);
            return CreateGetter<TInstance, TProperty>(propInfo, includeNonPublic);
        }

        /// <summary>
        /// Creates a property getter for a given instance type of <typeparamref name="TInstance"/> and property type of <typeparamref name="TProperty"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Func<TInstance, TProperty> CreateGetter<TInstance, TProperty>(PropertyInfo propertyInfo, bool includeNonPublic = false) where TInstance : class
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));
            var getMethod = propertyInfo.GetGetMethod(includeNonPublic);
            return (Func<TInstance, TProperty>)Delegate.CreateDelegate(typeof(Func<TInstance, TProperty>), getMethod);
        }

        /// <summary>
        /// Creates a property setter for when both the instance and property type are unknown.
        /// </summary>
        [DebuggerStepThrough]
        public static Action<object, object> CreateSetter(PropertyInfo propertyInfo, bool includeNonPublic = false)
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));
            var instanceType = propertyInfo.ReflectedType;

            var setMethod = propertyInfo.GetSetMethod(includeNonPublic);
            var typeofObject = typeof(object);

            var instance = Expression.Parameter(typeofObject, "instance");
            var value = Expression.Parameter(typeofObject, "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            // ReSharper disable once PossibleNullReferenceException
            var instanceCast = !instanceType.GetTypeInfo().IsValueType
                ? Expression.TypeAs(instance, instanceType)
                : Expression.Convert(instance, instanceType);

            var valueCast = !propertyInfo.PropertyType.GetTypeInfo().IsValueType
                ? Expression.TypeAs(value, propertyInfo.PropertyType)
                : Expression.Convert(value, propertyInfo.PropertyType);

            return Expression.Lambda<Action<object, object>>(
                Expression.Call(instanceCast, setMethod, valueCast), instance, value).Compile();
        }

        /// <summary>
        /// Creates a property getter for when both the instance and property type are unknown.
        /// </summary>
        [DebuggerStepThrough]
        public static Func<object, object> CreateGetter(PropertyInfo propertyInfo, bool includeNonPublic = false)
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));
            var instanceType = propertyInfo.ReflectedType;
            var getMethod = propertyInfo.GetGetMethod(includeNonPublic);
            var typeofObject = typeof(object);

            var instance = Expression.Parameter(typeofObject, "instance");
            // ReSharper disable once PossibleNullReferenceException
            var isValueType = instanceType.GetTypeInfo().IsValueType;
            var instanceCast = !isValueType
                ? Expression.TypeAs(instance, instanceType)
                : Expression.Convert(instance, instanceType);

            return Expression.Lambda<Func<object, object>>(
                Expression.TypeAs(Expression.Call(instanceCast, getMethod), typeofObject), instance).Compile();
        }

        /// <summary>
        /// Creates a property setter for a given instance type of <typeparamref name="TInstance"/> and property name of <paramref name="propertyName"/>.
        /// <remarks>
        /// The setters for a <typeparamref name="{TInstance}"/> of <see lang="struct"/> are 
        /// intentionally not supported as changing the values of immutable types is a bad practice.
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static Action<TInstance, object> CreateSetter<TInstance>(string propertyName, bool includeNonPublic = false) where TInstance : class
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

            PropertyInfo propInfo;
            var found = typeof(TInstance).TryGetInstanceProperty(propertyName, out propInfo);
            ThrowIfNotFound(found, propertyName);
            return CreateSetter<TInstance>(propInfo, includeNonPublic);
        }

        /// <summary>
        /// Creates a property setter for a given instance type of <typeparamref name="TInstance"/> and property of <paramref name="propertyInfo"/>.
        /// <remarks>
        /// The setters for a <typeparamref name="{TInstance}"/> of <see lang="struct"/> are 
        /// intentionally not supported as changing the values of immutable types is a bad practice.
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static Action<TInstance, object> CreateSetter<TInstance>(PropertyInfo propertyInfo, bool includeNonPublic = false) where TInstance : class
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));
            var setMethod = propertyInfo.GetSetMethod(includeNonPublic);

            var instance = Expression.Parameter(typeof(TInstance), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var isValueType = propertyInfo.PropertyType.GetTypeInfo().IsValueType;
            var valueCast = !isValueType
                ? Expression.TypeAs(value, propertyInfo.PropertyType)
                : Expression.Convert(value, propertyInfo.PropertyType);

            return Expression.Lambda<Action<TInstance, object>>(
                Expression.Call(instance, setMethod, valueCast), instance, value).Compile();
        }

        /// <summary>
        /// Creates a property getter for a given instance type of <typeparamref name="TInstance"/> and property name of <paramref name="propertyName"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Func<TInstance, object> CreateGetter<TInstance>(string propertyName, bool includeNonPublic = false)
        {
            Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

            PropertyInfo propInfo;
            var found = typeof(TInstance).TryGetInstanceProperty(propertyName, out propInfo);
            ThrowIfNotFound(found, propertyName);
            return CreateGetter<TInstance>(propInfo, includeNonPublic);
        }

        /// <summary>
        /// Creates a property getter for a given instance type of <typeparamref name="TInstance"/> and property of <paramref name="propertyInfo"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Func<TInstance, object> CreateGetter<TInstance>(PropertyInfo propertyInfo, bool includeNonPublic = false)
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));
            var getMethod = propertyInfo.GetGetMethod(includeNonPublic);

            var instance = Expression.Parameter(typeof(TInstance), "instance");
            return Expression.Lambda<Func<TInstance, object>>(
                Expression.TypeAs(Expression.Call(instance, getMethod), typeof(object)), instance).Compile();
        }

        /// <summary>
        /// Creates a delegate for building an instance of the <typeparamref name="TInstance"/> from its <paramref name="constructor"/>.
        /// <remarks>The order of arguments passed to the delegate should match the order set by the constructor.</remarks>
        /// <exception cref="IndexOutOfRangeException">Thrown if the count parameters passed to the constructor does not match the required constructor parameter count.</exception>
        /// <exception cref="InvalidCastException">Thrown if parameters passed to the constructor are of invalid type.</exception>
        /// </summary>
        [DebuggerStepThrough]
        public static Func<object[], TInstance> CreateInstanceBuilder<TInstance>(ConstructorInfo constructor)
        {
            Ensure.NotNull(constructor, nameof(constructor));
            var type = constructor.ReflectedType;

            var ctroParams = constructor.GetParameters();

            // ReSharper disable once AssignNullToNotNullAttribute
            var dynamicMethod = new DynamicMethod("Create_" + constructor.Name, type, new[] { typeof(object[]) }, type, true);
            var ilGen = dynamicMethod.GetILGenerator();

            // Cast each argument of the input object array to the appropriate type.
            for (var i = 0; i < ctroParams.Length; i++)
            {
                ilGen.Emit(OpCodes.Ldarg_0); // Push Object array
                ilGen.Emit(OpCodes.Ldc_I4, i); // Push the index to access
                ilGen.Emit(OpCodes.Ldelem_Ref); // Push the element at the previously loaded index

                // Cast the object to the appropriate ctor Parameter Type
                var paramType = ctroParams[i].ParameterType;
                var isValueType = paramType.GetTypeInfo().IsValueType;
                ilGen.Emit(isValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
            }

            // Call the ctor, all values on the stack are passed to the ctor
            ilGen.Emit(OpCodes.Newobj, constructor);
            // Return the new object
            ilGen.Emit(OpCodes.Ret);

            return (Func<object[], TInstance>)dynamicMethod.CreateDelegate(typeof(Func<object[], TInstance>));
        }

        [DebuggerStepThrough]
        internal static void ThrowIfNotFound(bool found, string propertyName)
        {
            if (!found)
            {
                throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
            }
        }
    }
}
