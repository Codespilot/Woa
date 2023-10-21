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

	[HttpGet]
	public async Task<IActionResult> SearchAsync([FromQuery] UserQueryDto condition, int page, int size)
	{
		var result = await _service.SearchAsync(condition, page, size, HttpContext.RequestAborted);
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetAsync(int id)
	{
		var result = await _service.GetAsync(id, HttpContext.RequestAborted);
		return Ok(result);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> SetRoleAsync(int id, [FromBody] List<int> roles)
	{
		return Ok();
	}
}