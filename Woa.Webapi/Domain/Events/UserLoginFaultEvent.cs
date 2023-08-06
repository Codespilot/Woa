using MediatR;

namespace Woa.Webapi.Domain;

public class UserLoginFaultEvent : INotification
{
    public UserLoginFaultEvent(long userId)
    {
        UserId = userId;
    }

    public long UserId { get; }
}