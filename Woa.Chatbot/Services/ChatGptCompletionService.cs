using Woa.Chatbot.Apis;
using Woa.Chatbot.Models.ChatGpt;

namespace Woa.Chatbot.Services;

public class ChatGptCompletionService : IChatCompletionService
{
    private readonly IChatGptApi _api;
    private readonly IConfiguration _configuration;

    public ChatGptCompletionService(IChatGptApi api, IConfiguration configuration)
    {
        _api = api;
        _configuration = configuration;
    }

    public async Task<string> CreateCompletionAsync(string message, CancellationToken cancellationToken = default)
    {
        var request = new ChatCompletionRequest(message) { Model = _configuration["Bot:ChatGPT:Model"] };
        var response = await _api.CreateCompletionAsync(request, cancellationToken);
        var result = await response.EnsureSuccessStatusCodeAsync();
        return result.Content!.Choices[0].Message.Content;
    }
}