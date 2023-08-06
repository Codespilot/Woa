using System.Net.Http.Headers;
using Refit;
using Woa.Chatbot.Apis;
using Woa.Chatbot.Services;
using Woa.Common;

namespace Woa.Chatbot;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddChatGptApi(this IServiceCollection services)
    {
        services.AddRefitClient<IChatGptApi>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    client.BaseAddress = new Uri("https://api.openai.com");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["Bot:ChatGPT:Token"]);
                    client.DefaultRequestHeaders.Add("openai-organization", configuration["Bot:ChatGPT:Organization"]);
                });

        return services;
    }

    internal static IServiceCollection AddClaudeApi(this IServiceCollection services)
    {
        services.AddRefitClient<IClaudeApi>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var configuration = provider.GetRequiredService<IConfiguration>();
                    client.BaseAddress = new Uri("https://api.anthropic.com");
                    client.DefaultRequestHeaders.Add("anthropic-version", configuration["Bot:Claude:Version"]);
                    client.DefaultRequestHeaders.Add("x-api-key", configuration["Bot:Claude:Key"]);
                });
        return services;
    }

    internal static IServiceCollection AddChatCompletionService(this IServiceCollection services)
    {
        services.AddTransient<ChatGptCompletionService>()
                .AddTransient<ClaudeCompletionService>();

        services.AddTransient<NamedService<IChatCompletionService>>(provider =>
        {
            return name => name switch
            {
                "ChatGPT" => provider.GetRequiredService<ChatGptCompletionService>(),
                "Claude" => provider.GetRequiredService<ClaudeCompletionService>(),
                _ => throw new ArgumentException($"Unknown chat completion service {name}")
            };
        });

        return services;
    }
}