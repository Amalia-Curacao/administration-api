using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Controllers.Services;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.User_Process.Authorization.Requirement;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;

public class ScheduleInviteLinksController : ControllerBase
{
	private IRead<Schedule> ScheduleReader { get; }
	private IRead<User> UserReader { get; }
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }
	private HasAuthorityOverRequirement HasAuthorityOverRequirement { get; }
	private ScheduleInviteLinkService Service { get; }
	public ScheduleInviteLinksController(ScheduleDb db, UserProcessor userProcessor, ScheduleInviteLinkService service)
	{
		Service = service;
		ScheduleReader = new Crud<Schedule>(db);
		UserReader = new Crud<User>(db);
		UserProcessor = userProcessor;
		RoleRequirement = new RoleRequirement(db);
		HasAuthorityOverRequirement = new HasAuthorityOverRequirement(db);
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.Code)}}}")]
	public async Task<ObjectResult> Redeem([FromRoute]string Code)
	{

		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken);
		if(!result.IsAuthenticated) return Unauthorized(result.Errors);

		var role = await Service.TryRedeem(result.AuthenticatedUser!, Code);
		return role is null ? BadRequest("Invite link is invalid.") : Ok(role);
	}
	
	[HttpGet($"[controller]/[action]/{{scheduleId}}")]
	[HttpGet($"[controller]/[action]/{{scheduleId}}/{{userId}}/{{userRoles}}")]
	public async Task<ObjectResult> Revoke([FromRoute] int scheduleId, [FromRoute] int? userId, [FromRoute] string? userRoles)
	{
		
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var requestUser = (await UserProcessor.Process(accessToken)).AuthenticatedUser;
		if (requestUser is null) return Unauthorized("User is not authenticated.");

		var user = userId is not null ? await UserReader.Get([new(nameof(Data.Models.User.Id), userId)]) : null;
		var result = user is not null
			? await UserProcessor.Process(accessToken, new HasAuthorityOverRequirement(HasAuthorityOverRequirement, user, scheduleId))
			: await UserProcessor.Process(accessToken, new RoleRequirement(RoleRequirement, scheduleId, UserRoles.None));
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		return Ok(Service.Revoke(scheduleId, (user ?? requestUser).Id!.Value, userRoles is not null ? Enum.Parse<UserRoles>(userRoles) : null));
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Housekeeper([FromRoute]int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		var schedule = await ScheduleReader.Get([new(nameof(Schedule.Id), ScheduleId)]);
		return Ok(await Service.InviteLinkFor(UserRoles.Housekeeper, schedule));
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Manager([FromRoute]int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.Admin, UserRoles.Owner);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		var schedule = await ScheduleReader.Get([new(nameof(Schedule.Id), ScheduleId)]);
		return Ok(await Service.InviteLinkFor(UserRoles.Manager, schedule));
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Owner([FromRoute]int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.Admin);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		var schedule = await ScheduleReader.Get([new(nameof(Schedule.Id), ScheduleId)]);
		return Ok(await Service.InviteLinkFor(UserRoles.Owner, schedule));
	}

	[HttpGet($"[controller]/[action]")]
	public async Task<ObjectResult> Admin()
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.None);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		return Ok(await Service.InviteLinkFor(UserRoles.Admin));
	}
}
