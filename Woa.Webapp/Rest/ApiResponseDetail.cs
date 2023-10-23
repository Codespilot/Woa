namespace Woa.Webapp.Rest;

public class ApiResponseDetail
{
	public string Title { get; set; }

	public int StatusCode { get; set; }

	public string Message { get; set; }

	public Dictionary<string,string[]> Errors { get; set; }
}