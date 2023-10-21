using Woa.Transit;

namespace Woa.Webapi.Application;

public class RoleApplicationService : IRoleApplicationService
{
    public Task<int> CreateAsync(RoleEditDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(int id, RoleEditDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<RoleInfoDto> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<RoleInfoDto>> SearchAsync(RoleQueryDto condition, int page, int size, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}