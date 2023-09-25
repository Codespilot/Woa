using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Webapi.Application;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Controllers;

/// <summary>
/// 微信消息管理接口
/// </summary>
[Route("api/wechat/message")]
[ApiController]
[Authorize]
public class WechatMessageController : Controller
{
	private readonly IWechatMessageApplicationService _service;

	/// <summary>
	/// 初始化<see cref="WechatMessageController"/>.
	/// </summary>
	/// <param name="service"></param>
	public WechatMessageController(IWechatMessageApplicationService service)
	{
		_service = service;
	}

	/// <summary>
	/// 搜索微信消息
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="page"></param>
	/// <param name="size"></param>
	/// <returns></returns>
	[HttpGet]
	public async Task<IActionResult> SearchAsync([FromQuery] WechatMessageQueryDto condition, [FromQuery] int page = 1, [FromQuery] int size = 20)
	{
		var result = await _service.SearchAsync(condition, page, size, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 获取微信消息详情
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("{id:long}")]
	public async Task<IActionResult> GetAsync([FromRoute] long id)
	{
		var result = await _service.GetAsync(id, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 回复微信消息
	/// </summary>
	/// <param name="id"></param>
	/// <param name="dto"></param>
	/// <returns></returns>
	[HttpPut("{id:long}")]
	public async Task<IActionResult> ReplyAsync([FromRoute] long id, [FromBody] WechatMessageReplyDto dto)
	{
		await _service.ReplyAsync(id, dto.Content, HttpContext.RequestAborted);
		return Ok();
	}

	/// <summary>
	/// 删除微信消息
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpDelete("{id:long}")]
	public async Task<IActionResult> DeleteAsync([FromRoute] long id)
	{
		await _service.DeleteAsync(id, HttpContext.RequestAborted);
		return Ok();
	}
}