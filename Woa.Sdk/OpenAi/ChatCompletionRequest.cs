namespace Woa.Sdk.OpenAi;

public class ChatCompletionRequest
{
    public ChatCompletionRequest()
    {
    }

    public ChatCompletionRequest(string message)
    {
        Messages = new List<Message>
        {
            new() { Content = message }
        };
    }

    public string Model { get; set; }

    public bool Stream { get; set; }

    //public string[]? Prompt { get; set; }

    public List<Message> Messages { get; set; }

    public class Message
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; }
    }
}