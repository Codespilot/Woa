namespace Woa.Webapi.Dtos;

public class SensitiveWordItemDto
{
	public int Id { get; set; }

	public string Content { get; set; }

	public bool IsValid { get; set; }

	public DateTime CreateTime { get; set; }
}