using System.Collections.Concurrent;

namespace Woa.Common;

public class AsyncLock
{
	private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphoreSlims = new();

	public static async Task TryAcquire(string token, Func<Task> task, CancellationToken cancellationToken = default)
	{
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}

		var semaphoreSlim = _semaphoreSlims.GetOrAdd(token, _ => new SemaphoreSlim(1));
		if (semaphoreSlim.CurrentCount == 0)
		{
			return;
		}

		try
		{
			await semaphoreSlim.WaitAsync(cancellationToken);
			await task();
		}
		finally
		{
			semaphoreSlim.Release();
		}
	}

	public static async Task Acquire(string token, Func<Task> task, CancellationToken cancellationToken = default)
	{
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}

		var semaphoreSlim = _semaphoreSlims.GetOrAdd(token, _ => new SemaphoreSlim(1));

		try
		{
			await semaphoreSlim.WaitAsync(cancellationToken);
			await task();
		}
		finally
		{
			semaphoreSlim.Release();
		}
	}

	public static async Task<TResult> Acquire<TResult>(string token, Func<Task<TResult>> task, CancellationToken cancellationToken = default)
	{
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}

		var semaphoreSlim = _semaphoreSlims.GetOrAdd(token, _ => new SemaphoreSlim(1));

		try
		{
			await semaphoreSlim.WaitAsync(cancellationToken);
			return await task();
		}
		finally
		{
			semaphoreSlim.Release();
		}
	}
}
