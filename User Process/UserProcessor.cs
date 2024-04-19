using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authenticator;
using Scheduler.Api.Security.Authorization;
using Scheduler.Api.UserProcess.Authorization;

namespace Scheduler.Api.UserProcess;

public class UserProcessor
{
	private IAuthenticator Authenticator { get; }
	private IAuthorizer Authorizer { get; }
	public UserProcessor(IAuthenticator authenticator, IAuthorizer authorizer)
	{
		Authenticator = authenticator;
		Authorizer = authorizer;
	}

	public async Task<UserProcessorResult> Process(string accessToken, params IAuthorizeRequirement<User>[] requirements)
	{
		List<string> errors = []; 
		var user = await Authenticator.TryAuthenticate(accessToken);

		if (user is not null) {
			Authorizer.TryAuthorize(user, requirements);
			errors.AddRange(Authorizer.Errors);
		};

		return new UserProcessorResult()
		{
			Errors = errors.ToArray(),
			AuthenticatedUser = user
		};
	}
}

public class UserProcessorResult
{
	public string[] Errors { get; init; } = [];
	public bool IsAuthorized => Errors.Length == 0 && IsAuthenticated;
	public User? AuthenticatedUser { get; init; }
	public bool IsAuthenticated => AuthenticatedUser is not null;
}
