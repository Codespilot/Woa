using Woa.Transit;

namespace Woa.Webapi.Application;

public interface IWechatAccountApplicationService
{
	Task<List<WechatAccountItemDto>> SearchAsync(WechatAccountQueryDto condition, int page, int size, CancellationToken cancellationToken = default);

	Task<int> CountAsync(WechatAccountQueryDto condition, CancellationToken cancellationToken = default);

	Task<WechatAccountDetailDto> GetAsync(string id, CancellationToken cancellationToken = default);

	Task<string> CreateAsync(WechatAccountCreateDto model, CancellationToken cancellationToken = default);

	Task UpdateAsync(string id, WechatAccountUpdateDto model, CancellationToken cancellationToken = default);

	Task SetValidityAsync(string id, bool validity, CancellationToken cancellationToken = default);
}
