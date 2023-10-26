using Woa.Webapi.Domain;
using Woa.Transit;

namespace Woa.Webapi.Application;

public class SensitiveWordApplicationService : BaseApplicationService, ISensitiveWordApplicationService
{
	private readonly IRepository<SensitiveWordEntity, int> _repository;

	public SensitiveWordApplicationService(IRepository<SensitiveWordEntity, int> repository)
	{
		_repository = repository;
	}

	public async Task<List<SensitiveWordItemDto>> SearchAsync(string keyword, int page, int size, CancellationToken cancellationToken = default)
	{
		var offset = (page - 1) * size;

		var entities = await _repository.FindAsync(t => t.Content.Contains(keyword), offset, size, cancellationToken);
		var result = Mapper.Map<List<SensitiveWordItemDto>>(entities);
		return result;
	}

	public async Task<int> CountAsync(string keyword, CancellationToken cancellationToken = default)
	{
		var response = await _repository.CountAsync(t => t.Content.Contains(keyword), cancellationToken);
		return response;
	}

	public async Task CreateAsync(SensitiveWordCreateDto request, CancellationToken cancellationToken = default)
	{
		var command = new SensitiveWordCreateCommand(request.Content);
		await Mediator.Send(command, cancellationToken);
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var command = new SensitiveWordDeleteCommand(id);
		await Mediator.Send(command, cancellationToken);
	}
}