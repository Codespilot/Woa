using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Webapi.Application;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MenuController : ControllerBase
{
	private readonly IWechatMenuApplicationService _service;

	public MenuController(IWechatMenuApplicationService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<IActionResult> SearchAsync([FromQuery] WechatMenuQueryDto condition, int page = 1, int size = 10)
	{
		var result = await _service.SearchAsync(condition, page, size);
		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> CreateAsync([FromBody] WechatMenuEditDto model)
	{
		var id = await _service.CreateAsync(model, HttpContext.RequestAborted);
		Response.Headers.Add("Location", $"/api/menu/{id}");
		return Ok();
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateAsync(int id, [FromBody] WechatMenuEditDto model)
	{
		await _service.UpdateAsync(id, model, HttpContext.RequestAborted);
		return Ok();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteAsync(int id)
	{
		await _service.DeleteAsync(id, HttpContext.RequestAborted);
		return Ok();
	}
}