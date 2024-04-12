using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authenticator;

namespace Scheduler.Api.UserProcess.Authenticator.InHouse;

public class Authenticator : IAuthenticator
{
	public Authenticator(ScheduleDb db)
	{
		Crud = new Crud<AccessToken>(db, db.AccessTokens.Include(a => a.User!).ThenInclude(u => u.Invites!));
	}
	private ICrud<AccessToken> Crud;
	public async Task<User?> TryAuthenticate(string accessToken)
	{
		var token = (await Crud.Get(a => a.Token == accessToken)).FirstOrDefault();
		if (token is null) return null;
		if (token.IsValid()) return token.User;
		await Crud.Delete(token.GetPrimaryKey());
		return null;
	}
}
