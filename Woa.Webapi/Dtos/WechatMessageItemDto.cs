namespace Woa.Webapi.Dtos;

public class WechatMessageItemDto
{
	public long Id { get; set; }

	public string Type { get; set; }

	public string OpenId { get; set; }

	public DateTime CreateTime { get; set; }

	public bool HasReply { get; set; }
}