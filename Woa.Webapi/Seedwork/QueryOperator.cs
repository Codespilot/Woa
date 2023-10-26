namespace Woa.Webapi;

/// <summary>
/// 查询操作符
/// </summary>
public static class QueryOperator
{
	public const string And = "and";
	public const string Or = "or";
	public const string Equal = "eq";
	public const string GreaterThan = "gt";
	public const string GreaterThanOrEqual = "gte";
	public const string LessThan = "lt";
	public const string LessThanOrEqual = "lte";
	public const string NotEqual = "neq";
	public const string Like = "like";
	public const string ILike = "ilike";
	public const string In = "in";
	public const string Is = "is";
	public const string FTS = "fts";
	public const string PLFTS = "plfts";
	public const string PHFTS = "phfts";
	public const string WFTS = "wfts";
	public const string Contains = "cs";
	public const string ContainedIn = "cd";
	public const string Overlap = "ov";
	public const string StrictlyLeft = "sl";
	public const string StrictlyRight = "sr";
	public const string NotRightOf = "nxr";
	public const string NotLeftOf = "nxl";
	public const string Adjacent = "adj";
	public const string Not = "not";
}