using MediatR;

namespace Woa.Webapi.Domain;

public sealed record RoleDeleteCommand(int Id) : ICommand;