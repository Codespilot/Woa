using MediatR;

namespace Woa.Webapi.Domain;

public class SensitiveWordDeleteCommand : ICommand
{
	public SensitiveWordDeleteCommand(int sensitiveWordId)
	{
		SensitiveWordId = sensitiveWordId;
	}

	public int SensitiveWordId { get; }
}