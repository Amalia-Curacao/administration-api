using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authenticator.Auth0;

namespace Scheduler.Api.Controllers;

public class AccessTokenController : Controller
{
	private ICrud<AccessToken> Crud;
	private Auth0Authenticator Auth0Authenticator;

	public AccessTokenController(ScheduleDb db, Auth0Authenticator auth0Authenticator)
	{
		Crud = new Crud<AccessToken>(db);
		Auth0Authenticator = auth0Authenticator;
	}

	[HttpGet($"[controller]/[action]")]
	public async Task<IActionResult> Auth0()
	{
		var accessToken = HttpContext.Auth0AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var user = await Auth0Authenticator.TryAuthenticate(accessToken);
		if(user is null) return BadRequest("Access token is not valid.");
		var existingAccessToken = await GetExistingAccessToken(user.Id!.Value);
		return Ok((existingAccessToken ?? (await Crud.Add(true, new AccessToken(user.Id!.Value)))[0]).Token);
	}

	[HttpGet($"[controller]/[action]/{{AccessToken}}")]
	public async Task<IActionResult> Revoke([FromRoute] string AccessToken)
	{
		if (AccessToken is not null) 
		{
			var toDelete = await Crud.Get(a => a.Token == AccessToken);
			if(toDelete.Length > 0) await Crud.Delete(toDelete.Select(a => a.GetPrimaryKey()).ToArray());
			return Ok();
		}
		return BadRequest();
	}

	private async Task<AccessToken?> GetExistingAccessToken(int userId)
	{
		var existingToken = (await Crud.Get(a => a.UserId == userId)).FirstOrDefault();
		if(existingToken is null) return null;
		if(!existingToken.IsValid())
		{
			await Crud.Delete(existingToken.GetPrimaryKey());
			return null;
		}
		existingToken.ExtendExpiration();
		return await Crud.Update(existingToken);
	}
}
