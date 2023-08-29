using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Webapi.Application;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SensitiveWordController : ControllerBase
{
	private readonly ISensitiveWordApplicationService _service;

	public SensitiveWordController(ISensitiveWordApplicationService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<IActionResult> Search(string keyword, int page = 1, int size = 10)
	{
		var result = await _service.SearchAsync(keyword, page, size, HttpContext.RequestAborted);
		return Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] SensitiveWordCreateDto request)
	{
		await _service.CreateAsync(request, HttpContext.RequestAborted);
		return Ok();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete([FromRoute] int id)
	{
		await _service.DeleteAsync(id, HttpContext.RequestAborted);
		return Ok();
	}
}