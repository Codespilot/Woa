namespace Woa.Common;

internal static class DynamicTypeCreator
{
	public static DynamicTypeCreatorBase.IBaseObject Create(string className, Type parentType = null)
	{
		return DynamicTypeCreatorBase.Create(className, parentType);
	}
}