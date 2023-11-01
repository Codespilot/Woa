namespace Woa.Webapi;

public interface IUpdateAudit
{
	/// <summary>
	/// 更新人Id
	/// </summary>
	object UpdateBy { get; set; }

	/// <summary>
	/// 更新时间
	/// </summary>
	DateTime? UpdateAt { get; set; }
}

public interface IUpdateAudit<TKey> : IUpdateAudit
{
	/// <summary>
	/// 更新人Id
	/// </summary>
	new TKey UpdateBy { get; set; }
}