using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Woa.Common;

namespace Woa.Webapp;

public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
	private readonly ILocalStorageService _storageService;

	public IdentityAuthenticationStateProvider(ILocalStorageService storageService)
	{
		_storageService = storageService;
	}

	public async Task SetAuthenticationStateAsync(string accessToken, string refreshToken)
	{
		await _storageService.SetItemAsStringAsync(Constants.LocalStorage.AccessToken, accessToken);
		await _storageService.SetItemAsStringAsync(Constants.LocalStorage.RefreshToken, refreshToken);

		var jwt = TokenHelper.Resolve(accessToken);
		var identity = new ClaimsIdentity(jwt.Claims, "jwt");
		var user = new ClaimsPrincipal(identity);

		var authState = Task.FromResult(new AuthenticationState(user));
		NotifyAuthenticationStateChanged(authState);
	}

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		ClaimsIdentity identity;

		var token = await _storageService.GetItemAsStringAsync(Constants.LocalStorage.AccessToken);
		var jwt = TokenHelper.Resolve(token);
		if (jwt != null && jwt.ValidTo > DateTime.UtcNow)
		{
			identity = new ClaimsIdentity(jwt.Claims, "jwt");
		}
		else
		{
			identity = new ClaimsIdentity();
		}

		return new AuthenticationState(new ClaimsPrincipal(identity));
	}
}