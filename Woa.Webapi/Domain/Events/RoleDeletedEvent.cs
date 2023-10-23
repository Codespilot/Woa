namespace Woa.Webapi.Domain;

public record RoleDeletedEvent(int Id) : IEvent;