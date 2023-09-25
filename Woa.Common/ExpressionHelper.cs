using System.Linq.Expressions;

namespace Woa.Common;

public class ExpressionHelper
{
    public static TProperty GetProperty<TObject, TProperty>(TObject entity, string name)
    {
        var property = Expression.PropertyOrField(Expression.Constant(entity), name);
        var lambda = Expression.Lambda<Func<TObject, TProperty>>(property, Expression.Parameter(typeof(TObject), "source")).Compile();
        return lambda(entity);
    }

    public static Expression<Func<TObject, bool>> BuildPropertyEqualsExpression<TObject, TProperty>(TProperty property, string name)
    {
        // var parameter = Expression.Parameter(typeof(TEntity), "entity");
        // var member = Expression.PropertyOrField(parameter, "Id");
        // var expression = Expression.Call(typeof(object), nameof(Equals), new[] { member.Type }, member, Expression.Constant(id));
        // return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);

        var parameter = Expression.Parameter(typeof(TObject), "source");
        var member = Expression.PropertyOrField(parameter, name);
        var expression = Expression.Equal(member, Expression.Constant(property, member.Type));
        var predicate = Expression.Lambda<Func<TObject, bool>>(expression, parameter);
        return predicate;
    }
}