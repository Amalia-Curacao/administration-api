using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Controllers.Internal.Auth0;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Registration;

namespace Scheduler.Api.Security.Authenticator.Auth0;

public class Auth0Authenticator : IAuthenticator
{
	private IRead<User> UserReader { get; }

	private Auth0Api Auth0Controller { get; }
	private IRegisterUserService? RegisterService { get; }


	internal Auth0Authenticator(ScheduleDb db, Auth0Api auth0Controller, IRegisterUserService? registerService)
	{
		UserReader = new Crud<User>(db, db.Set<User>().Include(u => u.Invites!).ThenInclude(u => u.Schedule!));
		Auth0Controller = auth0Controller;
		RegisterService = registerService;
	}

	public async Task<User?> TryAuthenticate(string accessToken)
	{
		try
		{
			var auth0UserInfo = await Auth0Controller.UserInfo(accessToken);

			var databaseUser = (await UserReader.Get(u => u.Auth0Id == auth0UserInfo.Sub)).FirstOrDefault();
			if(databaseUser is not null) return databaseUser;
			else if(RegisterService is null) return null;
			else return await RegisterService.Register(new()
				{
					Auth0Id = auth0UserInfo.Sub,
					FirstName = auth0UserInfo.Nickname,
				});
		}
		catch(InvalidOperationException)
		{
			return null;
		}
	}
}
