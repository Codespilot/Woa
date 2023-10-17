namespace Woa.Common;

public class CancellationObject : IDisposable
{
	private readonly TimeSpan _timeOut;

	private CancellationTokenSource _cancellation;

	public CancellationObject(TimeSpan timeOut)
	{
		_timeOut = timeOut;
		_cancellation = new CancellationTokenSource(_timeOut);
	}

	public void Cancel()
	{
		if (_cancellation is { IsCancellationRequested: false })
		{
			_cancellation.Cancel();
		}

		_cancellation = new CancellationTokenSource(_timeOut);
	}

	public void Reset()
	{
		_cancellation.TryReset();
	}

	public CancellationToken Token => _cancellation.Token;

	public void Dispose()
	{
		_cancellation?.Dispose();
	}
}