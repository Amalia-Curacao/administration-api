using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Controllers.Services;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;

public class SchedulesController : Controller
{
	private ICrud<Schedule> Crud { get; }
	private RoleRequirement RoleRequirement { get; }
	private UserProcessor UserProcessor { get; }
	private ScheduleInviteLinkService ScheduleInviteLinkService { get; }

	public SchedulesController(ScheduleDb db, ScheduleInviteLinkService scheduleInviteLinkService, UserProcessor userProcessor)
	{
		Crud = new Crud<Schedule>(db, db.Set<Schedule>().Include(s => s.InviteLinks!).ThenInclude(i => i.User!));
		RoleRequirement = new RoleRequirement(db);
		UserProcessor = userProcessor;
		ScheduleInviteLinkService = scheduleInviteLinkService;
	}

	/// <summary> Api endpoint for getting all schedules in the database.</summary>
	/// <returns> Status 200 (OK) with all schedules in the database.</returns>
	[HttpGet("[controller]/[action]")]
	public async Task<ObjectResult> Get()
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.None);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthenticated) return Unauthorized("Failed to authenticate user");
		return result.AuthenticatedUser!.IsAdmin()
			? Ok(await MapSchedules(await Crud.Get(), true))
			: (ObjectResult)Ok(await MapSchedules(await Crud.Get(s => s.InviteLinks!.Any(i => i.UserId == result.AuthenticatedUser!.Id))));

		async Task<IEnumerable<Schedule>> MapSchedules(IEnumerable<Schedule> schedules, bool isAdmin = false)
		{
			var tasks = schedules.Select(async s =>
			{
				s.Role = isAdmin ? UserRoles.Admin : s.InviteLinks!.First(i => i.UserId == result.AuthenticatedUser!.Id).Role!.Value;
				s.Owners = s.InviteLinks!.Where(i => i.Role == UserRoles.Owner && !i.IsRedeemable()).Select(i => i.User!.FirstName + " " + i.User.LastName).ToArray();
				s.OwnerInviteCode = isAdmin ? await ScheduleInviteLinkService.InviteLinkFor(UserRoles.Owner, s) : null;
				return s;
			});
			return await Task.WhenAll(tasks);
		}
	}

	/// <summary> Api endpoint for creating schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the new schedule, when the schedule has been added.</returns>
	[HttpGet($"[controller]/[action]/{{{nameof(Schedule.Name)}}}")]
	public async Task<ObjectResult> Create([FromRoute] string Name)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);

		return result.IsAuthorized
			? Ok((await Crud.Add(true, new Schedule() { Name = Name }))[0])
			: Unauthorized(result.Errors);
	}

	/// <summary> Api endpoint for editing schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the edited schedule, when the schedule has been edited.</returns>
	[HttpPost($"[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] Schedule schedule)
	{
		var requirement = new RoleRequirement(RoleRequirement, schedule.Id!, UserRoles.Admin, UserRoles.Owner);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);

		return result.IsAuthorized
			? Ok(await Crud.Update(schedule))
			: Unauthorized(result.Errors);
	}

	/// <summary> Api endpoint for deleting schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the deleted schedule, when the schedule has been deleted.</returns>
	/// <remarks> CLOSED for safety to open change private to public </remarks>
	[HttpDelete($"[controller]/[action]/{{{nameof(Schedule.Id)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
	{
		var requirement = new RoleRequirement(RoleRequirement, Id, UserRoles.Admin);
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var result = await UserProcessor.Process(accessToken, requirement);

		return result.IsAuthorized
			? Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Schedule.Id), Id) })))
			: Unauthorized(result.Errors);
	}
}
