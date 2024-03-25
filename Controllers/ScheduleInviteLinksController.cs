using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;

public class ScheduleInviteLinksController : ControllerBase
{
	private ICrud<ScheduleInviteLink> Crud { get; }
	private IRead<Schedule> ScheduleReader { get; }
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }
	public ScheduleInviteLinksController(ScheduleDb db, UserProcessor userProcessor)
	{
		Crud = new Crud<ScheduleInviteLink>(db);
		ScheduleReader = new Crud<Schedule>(db);
		UserProcessor = userProcessor;
		RoleRequirement = new RoleRequirement(db);
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.Code)}}}")]
	public async Task<ObjectResult> Redeem([FromRoute]string Code)
	{
		var result = await UserProcessor.Process(HttpContext.AccessToken());
		if(!result.IsAuthenticated) return Unauthorized(result.Errors);

		var inviteLink = (await Crud.Get(inviteLink => inviteLink.Code == Code))[0];
		inviteLink.TryToRedeem(result.AuthenticatedUser!, out var redeemed);
		if(!redeemed) return BadRequest("Invite link is invalid.");
		return Ok((await Crud.Update(inviteLink)).Role!);
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Housekeeper([FromRoute]int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		var schedule = await ScheduleReader.Get(new HashSet<Key>() { new(nameof(Schedule.Id), ScheduleId) });
		return Ok(await GenerateInviteLink(UserRoles.Housekeeper, schedule));
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Manager([FromRoute]int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin, UserRoles.Owner);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		var schedule = await ScheduleReader.Get(new HashSet<Key>() { new(nameof(Schedule.Id), ScheduleId) });
		return Ok(await GenerateInviteLink(UserRoles.Manager, schedule));
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Owner([FromRoute]int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		var schedule = await ScheduleReader.Get(new HashSet<Key>() { new(nameof(Schedule.Id), ScheduleId) });
		return Ok(await GenerateInviteLink(UserRoles.Owner, schedule));
	}

	[HttpGet($"[controller]/[action]")]
	public async Task<ObjectResult> Admin()
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.None);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (!result.IsAuthenticated) return Unauthorized(result.Errors);

		return Ok(await GenerateInviteLink(UserRoles.Admin));
	}

	private async Task<string> GenerateInviteLink(UserRoles userRole, Schedule? schedule = null, bool checkedIfExists = false)
	{
		if (!checkedIfExists)
		{
			var existing = schedule is null
				? await Crud.Get(i => i.Role == userRole)
				: await Crud.Get(i => i.Role == userRole && i.ScheduleId == schedule.Id);
			existing = existing.Where(e => e.IsRedeemable()).ToArray();
			if (existing.Length > 0) return existing[0].Code!;
		}

		var inviteLink = ScheduleInviteLink.Generate(schedule, userRole);
		var duplicate = await Crud.Get(i => i.Code == inviteLink.Code);
		if(duplicate.Length > 0)
		{
			return await GenerateInviteLink(userRole, schedule, true);
		}
		else
		{
			await Crud.Add(true, inviteLink);
			return inviteLink.Code!;
		}
	}
}
