using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Webapi.Dtos;
using Woa.Webapi.Application;

namespace Woa.Webapi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
	private readonly IUserService _service;

	public AccountController(IUserService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<IActionResult> GetAsync()
	{
		var id = int.Parse(User.FindFirstValue(JwtClaimTypes.Subject)!);
		var entity = await _service.GetProfileAsync(id);
		return Ok(entity);
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> GrantAsync([FromBody] LoginRequestDto model)
	{
		try
		{
			var response = await _service.AuthenticateAsync(model.Username, model.Password);

			return Ok(response);
		}
		catch (ArgumentException exception)
		{
			return BadRequest(new { exception.Message });
		}
		catch (Exception exception)
		{
			return Unauthorized(new { exception.Message });
		}
	}

	[HttpGet("refresh")]
	[AllowAnonymous]
	public async Task<IActionResult> RefreshAsync(string token)
	{
		try
		{
			var response = await _service.AuthenticateAsync(token);
			return Ok(response);
		}
		catch (ArgumentException exception)
		{
			return BadRequest(new { exception.Message });
		}
		catch (Exception exception)
		{
			return Unauthorized(new { exception.Message });
		}
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> CreateAsync([FromBody] UserRegisterDto model)
	{
		try
		{
			var result = await _service.CreateAsync(model);
			Response.Headers.Add("x-entry-id", result.Id.ToString());
			return Ok();
		}
		catch (ArgumentException exception)
		{
			return BadRequest(new { exception.Message });
		}
		catch (Exception exception)
		{
			return StatusCode(500, new { exception.Message });
		}
	}
}