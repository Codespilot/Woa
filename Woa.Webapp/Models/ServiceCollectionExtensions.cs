using AutoMapper;
using FluentValidation;

namespace Woa.Webapp.Models;

public static class ServiceCollectionExtensions
{
	private static readonly List<Type> _types = typeof(ServiceCollectionExtensions).Assembly?.GetTypes().ToList();

	public static IServiceCollection AddObjectMapping(this IServiceCollection services, Action<MapperConfigurationExpression> config = null)
	{
		var expression = new MapperConfigurationExpression();

		if (_types != null)
		{
			foreach (var type in _types)
			{
				if (!typeof(Profile).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract)
				{
					continue;
				}
				expression.AddProfile(type);
			}
		}

		config?.Invoke(expression);
		var mapperConfiguration = new MapperConfiguration(expression);

		var mapper = mapperConfiguration.CreateMapper();

		services.AddSingleton(mapper);
		return services;
	}

	public static IServiceCollection AddObjectValidation(this IServiceCollection services)
	{
		if (_types == null)
		{
			return services;
		}

		var implements = _types.Where(type => typeof(IValidator).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
		                      .ToList();

		foreach (var validatorType in implements)
		{
			var inheritedType = validatorType.GetInterfaces().FirstOrDefault(t => t.IsGenericType);
			if (inheritedType == null)
			{
				continue;
			}

			if (inheritedType.GenericTypeArguments.Length != 1)
			{
				continue;
			}

			var objectType = inheritedType.GenericTypeArguments[0];
			if (!objectType.IsClass || objectType.IsAbstract || objectType.IsEnum)
			{
				continue;
			}

			var interfaceType = typeof(IValidator<>).MakeGenericType(objectType);
			services.AddSingleton(interfaceType, validatorType);
		}

		return services;
	}
}