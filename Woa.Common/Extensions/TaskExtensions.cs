using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Woa.Common;

public static class TaskExtensions
{
	// public static async Task Subscribe<T>(this Task<T> task, Action<T> next, Action<Exception> error = null)
	// {
	// 	task.ToObservable()
	// 	    .SubscribeOn(Scheduler.Default)
	// 	    .ObserveOn(MainScheduler.Instance)
	// 	    .Subscribe(next, e => error?.Invoke(e), () => { });
	// }
	//
	// public static async Task Subscribe(this Task task, Action next, Action<Exception> error = null)
	// {
	// 	task.ToObservable()
	// 	    .SubscribeOn(Scheduler.Default)
	// 	    .ObserveOn(await MainThread.GetMainThreadSynchronizationContextAsync())
	// 	    .Subscribe(_ => next(), e => error?.Invoke(e), () => { });
	// }
	
	public static async Task Catch(this Task task, Action<Exception> onError)
	{
		try
		{
			await task;
		}
		catch (Exception exception)
		{
			onError(exception);
		}
	}

	public static async Task<T> Catch<T>(this Task<T> task, Action<Exception> onError)
	{
		try
		{
			return await task;
		}
		catch (Exception exception)
		{
			onError(exception);
			return default;
		}
	}

	public static async Task<T> IgnoreExceptionAsync<T>(this Task<T> task, Action<Exception> error = null)
	{
		try
		{
			return await task;
		}
		catch (Exception exception)
		{
			error?.Invoke(exception);
			return default;
		}
	}

	public static async Task IgnoreExceptionAsync(this Task task, Action<Exception> error = null)
	{
		try
		{
			await task;
		}
		catch (Exception exception)
		{
			error?.Invoke(exception);
		}
	}
}