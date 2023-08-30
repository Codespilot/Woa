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
	private readonly IUserApplicationService _applicationService;

	public AccountController(IUserApplicationService applicationService)
	{
		_applicationService = applicationService;
	}

	[HttpGet]
	public async Task<IActionResult> GetAsync()
	{
		var id = int.Parse(User.FindFirstValue(JwtClaimTypes.Subject)!);
		var entity = await _applicationService.GetProfileAsync(id, HttpContext.RequestAborted);
		return Ok(entity);
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> GrantAsync([FromBody] LoginRequestDto model)
	{
		try
		{
			var response = await _applicationService.AuthenticateAsync(model.Username, model.Password, HttpContext.RequestAborted);

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
			var response = await _applicationService.AuthenticateAsync(token, HttpContext.RequestAborted);
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
			var result = await _applicationService.CreateAsync(model, HttpContext.RequestAborted);
			Response.Headers.Add("x-entry-id", result.ToString());
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