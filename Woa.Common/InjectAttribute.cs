namespace Woa.Common;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class InjectAttribute : Attribute
{
}