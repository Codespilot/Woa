using Refit;

namespace Woa.Sdk.OpenAi;

public interface IOpenAiApi
{
	[Post("/v1/chat/completions")]
	Task<ApiResponse<ChatCompletionResponse>> CreateCompletionAsync([Body] ChatCompletionRequest request, CancellationToken cancellationToken = default);
}