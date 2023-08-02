using System.Net.Http.Headers;
using Refit;
using Supabase;
using Woa.Chatbot.Apis;

namespace Woa.Chatbot;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSupabaseClient(this IServiceCollection services)
    {
        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };
        return services.AddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var url = configuration["Supabase:Url"];
            var key = configuration["Supabase:Key"];
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new NullReferenceException("Supabase:Url is null or empty");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new NullReferenceException("Supabase:Key is null or empty");
            }

            return new SupabaseClient(url, key, options);
        });
    }

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
}