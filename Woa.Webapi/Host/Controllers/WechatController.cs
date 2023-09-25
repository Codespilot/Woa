using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Common;
using Woa.Sdk.Wechat;
using Woa.Webapi.Application;

namespace Woa.Webapi.Controllers;

/// <summary>
/// 微信消息交互接口Controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class WechatController : ControllerBase
{
	private readonly IServiceProvider _provider;
	private readonly IConfiguration _configuration;
	private readonly IWechatMessageApplicationService _service;

	/// <summary>
	/// 初始化<see cref="WechatController"/>实例
	/// </summary>
	/// <param name="provider"></param>
	/// <param name="configuration"></param>
	/// <param name="service"></param>
	public WechatController(IServiceProvider provider, IConfiguration configuration, IWechatMessageApplicationService service)
	{
		_provider = provider;
		_configuration = configuration;
		_service = service;
	}

	/// <summary>
	/// 验证消息的确来自微信服务器
	/// </summary>
	/// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
	/// <param name="echostr">随机字符串</param>
	/// <param name="nonce">随机数</param>
	/// <param name="timestamp">时间戳</param>
	/// <returns></returns>
	[HttpGet]
	public async Task<IActionResult> Handle(string signature, string echostr, string nonce, string timestamp)
	{
		var token = _configuration.GetValue<string>("Wechat:Token");

		var parameters = new List<string> { token, timestamp, nonce };
		parameters.Sort();

		var res = string.Join("", parameters);

		var asciiBytes = Encoding.ASCII.GetBytes(res);
		var hashBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")!).ComputeHash(asciiBytes);
		var builder = new StringBuilder();
		foreach (var @byte in hashBytes)
		{
			builder.Append(@byte.ToString("x2"));
		}

		var result = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
		if (string.Equals(result, signature, StringComparison.OrdinalIgnoreCase))
		{
			return await Task.FromResult(Ok(echostr));
		}

		{
		}

		return await Task.FromResult(BadRequest());
	}

	/// <summary>
	/// 接收微信消息
	/// </summary>
	/// <returns></returns>
	[HttpPost]
	public async Task<IActionResult> Handle()
	{
		Debug.WriteLine(DateTime.Now);
		var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();

		var message = WechatMessage.Parse(requestBody);

		var response = WechatMessage.Empty;

		var handler = _provider.GetService<IWechatMessageHandler>(message.MessageType.ToString());
		if (handler != null)
		{
			response = await handler.HandleAsync(message);
		}

		if (response == null || response.Count < 1)
		{
			return Ok();
		}

		response[WechatMessageKey.ToUserName] = message.GetValue<string>(WechatMessageKey.FromUserName);
		response[WechatMessageKey.FromUserName] = _configuration.GetValue<string>("Wechat:OpenId");
		response[WechatMessageKey.CreateTime] = DateTimeOffset.Now.ToUnixTimeSeconds();
		return Content(response.ToXml(), "text/xml");
	}

	/// <summary>
	/// 查看消息回复
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("{id:long}/reply")]
	public async Task<IActionResult> Reply(long id)
	{
		var result = await _service.GetReplyAsync(id);
		return Content(result ?? "正在处理您的请求，请耐心等待...", "text/plain", Encoding.UTF8);
	}
}