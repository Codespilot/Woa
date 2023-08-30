using Newtonsoft.Json;

namespace Woa.Sdk.Wechat;

public class WechatMenuUpdateRequest
{
	[JsonProperty("button")]
	public List<WechatMenu> Menus { get; set; } = new();
}