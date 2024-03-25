using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;
public class UsersController : Controller
{
	private ICrud<User> Crud { get; }
	private IValidator<User> Validator { get; }
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }
	public UsersController(ScheduleDb db, IValidator<User> validator, UserProcessor userProcessor)
	{
		Crud = new Crud<User>(db, db.Users
			.Include(u => u.Invites!)
				.ThenInclude(i => i.Schedule!));
		Validator = validator;
		UserProcessor = userProcessor;
		RoleRequirement = new RoleRequirement(db);
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Housekeepers([FromRoute] int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		return result.IsAuthorized
			? Ok(await Crud.Get(u => u.Role(ScheduleId) == UserRoles.Housekeeper))
			: Unauthorized(result.Errors);
	}

	[HttpPut($"[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] User user)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		var validationResult = Validator.Validate(user);
		if (!validationResult.IsValid) return BadRequest(validationResult.Errors);
		var updatedUser = await Crud.Update(user);

		return Ok(updatedUser);
	}

	/// <summary> Api endpoint for getting a user role in a schedule. </summary>
	/// <returns>
	/// Status 200 (OK) with user role of user in requested schedule.
	/// </returns>
	[HttpGet($"[controller]/[action]")]
	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Role([FromRoute] int? ScheduleId)
		{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.None);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		return result.IsAuthorized
			? Ok(result.AuthenticatedUser!.Role(ScheduleId))
			: Unauthorized(result.Errors);
	}
}