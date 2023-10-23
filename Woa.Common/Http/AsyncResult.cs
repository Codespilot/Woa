using System.Diagnostics.Contracts;

namespace System.Net.Http.Internal;

internal abstract class AsyncResult : IAsyncResult
{
	private readonly AsyncCallback _callback;
    private readonly object _state;

    private bool _isCompleted;
    private bool _completedSynchronously;
    private bool _endCalled;

    private Exception _exception;

    protected AsyncResult(AsyncCallback callback, object state)
    {
        _callback = callback;
        _state = state;
    }

    public object AsyncState
    {
        get { return _state; }
    }

    public WaitHandle AsyncWaitHandle
    {
        get
        {
            Contract.Assert(false, "AsyncWaitHandle is not supported -- use callbacks instead.");
            return null;
        }
    }

	public bool CompletedSynchronously => _completedSynchronously;

	public bool HasCallback => _callback != null;

	public bool IsCompleted => _isCompleted;

	protected void Complete(bool completedSynchronously)
    {
        if (_isCompleted)
        {
            throw new InvalidOperationException(string.Format("The IAsyncResult implementation '{0}' tried to complete a single operation multiple times. This could be caused by an incorrect application IAsyncResult implementation or other extensibility code, such as an IAsyncResult that returns incorrect CompletedSynchronously values or invokes the AsyncCallback multiple times.", GetType().Name));
        }

        _completedSynchronously = completedSynchronously;
        _isCompleted = true;

        if (_callback != null)
        {
            try
            {
                _callback(this);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Async Callback threw an exception.");
            }
        }
    }

    protected void Complete(bool completedSynchronously, Exception exception)
    {
        _exception = exception;
        Complete(completedSynchronously);
    }

    protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) 
		where TAsyncResult : AsyncResult
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

		if (result is not TAsyncResult thisPtr)
		{
			throw new ArgumentException(string.Format("An incorrect IAsyncResult was provided to an 'End' method. The IAsyncResult object passed to 'End' must be the one returned from the matching 'Begin' or passed to the callback provided to 'Begin'."));
		}

		if (!thisPtr.IsCompleted)
        {
            thisPtr.AsyncWaitHandle.WaitOne();
        }

        if (thisPtr._endCalled)
        {
            throw new InvalidOperationException("End cannot be called twice on an AsyncResult.");
        }

        thisPtr._endCalled = true;

        if (thisPtr._exception != null)
        {
            throw thisPtr._exception;
        }

        return thisPtr;
    }
}