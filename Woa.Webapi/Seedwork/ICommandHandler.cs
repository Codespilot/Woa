using MediatR;

namespace Woa.Webapi;

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
	where TCommand : ICommand<TResponse>, IRequest<TResponse>
{
}

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
	where TCommand : ICommand<Unit>, IRequest
{
}