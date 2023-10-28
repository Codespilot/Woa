namespace Woa.Webapi.Domain;

public class WechatMessageCommandHandler : ICommandHandler<WechatMessageDeleteCommand>,
										   ICommandHandler<WechatMessageReplyCommand>
{
	private readonly WechatMessageRepository _repository;

	public WechatMessageCommandHandler(WechatMessageRepository repository)
	{
		_repository = repository;
	}

	public async Task Handle(WechatMessageDeleteCommand request, CancellationToken cancellationToken)
	{
		await _repository.DeleteAsync(request.Id, cancellationToken);
	}

	public async Task Handle(WechatMessageReplyCommand request, CancellationToken cancellationToken)
	{
		await _repository.UpdateAsync(request.Id, entity =>
		{
			if (string.IsNullOrWhiteSpace(entity.Reply))
			{
				entity.Reply = request.Content;
			}
			else
			{
				entity.Reply = $"{entity.Reply}{Environment.NewLine}-------{Environment.NewLine}{request.Content}";
			}
		}, cancellationToken);
	}
}