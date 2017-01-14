namespace Easy.Common.Extensions
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides a set of helpful methods for <see cref="Expression"/>.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Returns the name of the property specified by the <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TInstance">The type of the model whose property is to be selected.</typeparam>
        /// <typeparam name="TProperty">The type of the property which should be selected.</typeparam>
        public static string GetPropertyName<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> selector)
        {
            var memberExpression = selector.Body as MemberExpression;
            return memberExpression?.Member.Name ?? ((MemberExpression)((UnaryExpression)selector.Body).Operand).Member.Name;
        }

        /// <summary>
        /// Returns the <see cref="PropertyInfo"/> specified by the <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TInstance">The type of the model whose property is to be selected.</typeparam>
        /// <typeparam name="TProperty">The type of the property to be selected.</typeparam>
        /// <param name="selector">The expression to select the property.</param>
        /// <param name="instance">The instance for which the property should be selected.</param>
        public static PropertyInfo GetProperty<TInstance, TProperty>(this Expression<Func<TInstance, TProperty>> selector, TInstance instance)
        {
            var type = typeof(TInstance);

            var member = selector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expression '{selector}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{selector}' refers to a field, not a property.");
            }

            // ReSharper disable once PossibleNullReferenceException
            if (!propInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException($"Expression '{selector}' refers to a property that is not from type {type}.");
            }

            return propInfo;
        }
    }
}