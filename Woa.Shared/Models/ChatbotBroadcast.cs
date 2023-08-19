using Supabase.Realtime.Models;

namespace Woa.Shared;

public class ChatbotBroadcast : BaseBroadcast
{
	public string OpenId { get; set; }

	public long MessageId { get; set; }

	public string MessageContent { get; set; }

	public override string ToString()
	{
		return $"{OpenId} ({MessageId}){MessageContent}";
	}
}