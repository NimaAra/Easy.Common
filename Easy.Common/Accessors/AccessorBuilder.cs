// ReSharper disable once CheckNamespace
namespace Easy.Common;

using Easy.Common.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

/// <summary>
/// Provides a very fast and efficient property setter and getter access as well
/// as object creation.
/// </summary>
public static class AccessorBuilder
 {
     /// <summary>
     /// Builds a property setter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property type of <typeparamref name="TProperty"/> with the name of <paramref name="propertyName"/>.
     /// <remarks>
     /// The setters for a <typeparamref name="TInstance"/> of <see lang="struct"/> are 
     /// intentionally not supported as changing the values of immutable types is a bad practice.
     /// </remarks>
     /// </summary>
     public static Action<TInstance, TProperty> BuildSetter<TInstance, TProperty>(string propertyName, bool includePrivate = false) where TInstance : class
     {
         Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

         if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo? propInfo))
         {
             throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
         }
         return BuildSetter<TInstance, TProperty>(propInfo, includePrivate);
     }

     /// <summary>
     /// Builds a property setter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property type of <typeparamref name="TProperty"/>.
     /// <remarks>
     /// The setters for a <typeparamref name="TInstance"/> of <see lang="struct"/> are 
     /// intentionally not supported as changing the values of immutable types is a bad practice.
     /// </remarks>
     /// </summary>
     public static Action<TInstance, TProperty> BuildSetter<TInstance, TProperty>(PropertyInfo propertyInfo, bool includePrivate = false) where TInstance : class
     {
         if (!propertyInfo.CanWrite)
         {
             throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support writing.");
         }

         MethodInfo? setMethod = propertyInfo.GetSetMethod(includePrivate);
         return (Action<TInstance, TProperty>) Delegate.CreateDelegate(typeof(Action<TInstance, TProperty>),
             setMethod ?? throw new InvalidOperationException());
     }

     /// <summary>
     /// Builds a property getter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property type of <typeparamref name="TProperty"/> with the name of <paramref name="propertyName"/>.
     /// </summary>
     public static Func<TInstance, TProperty> BuildGetter<TInstance, TProperty>(string propertyName, bool includePrivate = false) where TInstance : class
     {
         Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

         if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo? propInfo))
         {
             throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
         }
         return BuildGetter<TInstance, TProperty>(propInfo, includePrivate);
     }

     /// <summary>
     /// Builds a property getter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property type of <typeparamref name="TProperty"/>.
     /// </summary>
     public static Func<TInstance, TProperty> BuildGetter<TInstance, TProperty>(PropertyInfo propertyInfo, bool includePrivate = false) where TInstance : class
     {
         if (!propertyInfo.CanRead)
         {
             throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support reading.");
         }

         MethodInfo? getMethod = propertyInfo.GetGetMethod(includePrivate);
         return (Func<TInstance, TProperty>) Delegate.CreateDelegate(typeof(Func<TInstance, TProperty>),
             getMethod ?? throw new InvalidOperationException());
     }

     /// <summary>
     /// Builds a property setter for when both the instance and property type are unknown.
     /// </summary>
     public static Action<object, object> BuildSetter(PropertyInfo propertyInfo, bool includePrivate = false)
     {
         Type? instanceType = propertyInfo.ReflectedType;

         if (!propertyInfo.CanWrite)
         {
             throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{instanceType?.FullName}` does not support writing.");
         }

         MethodInfo? setMethod = propertyInfo.GetSetMethod(includePrivate);
         Type typeofObject = typeof(object);

         ParameterExpression instance = Expression.Parameter(typeofObject, "instance");
         ParameterExpression value = Expression.Parameter(typeofObject, "value");

         // value as T is slightly faster than (T)value, so if it's not a value type, use that
         UnaryExpression instanceCast = !(instanceType ?? throw new InvalidOperationException()).GetTypeInfo().IsValueType
             ? Expression.TypeAs(instance, instanceType)
             : Expression.Convert(instance, instanceType);

         UnaryExpression valueCast = !propertyInfo.PropertyType.GetTypeInfo().IsValueType
             ? Expression.TypeAs(value, propertyInfo.PropertyType)
             : Expression.Convert(value, propertyInfo.PropertyType);

         return Expression.Lambda<Action<object, object>>(
             Expression.Call(instanceCast, setMethod ?? throw new InvalidOperationException(), valueCast), instance, value).Compile();
     }

     /// <summary>
     /// Builds a property getter for when both the instance and property type are unknown.
     /// </summary>
     public static Func<object, object> BuildGetter(PropertyInfo propertyInfo, bool includePrivate = false)
     {
         Type? instanceType = propertyInfo.ReflectedType;
         if (!propertyInfo.CanRead)
         {
             throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{instanceType?.FullName}` does not support reading.");
         }

         MethodInfo? getMethod = propertyInfo.GetGetMethod(includePrivate);
         Type typeofObject = typeof(object);

         ParameterExpression instance = Expression.Parameter(typeofObject, "instance");
         bool isValueType = (instanceType ?? throw new InvalidOperationException()).GetTypeInfo().IsValueType;
         UnaryExpression instanceCast = !isValueType
             ? Expression.TypeAs(instance, instanceType)
             : Expression.Convert(instance, instanceType);

         return Expression.Lambda<Func<object, object>>(
             Expression.TypeAs(Expression.Call(instanceCast, getMethod ?? throw new InvalidOperationException()), typeofObject), instance).Compile();
     }

     /// <summary>
     /// Builds a property setter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property name of <paramref name="propertyName"/>.
     /// <remarks>
     /// The setters for a <typeparamref name="TInstance"/> of <see lang="struct"/> are 
     /// intentionally not supported as changing the values of immutable types is a bad practice.
     /// </remarks>
     /// </summary>
     public static Action<TInstance, object> BuildSetter<TInstance>(string propertyName, bool includePrivate = false) where TInstance : class
     {
         Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

         if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo? propInfo))
         {
             throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
         }
            
         return BuildSetter<TInstance>(propInfo, includePrivate);
     }

     /// <summary>
     /// Builds a property setter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property of <paramref name="propertyInfo"/>.
     /// <remarks>
     /// The setters for a <typeparamref name="TInstance"/> of <see lang="struct"/> are 
     /// intentionally not supported as changing the values of immutable types is a bad practice.
     /// </remarks>
     /// </summary>
     public static Action<TInstance, object> BuildSetter<TInstance>(PropertyInfo propertyInfo, bool includePrivate = false) where TInstance : class
     {
         if (!propertyInfo.CanWrite)
         {
             throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support writing.");
         }

         MethodInfo? setMethod = propertyInfo.GetSetMethod(includePrivate);

         ParameterExpression instance = Expression.Parameter(typeof(TInstance), "instance");
         ParameterExpression value = Expression.Parameter(typeof(object), "value");
         bool isValueType = propertyInfo.PropertyType.GetTypeInfo().IsValueType;
         UnaryExpression valueCast = !isValueType
             ? Expression.TypeAs(value, propertyInfo.PropertyType)
             : Expression.Convert(value, propertyInfo.PropertyType);

         return Expression.Lambda<Action<TInstance, object>>(
             Expression.Call(instance, setMethod ?? throw new InvalidOperationException(), valueCast), instance, value).Compile();
     }

     /// <summary>
     /// Builds a property getter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property name of <paramref name="propertyName"/>.
     /// </summary>
     public static Func<TInstance, object> BuildGetter<TInstance>(string propertyName, bool includePrivate = false)
     {
         Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

         if (!typeof(TInstance).TryGetInstanceProperty(propertyName, out PropertyInfo? propInfo))
         {
             throw new InvalidOperationException("Unable to find property: " + propertyName + ".");
         }
            
         return BuildGetter<TInstance>(propInfo, includePrivate);
     }

     /// <summary>
     /// Builds a property getter for a given instance type of <typeparamref name="TInstance"/> 
     /// and property of <paramref name="propertyInfo"/>.
     /// </summary>
     public static Func<TInstance, object> BuildGetter<TInstance>(PropertyInfo propertyInfo, bool includePrivate = false)
     {
         if (!propertyInfo.CanRead)
         {
             throw new ArgumentException($"Property: `{propertyInfo.Name}` of type: `{propertyInfo.ReflectedType?.FullName}` does not support reading.");
         }

         MethodInfo? getMethod = propertyInfo.GetGetMethod(includePrivate);

         ParameterExpression instance = Expression.Parameter(typeof(TInstance), "instance");
         return Expression.Lambda<Func<TInstance, object>>(
             Expression.TypeAs(Expression.Call(instance, getMethod ?? throw new InvalidOperationException()), typeof(object)), instance).Compile();
     }

     /// <summary>
     /// Builds a delegate for creating an instance of the <typeparamref name="TInstance"/>.
     /// </summary>
     public static Func<TInstance> BuildInstanceCreator<TInstance>() where TInstance : new()
     {
         Type type = typeof(TInstance);

         DynamicMethod dynamicMethod = new DynamicMethod("Build_" + type.Name, type, Type.EmptyTypes, typeof(AccessorBuilder).Module, true);
         ILGenerator ilGen = dynamicMethod.GetILGenerator();

         var defaultCtor = type.GetConstructor(Type.EmptyTypes);
         if (defaultCtor is not null)
         {
             // Call the ctor, all values on the stack are passed to the ctor
             ilGen.Emit(OpCodes.Newobj, defaultCtor);
         }
         else
         {
             LocalBuilder builder = ilGen.DeclareLocal(type);
             ilGen.Emit(OpCodes.Ldloca, builder);
             ilGen.Emit(OpCodes.Initobj, type);
             ilGen.Emit(OpCodes.Ldloc, builder);
         }

         // Return the new object
         ilGen.Emit(OpCodes.Ret);

         return (Func<TInstance>)dynamicMethod.CreateDelegate(typeof(Func<TInstance>));
     }

     /// <summary>
     /// Builds a delegate for creating an instance of the <typeparamref name="TInstance"/> 
     /// from its <paramref name="constructor"/>.
     /// <remarks>
     /// The order of arguments passed to the delegate should match the order set by the constructor.
     /// </remarks>
     /// <exception cref="IndexOutOfRangeException">
     /// Thrown if the count parameters passed to the constructor does not match the required 
     /// constructor parameter count.
     /// </exception>
     /// <exception cref="InvalidCastException">
     /// Thrown if parameters passed to the constructor are of invalid type.
     /// </exception>
     /// </summary>
     public static Func<object[], TInstance> BuildInstanceCreator<TInstance>(ConstructorInfo constructor)
     {
         Type type = typeof(TInstance);

         ParameterInfo[] ctroParams = constructor.GetParameters();

         DynamicMethod dynamicMethod = new("Build_" + type.Name, type, new[] { typeof(object[]) }, typeof(AccessorBuilder).Module, true);
         ILGenerator ilGen = dynamicMethod.GetILGenerator();

         // Cast each argument of the input object array to the appropriate type.
         for (int i = 0; i < ctroParams.Length; i++)
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
 }