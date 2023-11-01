namespace Woa.Webapi;

public interface ICreateAudit
{
	/// <summary>
	/// 创建人Id
	/// </summary>
	object CreateBy { get; set; }

	/// <summary>
	/// 创建时间
	/// </summary>
	DateTime CreateAt { get; set; }
}

public interface ICreateAudit<TKey> : ICreateAudit
{
	/// <summary>
	/// 创建人Id
	/// </summary>
	new TKey CreateBy { get; set; }
}