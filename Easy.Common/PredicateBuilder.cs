namespace Easy.Common
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Helps with the creation of predicates.
    /// <see href="http://www.albahari.com/nutshell/predicatebuilder.aspx"/>
    /// </summary>
    public static class PredicateBuilder
    {
        /// <summary>
        /// Creates a predicate expression from the given lambda expression.
        /// </summary>
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate;
        }

        /// <summary>
        /// Creates a predicate that evaluates to <c>True</c>.
        /// </summary>
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        /// <summary>
        /// Creates a predicate that evaluates to <c>False</c>.
        /// </summary>
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>
        /// Combines the first predicate with the second using the logical <c>OR</c>.
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Combines the first predicate with the second using the logical <c>AND</c>.
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// Negates a given predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }
    }
}