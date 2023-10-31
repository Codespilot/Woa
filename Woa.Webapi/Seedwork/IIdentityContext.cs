namespace Woa.Webapi;

public interface IIdentityContext
{
	int Id { get; }

	public string UserName { get; }

	bool IsAuthenticated { get; }

	IEnumerable<string> Roles { get; }

	bool IsInRole(string role);
}