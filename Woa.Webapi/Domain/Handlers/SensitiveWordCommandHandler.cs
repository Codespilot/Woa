using Polly;
using Woa.Common;

namespace Woa.Webapi.Domain;

public class SensitiveWordCommandHandler : ICommandHandler<SensitiveWordCreateCommand>,
                                           ICommandHandler<SensitiveWordDeleteCommand>
{
	private readonly SupabaseClient _client;
	private readonly ILogger<SensitiveWordCommandHandler> _logger;

	public SensitiveWordCommandHandler(SupabaseClient client, ILoggerFactory logger)
	{
		_client = client;
		_logger = logger.CreateLogger<SensitiveWordCommandHandler>();
	}

	public async Task Handle(SensitiveWordCreateCommand request, CancellationToken cancellationToken)
	{
		var entity = new SensitiveWordEntity
		{
			Content = request.Content
		};

		await Policy.Handle<Exception>()
		            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		            .ExecuteAsync(() => _client.From<SensitiveWordEntity>().Insert(entity, cancellationToken: cancellationToken));
	}

	public async Task Handle(SensitiveWordDeleteCommand request, CancellationToken cancellationToken)
	{
		await Policy.Handle<Exception>()
		            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		            .ExecuteAsync(async () =>
		            {
			            await _client.From<SensitiveWordEntity>()
			                         .Where(t => t.Id == request.SensitiveWordId)
			                         .Delete(cancellationToken: cancellationToken);
		            });
	}

	private void OnRetry(Exception exception, TimeSpan timeSpan, int retryCount, object context)
	{
		_logger.LogError(exception, "第{RetryCount}次重试，等待{TimeSpan}后重试", retryCount, timeSpan);
	}
}