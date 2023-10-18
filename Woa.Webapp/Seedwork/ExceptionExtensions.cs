using Refit;
using Woa.Webapp.Rest;

namespace Woa.Webapp;

internal static class ExceptionExtensions
{
    public static string GetPromptMessage(this Exception exception)
    {
        while (exception.InnerException != null)
        {
            exception = exception.InnerException;
        }

        return exception switch
        {
            HttpRequestException _ => "Unable to connect to the server",
            TaskCanceledException _ => "The request has timed out",
            OperationCanceledException _ => "The request has timed out",
            ApiException ex => ex.GetDetail()?.Message ?? ex.Message,
            _ => exception.Message
        };
    }
}