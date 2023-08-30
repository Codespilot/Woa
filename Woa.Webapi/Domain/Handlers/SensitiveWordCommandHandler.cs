namespace Woa.Webapi.Domain;

public class SensitiveWordCommandHandler : ICommandHandler<SensitiveWordCreateCommand>,
                                           ICommandHandler<SensitiveWordDeleteCommand>
{
	private readonly IRepository<SensitiveWordEntity, int> _repository;

	public SensitiveWordCommandHandler(IRepository<SensitiveWordEntity, int> repository)
	{
		_repository = repository;
	}

	public async Task Handle(SensitiveWordCreateCommand request, CancellationToken cancellationToken)
	{
		var entity = new SensitiveWordEntity
		{
			Content = request.Content
		};

		await _repository.InsertAsync(entity, cancellationToken);
	}

	public async Task Handle(SensitiveWordDeleteCommand request, CancellationToken cancellationToken)
	{
		await _repository.DeleteAsync(request.SensitiveWordId, cancellationToken);
	}
}