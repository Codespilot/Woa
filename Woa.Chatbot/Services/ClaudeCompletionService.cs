using Woa.Chatbot.Apis;
using Woa.Chatbot.Models.Claude;

namespace Woa.Chatbot.Services;

public class ClaudeCompletionService : IChatCompletionService
{
    private readonly IClaudeApi _api;
    private readonly IConfiguration _configuration;

    public ClaudeCompletionService(IClaudeApi api, IConfiguration configuration)
    {
        _api = api;
        _configuration = configuration;
    }

    public async Task<string> CreateCompletionAsync(string message, CancellationToken cancellationToken = default)
    {
        var request = new ChatCompletionRequest(message) { Model = _configuration["Bot:Claude:Model"] };
        var response = await _api.CreateCompletionAsync(request, cancellationToken);
        var result = await response.EnsureSuccessStatusCodeAsync();
        return result.Content!.Completion;
    }
}