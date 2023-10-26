using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Woa.Transit;
using Woa.Webapi.Application;

namespace Woa.Webapi.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SA")]
public class RoleController : ControllerBase
{
	private readonly IRoleApplicationService _service;

	public RoleController(IRoleApplicationService service)
	{
		_service = service;
	}

	/// <summary>
	/// 搜索角色
	/// </summary>
	/// <param name="condition">查询条件</param>
	/// <param name="page">页码</param>
	/// <param name="size">数量</param>
	/// <returns></returns>
	[HttpGet("list")]
	public async Task<IActionResult> SearchAsync([FromQuery] RoleQueryDto condition, int page, int size)
	{
		var result = await _service.SearchAsync(condition, page, size, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 获取角色数量
	/// </summary>
	/// <param name="condition"></param>
	/// <returns></returns>
	[HttpGet("count")]
	public async Task<IActionResult> CountAsync([FromQuery] RoleQueryDto condition)
	{
		var result = await _service.CountAsync(condition, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 获取角色信息
	/// </summary>
	/// <param name="id">角色Id</param>
	/// <returns></returns>
	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetAsync(int id)
	{
		var result = await _service.GetAsync(id, HttpContext.RequestAborted);
		return Ok(result);
	}

	/// <summary>
	/// 创建角色
	/// </summary>
	/// <param name="model">数据模型</param>
	/// <returns></returns>
	[HttpPost]
	public async Task<IActionResult> CreateAsync([FromBody] RoleEditDto model)
	{
		var result = await _service.CreateAsync(model, HttpContext.RequestAborted);
		//HttpContext.Response.Headers.Add("Location", $"{HttpContext.Request.Path}/{result}");
		Response.Headers.Add("x-entry-id", result.ToString());
		return Ok();
	}

	/// <summary>
	/// 更新角色
	/// </summary>
	/// <param name="id">角色Id</param>
	/// <param name="model">数据模型</param>
	/// <returns></returns>
	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateAsync(int id, [FromBody] RoleEditDto model)
	{
		await _service.UpdateAsync(id, model, HttpContext.RequestAborted);
		return Ok();
	}

	/// <summary>
	/// 删除角色
	/// </summary>
	/// <param name="id">角色Id</param>
	/// <returns></returns>
	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteAsync(int id)
	{
		await _service.DeleteAsync(id, HttpContext.RequestAborted);
		return Ok();
	}
}