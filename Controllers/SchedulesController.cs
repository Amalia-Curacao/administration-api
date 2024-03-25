using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

	public SchedulesController(ScheduleDb db, UserProcessor userProcessor)
	{
		Crud = new Crud<Schedule>(db, db.Set<Schedule>().Include(s => s.InviteLinks!));
		RoleRequirement = new RoleRequirement(db);
		UserProcessor = userProcessor;
	}

	/// <summary> Api endpoint for getting all schedules in the database.</summary>
	/// <returns> Status 200 (OK) with all schedules in the database.</returns>
	[HttpGet("[controller]/[action]")]
	public async Task<ObjectResult> Get()
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);

		if (!result.IsAuthenticated) return Unauthorized("User is not authenticated");

		return result.IsAuthorized	
			? Ok(await Crud.Get())
			: Ok(await Crud.Get(s => s.InviteLinks!.Any(i => i.UserId == result.AuthenticatedUser!.Id)));
	}

	/// <summary> Api endpoint for creating schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the new schedule, when the schedule has been added.</returns>
	[HttpGet($"[controller]/[action]/{{{nameof(Schedule.Name)}}}")]
	public async Task<ObjectResult> Create([FromRoute] string Name)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);

		return result.IsAuthorized
			? Ok((await Crud.Add(true, new Schedule() { Name = Name }))[0])
			: Unauthorized(result.Errors);
	}

	/// <summary> Api endpoint for editing schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the edited schedule, when the schedule has been edited.</returns>
	[HttpPost($"[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] Schedule schedule)
	{
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin, UserRoles.Owner);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);

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
		var requirement = new RoleRequirement(RoleRequirement, UserRoles.Admin);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);

		return result.IsAuthorized
			? Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Schedule.Id), Id) })))
			: Unauthorized(result.Errors);
	}
}
