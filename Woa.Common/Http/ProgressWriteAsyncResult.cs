using System.Diagnostics.Contracts;
using System.Net.Http.Internal;

namespace System.Net.Http.Handlers;

internal class ProgressWriteAsyncResult : AsyncResult
{
    private static readonly AsyncCallback _writeCompletedCallback = WriteCompletedCallback;

    private readonly Stream _innerStream;
    private readonly ProgressStream _progressStream;
    private readonly int _count;

	public ProgressWriteAsyncResult(Stream innerStream, ProgressStream progressStream, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        : base(callback, state)
    {
        Contract.Assert(innerStream != null);
        Contract.Assert(progressStream != null);
        Contract.Assert(buffer != null);

        _innerStream = innerStream;
        _progressStream = progressStream;
        _count = count;

        try
        {
            IAsyncResult result = innerStream.BeginWrite(buffer, offset, count, _writeCompletedCallback, this);
            if (result.CompletedSynchronously)
            {
                WriteCompleted(result);
            }
        }
        catch (Exception e)
        {
            Complete(true, e);
        }
    }

	private static void WriteCompletedCallback(IAsyncResult result)
    {
        if (result.CompletedSynchronously)
        {
            return;
        }

        ProgressWriteAsyncResult thisPtr = (ProgressWriteAsyncResult)result.AsyncState;
        try
        {
            thisPtr.WriteCompleted(result);
        }
        catch (Exception e)
        {
            thisPtr.Complete(false, e);
        }
    }

    private void WriteCompleted(IAsyncResult result)
    {
        _innerStream.EndWrite(result);
        _progressStream.ReportBytesSent(_count, AsyncState);
        Complete(result.CompletedSynchronously);
    }

    public static void End(IAsyncResult result)
    {
		End<ProgressWriteAsyncResult>(result);
    }
}