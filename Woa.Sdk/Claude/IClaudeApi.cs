using Refit;

namespace Woa.Sdk.Claude;

public interface IClaudeApi
{
	[Post("/v1/complete")]
	Task<ApiResponse<ChatCompletionResponse>> CreateCompletionAsync([Body] ChatCompletionRequest request, CancellationToken cancellationToken = default);
}