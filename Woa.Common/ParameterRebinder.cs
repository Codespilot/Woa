using System.Linq.Expressions;

namespace Woa.Common;

internal class ParameterRebinder : ExpressionVisitor
{
    /// <summary>
    /// The ParameterExpression map
    /// </summary>
    readonly Dictionary<ParameterExpression, ParameterExpression> _map;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRebinder"/> class.
    /// </summary>
    /// <param name="map">The map.</param>
    ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    /// <summary>
    /// Replaces the parameters.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="exp">The exp.</param>
    /// <returns>Expression</returns>
    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }

    /// <summary>
    /// Visits the parameter.
    /// </summary>
    /// <param name="node">The p.</param>
    /// <returns>Expression</returns>
    protected override Expression VisitParameter(ParameterExpression node)
    {

        if (_map.TryGetValue(node, out ParameterExpression replacement))
        {
            node = replacement;
        }

        return base.VisitParameter(node);
    }
}