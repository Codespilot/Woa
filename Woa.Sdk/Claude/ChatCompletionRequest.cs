using System.Text.Json.Serialization;

namespace Woa.Sdk.Claude;

public class ChatCompletionRequest
{
    public ChatCompletionRequest()
    {
    }

    public ChatCompletionRequest(string message)
        : this()
    {
        Prompt = $"\n\nHuman: ${message}\n\nAssistant:";
    }

    public string Model { get; set; }

    public string Prompt { get; set; }

    public bool Stream { get; set; } = false;

    [JsonPropertyName("max_tokens_to_sample")]
    public int MaxTokens { get; set; } = 256;

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; } = 1;
}