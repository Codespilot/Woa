namespace Woa.Shared;

/// <summary>
/// Supabase扩展方法
/// </summary>
internal static class SupabaseExtensions
{
	// ReSharper disable once MemberCanBePrivate.Global

	/// <summary>
	/// 注册Supabase Client到DI容器
	/// </summary>
	/// <param name="services"></param>
	/// <param name="url"></param>
	/// <param name="key"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	public static IServiceCollection AddSupabaseClient(this IServiceCollection services, string url, string key)
	{
		if (string.IsNullOrWhiteSpace(url))
		{
			throw new NullReferenceException("Supabase:Url is null or empty");
		}

		if (string.IsNullOrWhiteSpace(key))
		{
			throw new NullReferenceException("Supabase:Key is null or empty");
		}

		var options = new Supabase.SupabaseOptions
		{
			AutoRefreshToken = true,
			AutoConnectRealtime = true
		};
		services.AddSingleton(_ => new SupabaseClient(url, key, options));

		return services;
	}

	/// <summary>
	/// 注册Supabase Client到DI容器
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	public static IServiceCollection AddSupabaseClient(this IServiceCollection services, IConfiguration configuration)
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

	/// <summary>
	/// 注册Supabase Client到DI容器
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	public static IServiceCollection AddSupabaseClient(this IServiceCollection services)
	{
		var options = new Supabase.SupabaseOptions
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