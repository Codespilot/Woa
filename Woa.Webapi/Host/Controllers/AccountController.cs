using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Woa.Webapi.Application;
using Woa.Transit;

namespace Woa.Webapi.Controllers;

/// <summary>
/// 账号控制器
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController : ControllerBase
{
	private readonly IUserApplicationService _service;

	public AccountController(IUserApplicationService service)
	{
		_service = service;
	}

	/// <summary>
	/// 获取用户信息
	/// </summary>
	/// <returns></returns>
	[HttpGet]
	public async Task<IActionResult> GetAsync()
	{
		var id = int.Parse(User.FindFirstValue(JwtClaimTypes.Subject)!);
		var entity = await _service.GetProfileAsync(id, HttpContext.RequestAborted);
		return Ok(entity);
	}

	/// <summary>
	/// 获取Token
	/// </summary>
	/// <param name="model"></param>
	/// <returns></returns>
	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<IActionResult> GrantAsync([FromBody] LoginRequestDto model)
	{
		try
		{
			var response = await _service.AuthenticateAsync(model.Username, model.Password, HttpContext.RequestAborted);

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

	/// <summary>
	/// 刷新Token
	/// </summary>
	/// <param name="token"></param>
	/// <returns></returns>
	[HttpGet("refresh")]
	[AllowAnonymous]
	public async Task<IActionResult> RefreshAsync(string token)
	{
		try
		{
			var response = await _service.AuthenticateAsync(token, HttpContext.RequestAborted);
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
}