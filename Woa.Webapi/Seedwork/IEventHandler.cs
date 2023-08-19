using MediatR;

namespace Woa.Webapi;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
	where TEvent : IEvent
{
}