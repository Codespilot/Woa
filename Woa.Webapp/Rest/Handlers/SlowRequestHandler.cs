using System.Diagnostics;

namespace Woa.Webapp.Rest;

internal class SlowRequestHandler : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var stopwatch = Stopwatch.StartNew();
		stopwatch.Start();
		var response = await base.SendAsync(request, cancellationToken);
		stopwatch.Stop();

		if (stopwatch.Elapsed.TotalSeconds > 3)
		{
			Debug.WriteLine($"SlowRequest ({stopwatch.Elapsed.TotalSeconds}s) {request.RequestUri}");
		}

		return response;
	}
}
