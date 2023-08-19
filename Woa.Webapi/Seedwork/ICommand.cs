using MediatR;

namespace Woa.Webapi;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}