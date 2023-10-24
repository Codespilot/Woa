using System.Collections.Concurrent;

namespace Woa.Common;

public class LazyServiceProvider
{
	/// <summary>
	/// 
	/// </summary>
	private ConcurrentDictionary<Type, object> CachedServices { get; set; }

	/// <summary>
	/// 
	/// </summary>
	protected IServiceProvider ServiceProvider { get; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="serviceProvider"></param>
	public LazyServiceProvider(IServiceProvider serviceProvider)
	{
		ServiceProvider = serviceProvider;
		CachedServices = new ConcurrentDictionary<Type, object>();
	}

	public virtual T GetRequiredService<T>()
	{
		return (T)GetRequiredService(typeof(T));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="serviceType"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="NullReferenceException"></exception>
	public virtual object GetRequiredService(Type serviceType)
	{
		return CachedServices.GetOrAdd(serviceType, type =>
		{
			if (ServiceProvider == null)
			{
				throw new ArgumentNullException(nameof(ServiceProvider));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			var service = ServiceProvider.GetService(type);
			if (service == null)
			{
				throw new NullReferenceException(nameof(type));
			}

			return service;
		});
	}

	public virtual T GetService<T>()
	{
		return (T)GetService(typeof(T));
	}

	public virtual object GetService(Type serviceType)
	{
		var service = CachedServices.GetOrAdd(serviceType, type => ServiceProvider.GetService(type));
		return service;
	}

	public virtual T GetService<T>(T defaultValue)
	{
		return (T)GetService(typeof(T), defaultValue);
	}

	public virtual object GetService(Type serviceType, object defaultValue)
	{
		return GetService(serviceType) ?? defaultValue;
	}

	public virtual T GetService<T>(Func<IServiceProvider, object> factory)
	{
		return (T)GetService(typeof(T), factory);
	}

	public virtual object GetService(Type serviceType, Func<IServiceProvider, object> factory)
	{
		return CachedServices.GetOrAdd(serviceType, () => factory(ServiceProvider));
	}
}