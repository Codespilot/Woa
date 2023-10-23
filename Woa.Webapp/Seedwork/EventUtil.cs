using Microsoft.AspNetCore.Components;

namespace Woa.Webapp;

public static class EventUtil
{
	public static Action AsNonRenderingEventHandler(Action callback)
	{
		return new SyncReceiver(callback).Invoke;
	}

	public static Action<TValue> AsNonRenderingEventHandler<TValue>(
			Action<TValue> callback)
	{
		return new SyncReceiver<TValue>(callback).Invoke;
	}

	public static Func<Task> AsNonRenderingEventHandler(Func<Task> callback)
	{
		return new AsyncReceiver(callback).Invoke;
	}

	public static Func<TValue, Task> AsNonRenderingEventHandler<TValue>(Func<TValue, Task> callback)
	{
		return new AsyncReceiver<TValue>(callback).Invoke;
	}

    private record SyncReceiver(Action Callback)
        : ReceiverBase
    {
        public void Invoke() => Callback();
    }

    private record SyncReceiver<T>(Action<T> Callback)
        : ReceiverBase
    {
        public void Invoke(T arg) => Callback(arg);
    }

    private record AsyncReceiver(Func<Task> Callback)
        : ReceiverBase
    {
        public Task Invoke() => Callback();
    }

    private record AsyncReceiver<T>(Func<T, Task> Callback)
        : ReceiverBase
    {
        public Task Invoke(T arg) => Callback(arg);
    }

    private record ReceiverBase : IHandleEvent
    {
		public Task HandleEventAsync(EventCallbackWorkItem item, object arg)
		{
			return item.InvokeAsync(arg);
		}
	}
}