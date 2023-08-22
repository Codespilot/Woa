using AutoMapper;
using MediatR;
using Polly;
using Postgrest;
using Woa.Webapi.Domain;
using Woa.Webapi.Dtos;

namespace Woa.Webapi.Application;

public class SensitiveWordApplicationService : ISensitiveWordApplicationService
{
	private readonly SupabaseClient _client;
	private readonly IMapper _mapper;
	private readonly IMediator _mediator;
	private readonly ILogger<SensitiveWordApplicationService> _logger;

	public SensitiveWordApplicationService(SupabaseClient client, IMapper mapper, IMediator mediator, ILoggerFactory logger)
	{
		_client = client;
		_mapper = mapper;
		_mediator = mediator;
		_logger = logger.CreateLogger<SensitiveWordApplicationService>();
	}

	public async Task<List<SensitiveWordItemDto>> SearchAsync(string keyword, int page, int size, CancellationToken cancellationToken = default)
	{
		var response = await Policy.Handle<Exception>()
		                           .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                           .ExecuteAsync(() =>
			                           _client.From<SensitiveWordEntity>()
			                                  .Where(t => t.Content.Contains(keyword))
			                                  .Get(cancellationToken)
		                           );

		var entities = response.Models;
		var result = _mapper.Map<List<SensitiveWordItemDto>>(entities);
		return result;
	}

	public async Task<int> CountAsync(string keyword, CancellationToken cancellationToken = default)
	{
		var response = await Policy.Handle<Exception>()
		                           .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)), OnRetry)
		                           .ExecuteAsync(() =>
			                           _client.From<SensitiveWordEntity>()
			                                  .Where(t => t.Content.Contains(keyword))
			                                  .Count(Constants.CountType.Estimated, cancellationToken)
		                           );

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

	private void OnRetry(Exception exception, TimeSpan timeSpan, int retryCount, object context)
	{
		_logger.LogError(exception, "第{RetryCount}次重试，等待{TimeSpan}后重试", retryCount, timeSpan);
	}
}