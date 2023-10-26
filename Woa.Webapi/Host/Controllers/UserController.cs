using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Woa.Transit;
using Woa.Webapi.Application;

namespace Woa.Webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
	private readonly IUserApplicationService _service;

	public UserController(IUserApplicationService service)
	{
		_service = service;
	}

	/// <summary>
	/// 查询用户列表
	/// </summary>
	/// <param name="condition"></param>
	/// <param name="page"></param>
	/// <param name="size"></param>
	/// <returns></returns>
	[HttpGet("list")]
	public async Task<IActionResult> SearchAsync([FromQuery] UserQueryDto condition, int page, int size)
	{
		var result = await _service.SearchAsync(condition, page, size, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 查询用户数量
	/// </summary>
	/// <param name="condition"></param>
	/// <returns></returns>
	[HttpGet("count")]
	public async Task<IActionResult> CountAsync([FromQuery] UserQueryDto condition)
	{
		var result = await _service.CountAsync(condition, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 获取用户详细信息
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetAsync(int id)
	{
		var result = await _service.GetAsync(id, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 新增用户
	/// </summary>
	/// <param name="model"></param>
	/// <returns></returns>
	[HttpPost]
	[Authorize(Roles = "SA")]
	public async Task<IActionResult> CreateAsync([FromBody] UserCreateDto model)
	{
		var result = await _service.CreateAsync(model, HttpContext.RequestAborted);
		Response.Headers.Add("x-entry-id", result.ToString());
		return Ok();
	}

	[HttpPut("{id:int}/role")]
	public async Task<IActionResult> SetRoleAsync(int id, [FromBody] List<int> roles)
	{
		return Ok();
	}
}