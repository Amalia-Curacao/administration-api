using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;

public class HousekeepersController : Controller
{
	private ICrud<ScheduleInviteLink> Crud { get; }
	private IRead<User> UserReader { get; }
	private IRead<Room> RoomReader { get; }
	private RoleRequirement RoleRequirement { get; }
	private UserProcessor UserProcessor { get; }

	public HousekeepersController(ScheduleDb db, UserProcessor userProcessor)
	{
		Crud = new Crud<ScheduleInviteLink>(db, db.Set<ScheduleInviteLink>().Include(i => i.Schedule!).Include(i => i.User!));
		UserReader = new Crud<User>(db, db.Users.Include(u => u.Invites!));
		RoomReader = new Crud<Room>(db, db.Rooms.Include(r => r.Reservations!).Include(r => r.HousekeepingTasks!));
		RoleRequirement = new RoleRequirement(db);
		UserProcessor = userProcessor;
	}

	[HttpGet($"[controller]/[action]/{{scheduleId}}/{{userId}}")]
	[HttpGet($"[controller]/[action]/{{scheduleId}}/{{userId}}/{{note}}")]
	public async Task<ObjectResult> Note([FromRoute] int userId, [FromRoute] int scheduleId, [FromRoute] string? note)
	{
		var user = await UserReader.Get([new(nameof(Data.Models.User.Id), userId)]);
		if(user is null) return BadRequest("User not found.");

		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);

		var requirement = new RoleRequirement(RoleRequirement, scheduleId, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized && result.AuthenticatedUser!.Id != user.Id) return Unauthorized(result.Errors);

		var inviteLink = (await Crud.Get(i => i.UserId == user.Id && i.ScheduleId == scheduleId)).FirstOrDefault();
		if (inviteLink is null) return BadRequest("Invite link not found.");

		// TODO refactor
		inviteLink.Note = note ?? "";
		var updated = await Crud.Update(inviteLink);
		updated.User!.Note = updated.Note;
		return Ok(updated.User);
	}

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Get([FromRoute] int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		return result.IsAuthorized
			// TODO refactor
			? Ok((await Crud.Get(i => i.Role == UserRoles.Housekeeper && i.ScheduleId == ScheduleId)).Where(i => i.User is not null).Select(i => { i.User!.Note = i.Note; return i.User; }))
			: Unauthorized(result.Errors);
	}

	[HttpGet($"[controller]/[action]/{{scheduleId}}")]
	[HttpGet($"[controller]/[action]/{{scheduleId}}/{{userId}}")]
	public async Task<ObjectResult> Rooms([FromRoute] int scheduleId, [FromRoute] int? userId)
	{
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);

		var requirement = new RoleRequirement(RoleRequirement, scheduleId, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager, UserRoles.Housekeeper);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		var user = userId is null ? result.AuthenticatedUser : await UserReader.TryGet([new(nameof(Data.Models.User.Id), userId)]);
		if (user is null) return BadRequest("User not found.");

		var rooms = await RoomReader.Get(r => r.ScheduleId == scheduleId);
		return user.Role(scheduleId) != UserRoles.Housekeeper
			? Ok(rooms)
			: Ok(rooms.Select(r => { r.HousekeepingTasks = (r.HousekeepingTasks ?? []).Where(t => t.HousekeeperId == user.Id).ToArray(); return r; }));
	}
}
