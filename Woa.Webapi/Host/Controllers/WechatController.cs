using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Common;
using Woa.Sdk.Tencent;

namespace Woa.Webapi.Controllers;

/// <summary>
/// 微信消息交互接口Controller
/// </summary>
[Route("api/[controller]/{id}")]
[ApiController]
[AllowAnonymous]
public class WechatController : ControllerBase
{
	private readonly IServiceProvider _provider;
	private readonly WechatOptions _options;

	/// <summary>
	/// 初始化<see cref="WechatController"/>实例
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="options"></param>
	public WechatController(IServiceProvider provider, WechatOptions options)
	{
		_provider = provider;
		_options = options;
	}

	/// <summary>
	/// 接收微信消息
	/// </summary>
	/// <param name="id"></param>
	/// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
	/// <param name="timestamp">时间戳</param>
	/// <param name="nonce">随机数</param>
	/// <returns>
	///	<para>GET: 返回请求Url的echostr参数</para>
	/// <para>POST: 处理消息并返回结果</para>
	/// </returns>
	[HttpGet, HttpPost]
	public async Task<IActionResult> HandleAsync([FromRoute] string id, string signature, string timestamp, string nonce)
	{
		var isValid = Utility.VerifySignature(signature, timestamp, nonce, _options.Accounts[id].AppToken);

		if (!isValid)
		{
			return BadRequest();
		}

		return Request.Method.Normalize(TextCaseType.Upper) switch
		{
			"GET" => Ok(Request.Query["echostr"].ToString()),
			"POST" => await Task.Run<IActionResult>(async () =>
			{
				var payload = await new StreamReader(Request.Body).ReadToEndAsync();

				var message = WechatMessage.Parse(payload);

				var response = WechatMessage.Empty;

				var handler = _provider.GetService<IWechatMessageHandler>(message.MessageType.ToString());
				if (handler != null)
				{
					response = await handler.HandleAsync(message, payload);
				}

				if (response == null || response.Count < 1)
				{
					return Ok();
				}

				response[WechatMessageKey.ToUserName] = message.GetValue<string>(WechatMessageKey.FromUserName);
				response[WechatMessageKey.FromUserName] = message.GetValue<string>(WechatMessageKey.ToUserName);
				response[WechatMessageKey.CreateTime] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				return Content(response.ToXml(), "text/xml");
			}),
			_ => StatusCode(StatusCodes.Status405MethodNotAllowed)
		};
	}
}