using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Woa.Common;

namespace Woa.Webapi.Application;

public abstract class BaseApplicationService : IApplicationService
{
	protected internal LazyServiceProvider ServiceProvider { get; set; }

	protected IMediator Mediator => ServiceProvider.GetService<IMediator>();

	protected IMapper Mapper => ServiceProvider.GetService<IMapper>();

	protected IRepositoryContext Context => ServiceProvider.GetService<IRepositoryContext>();

	protected IMemoryCache Cache => ServiceProvider.GetService<IMemoryCache>();

	protected ILogger Logger => ServiceProvider.GetService<ILoggerFactory>().CreateLogger(GetType());
}