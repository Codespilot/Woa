using Refit;
using Woa.Chatbot.Models.ChatGpt;

namespace Woa.Chatbot.Apis;

public interface IChatGptApi
{
    [Post("/v1/chat/completions")]
    Task<ApiResponse<ChatCompletionResponse>> CreateCompletionAsync([Body] ChatCompletionRequest request, CancellationToken cancellationToken = default);
}