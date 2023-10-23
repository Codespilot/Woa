using System.Diagnostics;

namespace Woa.Webapp.Rest;

internal class LoggingHandler : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var response = await base.SendAsync(request, cancellationToken);
		Debug.WriteLineIf(!response.IsSuccessStatusCode, $"[{request.Method}]{request.RequestUri} ({response.StatusCode})");
		return response;
	}
}
