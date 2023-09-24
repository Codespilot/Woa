using System.Collections.Concurrent;

namespace Woa.Common;

public class LazyServiceProvider
{
	/// <summary>
	/// 
	/// </summary>
	protected ConcurrentDictionary<Type, object> CachedServices { get; set; }

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

	/// <inheritdoc />
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
		return CachedServices.GetOrAdd(serviceType, () =>
		{
			if (ServiceProvider == null)
			{
				throw new ArgumentNullException(nameof(ServiceProvider));
			}

			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			var service = ServiceProvider.GetService(serviceType);
			if (service == null)
			{
				throw new NullReferenceException(nameof(serviceType));
			}

			return service;
		});
	}

	/// <inheritdoc />
	public virtual T GetService<T>()
	{
		return (T)GetService(typeof(T));
	}

	/// <inheritdoc />
	public virtual object GetService(Type serviceType)
	{
		var service = CachedServices.GetOrAdd(serviceType, _ => ServiceProvider.GetService(serviceType));
		return service;
	}

	/// <inheritdoc />
	public virtual T GetService<T>(T defaultValue)
	{
		return (T)GetService(typeof(T), defaultValue);
	}

	/// <inheritdoc />
	public virtual object GetService(Type serviceType, object defaultValue)
	{
		return GetService(serviceType) ?? defaultValue;
	}

	/// <inheritdoc />
	public virtual T GetService<T>(Func<IServiceProvider, object> factory)
	{
		return (T)GetService(typeof(T), factory);
	}

	/// <inheritdoc />
	public virtual object GetService(Type serviceType, Func<IServiceProvider, object> factory)
	{
		return CachedServices.GetOrAdd(serviceType, () => factory(ServiceProvider));
	}
}
