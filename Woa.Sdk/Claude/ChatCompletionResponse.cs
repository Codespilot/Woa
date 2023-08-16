using System.Text.Json.Serialization;

namespace Woa.Sdk.Claude;

/// <summary>
/// Represents a response from the Claude API.
/// </summary>
public class ChatCompletionResponse
{
    /// <summary>
    /// 
    /// </summary>
    public string Completion { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Model { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public CompletionError Error { get; set; }

    public class CompletionError
    {
        public string Message { get; set; }

        public string Type { get; set; }
    }
}