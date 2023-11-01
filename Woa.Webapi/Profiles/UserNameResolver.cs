using AutoMapper;

namespace Woa.Webapi.Profiles;

public class UserNameResolver<TSource, TDestination, TMember> : IMemberValueResolver<TSource, TDestination, TMember, string>
{
    public string Resolve(TSource source, TDestination destination, TMember sourceMember, string destMember, ResolutionContext context)
    {
        if (!context.Items.TryGetValue("users", out var value))
        {
            return null;
        }

        var users = (Dictionary<int, string>)value;

        var userId = sourceMember switch
        {
            int id => id,
            _ => 0
        };

        return users.TryGetValue(userId, out var username) ? username : null;
    }
}