namespace Woa.Common;

/// <summary>
/// The expression conditional operation type.
/// </summary>
public enum PredicateOperator
{
    /// <summary>
    /// Represents a conditional AND operation that evaluates the second operand only if the first operand evaluates to true.
    /// </summary>
    AndAlso,

    /// <summary>
    /// Represents a conditional OR operation that evaluates the second operand only if the first operand evaluates to false.
    /// </summary>
    OrElse
}