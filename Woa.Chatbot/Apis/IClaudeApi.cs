﻿using Refit;
using Woa.Chatbot.Models.Claude;

namespace Woa.Chatbot.Apis;

public interface IClaudeApi
{
    [Post("/v1/complete")]
    //[Headers("anthropic-version: 2023-06-01")]
    Task<ApiResponse<ChatCompletionResponse>> CreateCompletionAsync([Body] ChatCompletionRequest request, CancellationToken cancellationToken = default);
}