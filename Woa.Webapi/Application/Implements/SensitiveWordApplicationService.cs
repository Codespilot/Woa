using AutoMapper;
using MediatR;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class SensitiveWordApplicationService : ISensitiveWordApplicationService
{
	private readonly IRepository<SensitiveWordEntity, int> _repository;
	private readonly IMapper _mapper;
	private readonly IMediator _mediator;

	public SensitiveWordApplicationService(IRepository<SensitiveWordEntity, int> repository, IMapper mapper, IMediator mediator)
	{
		_repository = repository;
		_mapper = mapper;
		_mediator = mediator;
	}

	public async Task<List<SensitiveWordItemDto>> SearchAsync(string keyword, int page, int size, CancellationToken cancellationToken = default)
	{
		var offset = (page - 1) * size;

		var entities = await _repository.FindAsync(t => t.Content.Contains(keyword), offset, size, cancellationToken);
		var result = _mapper.Map<List<SensitiveWordItemDto>>(entities);
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
		await _mediator.Send(command, cancellationToken);
	}

	public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
	{
		var command = new SensitiveWordDeleteCommand(id);
		await _mediator.Send(command, cancellationToken);
	}
}