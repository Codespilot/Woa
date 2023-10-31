using System.Security.Claims;
using IdentityModel;

namespace Woa.Webapi;

public class IdentityContext : IIdentityContext
{
	private readonly IHttpContextAccessor _accessor;

	public IdentityContext(IHttpContextAccessor accessor)
	{
		_accessor = accessor;
	}

	public int Id
	{
		get
		{
			var value = _accessor.HttpContext?.User?.FindFirstValue(JwtClaimTypes.Subject);
			return int.TryParse(value, out var id) ? id : 0;
		}
	}

	public string UserName => _accessor.HttpContext?.User?.FindFirstValue(JwtClaimTypes.Name) ?? string.Empty;

	public bool IsAuthenticated => _accessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

	public IEnumerable<string> Roles => _accessor.HttpContext?.User?.FindAll(JwtClaimTypes.Role).Select(x => x.Value) ?? Enumerable.Empty<string>();

	public bool IsInRole(string role)
	{
		return _accessor.HttpContext?.User?.IsInRole(role) == true;
	}
}