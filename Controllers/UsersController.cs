using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;
public class UsersController : Controller
{
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }
	public UsersController(ScheduleDb db, UserProcessor userProcessor)
	{
		UserProcessor = userProcessor;
		RoleRequirement = new RoleRequirement(db);
	}


	/*[HttpPut($"[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] User user)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		var validationResult = Validator.Validate(user);
		if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
		var originalUser = await Crud.Get(user.GetPrimaryKey());
		var updatedUser = await Crud.Update(new User()
		{
			FirstName = user.FirstName,
			LastName = user.LastName,
			Note = user.Note,
			
			AccessTokens = originalUser.AccessTokens,
			Invites = originalUser.Invites,
			Auth0Id = originalUser.Auth0Id,
			Id = originalUser.Id,
			Tasks = originalUser.Tasks
		});

		return Ok(updatedUser);
	}*/

	/// <summary> Api endpoint for getting a user role in a schedule. </summary>
	/// <returns>
	/// Status 200 (OK) with user role of user in requested schedule.
	/// </returns>
	[HttpGet($"[controller]/[action]")]
	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Role([FromRoute] int? ScheduleId)
		{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.None);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		return result.IsAuthorized
			? Ok(result.AuthenticatedUser!.Role(ScheduleId))
			: Unauthorized(result.Errors);
	}
}