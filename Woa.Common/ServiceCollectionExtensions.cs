using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase;

namespace Woa.Common;

internal static class ServiceCollectionExtensions
{
    // ReSharper disable once MemberCanBePrivate.Global
    internal static IServiceCollection AddSupabaseClient(this IServiceCollection services, string url, string key)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new NullReferenceException("Supabase:Url is null or empty");
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new NullReferenceException("Supabase:Key is null or empty");
        }

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };
        services.AddSingleton(_ => new SupabaseClient(url, key, options));

        return services;
    }

    internal static IServiceCollection AddSupabaseClient(this IServiceCollection services, IConfiguration configuration)
    {
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

        return services.AddSupabaseClient(url, key);
    }

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
}