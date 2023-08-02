namespace Woa.Chatbot.Services;

public interface IChatCompletionService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> CreateCompletionAsync(string message, CancellationToken cancellationToken = default);
}