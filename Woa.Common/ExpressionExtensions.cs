using System.Linq.Expressions;
using System.Reflection;
using Woa.Common;

/// <summary>
/// Extension methods for <see cref="Expression"/>.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="condition"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<Expression<Func<T, bool>>> AddIf<T>(this IList<Expression<Func<T, bool>>> expressions, bool condition, Expression<Func<T, bool>> expression)
    {
        if (condition)
        {
            expressions.Add(expression);
        }

        return expressions;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="condition"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IList<Expression<Func<T, bool>>> AddIf<T>(this IList<Expression<Func<T, bool>>> expressions, Func<bool> condition, Expression<Func<T, bool>> expression)
    {
        return expressions.AddIf(condition(), expression);
    }

    /// <summary>
    /// Combine all expressions into a new expression.
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Compose<T>(this IEnumerable<Expression<Func<T, bool>>> expressions, PredicateOperator type = PredicateOperator.AndAlso)
    {
        return expressions.Compose(PredicateBuilder.True<T>(), type);
    }

    /// <summary>
    /// Combine all expressions into a new expression.
    /// </summary>
    /// <param name="expressions"></param>
    /// <param name="seed"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Compose<T>(this IEnumerable<Expression<Func<T, bool>>> expressions, Expression<Func<T, bool>> seed, PredicateOperator type = PredicateOperator.AndAlso)
    {
        var predicate = expressions.Aggregate(seed, (current, next) => Compose(current, next, type));
        return predicate;
    }

    private static Expression<Func<T, bool>> Compose<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right, PredicateOperator type)
    {
        return type switch
        {
            PredicateOperator.AndAlso => left.And(right),
            PredicateOperator.OrElse => left.Or(right),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary>
    /// Creates a <see cref="MemberExpression"/> for the specified property.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="propertyName">The property name. e.g: Name, Customer.Name</param>
    public static Expression Property(this Expression expression, string propertyName)
    {
        if (propertyName.All(t => t != '.'))
            return Expression.Property(expression, propertyName);
        var propertyNameList = propertyName.Split('.');
        Expression result = null;
        for (var i = 0; i < propertyNameList.Length; i++)
        {
            if (i == 0)
            {
                result = Expression.Property(expression, propertyNameList[0]);
                continue;
            }

            result = result.Property(propertyNameList[i]);
        }

        return result;
    }

    /// <summary>
    /// Create a <see cref="MemberExpression"/> for the specified property.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="member">The property member.</param>
    public static Expression Property(this Expression expression, MemberInfo member)
    {
        return Expression.MakeMemberAccess(expression, member);
    }

    /// <summary>
    /// Combines the first predicate with the second using the logical "and".
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.AndAlso);
    }

    ///*
    /// <summary>
    /// 与操作表达式
    /// </summary>
    /// <param name="left">左操作数</param>
    /// <param name="right">右操作数</param>
    public static Expression And(this Expression left, Expression right)
    {
        if (left == null)
            return right;
        if (right == null)
            return left;
        return Expression.AndAlso(left, right);
    }

    /// <summary>
    /// Combines the first predicate with the second using the logical "or".
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null)
        {
            return second;
        }

        if (second == null)
        {
            return first;
        }

        return first.Compose(second, Expression.OrElse);
    }

    /// <summary>
    /// 或操作表达式
    /// </summary>
    /// <param name="first">左操作数</param>
    /// <param name="second">右操作数</param>
    public static Expression Or(this Expression first, Expression second)
    {
        return Expression.OrElse(first, second);
    }

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                       .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }
}