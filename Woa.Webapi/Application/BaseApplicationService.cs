using AutoMapper;
using MediatR;
using Woa.Common;

namespace Woa.Webapi.Application;

public abstract class BaseApplicationService : IApplicationService
{
	protected internal LazyServiceProvider Provider { get; set; }

	protected IMediator Mediator => Provider.GetService<IMediator>();

	protected IMapper Mapper => Provider.GetService<IMapper>();

	protected IRepositoryContext Context => Provider.GetService<IRepositoryContext>();
}