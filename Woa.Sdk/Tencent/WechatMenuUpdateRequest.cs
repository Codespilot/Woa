using Newtonsoft.Json;

namespace Woa.Sdk.Tencent;

public class WechatMenuUpdateRequest
{
	[JsonProperty("button")]
	public List<WechatMenu> Menus { get; set; } = new();
}