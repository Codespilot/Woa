﻿using Woa.Sdk.Claude;

namespace Woa.Chatbot.Services;

public class ClaudeChatCompletionService : IChatCompletionService
{
    private readonly IClaudeApi _api;
    private readonly IConfiguration _configuration;

    public ClaudeChatCompletionService(IClaudeApi api, IConfiguration configuration)
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