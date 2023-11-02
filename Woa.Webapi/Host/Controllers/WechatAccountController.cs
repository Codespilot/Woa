using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Transit;
using Woa.Webapi.Application;

namespace Woa.Webapi.Controllers;

/// <summary>
/// 微信公众号配置管理接口
/// </summary>
[Route("api/wechat/account")]
[ApiController]
[Authorize(Roles = "SA")]
public class WechatAccountController : ControllerBase
{
	private readonly IWechatAccountApplicationService _service;

	/// <summary>
	/// 初始化<see cref="WechatAccountController"/>
	/// </summary>
	/// <param name="service"></param>
	public WechatAccountController(IWechatAccountApplicationService service)
	{
		_service = service;
	}

	/// <summary>
	/// 搜索微信公众号配置
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="page"></param>
	/// <param name="size"></param>
	/// <returns></returns>
	[HttpGet("list")]
	public async Task<IActionResult> SearchAsync([FromQuery] WechatAccountQueryDto condition, [FromQuery] int page = 1, [FromQuery] int size = 20)
	{
		var result = await _service.SearchAsync(condition, page, size, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 获取微信公众号配置数量
	/// </summary>
	/// <param name="condition"></param>
	/// <returns></returns>
	[HttpGet("count")]
	public async Task<IActionResult> CountAsync([FromQuery] WechatAccountQueryDto condition)
	{
		var result = await _service.CountAsync(condition, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 获取微信公众号配置详情
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("{id}")]
	public async Task<IActionResult> GetAsync([FromRoute] string id)
	{
		var result = await _service.GetAsync(id, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 新增微信公众号配置
	/// </summary>
	/// <param name="model"></param>
	/// <returns></returns>
	[HttpPost]
	public async Task<IActionResult> CreateAsync([FromBody] WechatAccountCreateDto model)
	{
		var result = await _service.CreateAsync(model, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 更新微信公众号配置
	/// </summary>
	/// <param name="id"></param>
	/// <param name="model"></param>
	/// <returns></returns>
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] WechatAccountUpdateDto model)
	{
		await _service.UpdateAsync(id, model, HttpContext.RequestAborted);
		return Ok();
	}
}