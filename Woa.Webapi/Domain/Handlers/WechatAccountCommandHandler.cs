using AutoMapper;

namespace Woa.Webapi.Domain;

public class WechatAccountCommandHandler : ICommandHandler<WechatAccountCreateCommand, string>,
                                           ICommandHandler<WechatAccountUpdateCommand>
{
	private readonly WechatAccountRepository _repository;
	private readonly IMapper _mapper;

	public WechatAccountCommandHandler(WechatAccountRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	public Task<string> Handle(WechatAccountCreateCommand request, CancellationToken cancellationToken)
	{
		var entity = _mapper.Map<WechatAccountEntity>(request);
		return _repository.InsertAsync(entity, cancellationToken)
		                  .ContinueWith(task => task.Result.Id, cancellationToken);
	}

	public Task Handle(WechatAccountUpdateCommand request, CancellationToken cancellationToken)
	{
		return _repository.UpdateAsync(request.Id, entity => _mapper.Map(request, entity), cancellationToken);
	}
}