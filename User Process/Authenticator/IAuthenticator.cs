using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Security.Authenticator;

public interface IAuthenticator
{
	public Task<User?> TryAuthenticate(string accessToken);
}
