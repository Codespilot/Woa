using Supabase;
using Woa.Chatbot.Services;
using Woa.Common;

namespace Woa.Chatbot;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddChatCompletionService(this IServiceCollection services)
    {
        services.AddTransient<OpenAiChatCompletionService>()
                .AddTransient<ClaudeChatCompletionService>();

        services.AddTransient<NamedService<IChatCompletionService>>(provider =>
        {
            return name => name switch
            {
                "OpenAi" => provider.GetRequiredService<OpenAiChatCompletionService>(),
                "Claude" => provider.GetRequiredService<ClaudeChatCompletionService>(),
                _ => throw new ArgumentException($"Unknown chat completion service {name}")
            };
        });

        return services;
    }
}