using System.Text.Json.Serialization;

namespace Woa.Sdk.OpenAi;

public class ChatCompletionResponse
{
    public string Id { get; set; }

    public string Model { get; set; }

    public List<Choice> Choices { get; set; }

    public class Choice
    {
        public int Index { get; set; }

        public Message Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }

        public string Content { get; set; }
    }
}