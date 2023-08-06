using Microsoft.Extensions.DependencyInjection;

namespace Woa.Common;

internal static class ServiceProviderExtensions
{
    public static TService GetService<TService>(this IServiceProvider provider, string name, bool newScope = false)
    {
        if (!newScope)
        {
            return provider.GetService<NamedService<TService>>()(name);
        }

        using var scope = provider.CreateScope();
        return scope.ServiceProvider.GetService<NamedService<TService>>()(name);
    }

    public static TService GetRequiredService<TService>(this IServiceProvider provider, string name, bool newScope = false)
    {
        if (!newScope)
        {
            return provider.GetRequiredService<NamedService<TService>>()(name);
        }

        using var scope = provider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<NamedService<TService>>()(name);
    }
}