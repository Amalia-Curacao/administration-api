using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;

public class SchedulesController : Controller
{
	private ICrud<Schedule> Crud { get; }

	public SchedulesController(ScheduleDb db)
		=> Crud = new Crud<Schedule>(db);

	/// <summary> Api endpoint for getting all schedules in the database.</summary>
	/// <returns> Status 200 (OK) with all schedules in the database.</returns>
	[HttpGet("[controller]/[action]")]
	[Authorize("role:admin")]
	public async Task<ObjectResult> Get() 
		=> Ok(await Crud.Get());

	/// <summary> Api endpoint for creating schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the new schedule, when the schedule has been added.</returns>
	[HttpGet($"[controller]/[action]/{{{nameof(Schedule.Name)}}}")]
	[Authorize("role:admin")]
	public async Task<ObjectResult> Create([FromRoute] string Name)
		=> Ok((await Crud.Add(true, new Schedule() { Name = Name })).Single());
	
	/// <summary> Api endpoint for editing schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the edited schedule, when the schedule has been edited.</returns>
	[HttpPost($"[controller]/[action]")]
	[Authorize("role:owner/body")]
	[Authorize("role:admin")]
	public async Task<ObjectResult> Update([FromBody]Schedule schedule)
		=> Ok(await Crud.Update(schedule));

	/// <summary> Api endpoint for deleting schedules in the database. </summary>
	/// <returns> Status 200 (OK) with the deleted schedule, when the schedule has been deleted.</returns>
	/// <remarks> CLOSED for safety to open change private to public </remarks>
	[HttpDelete($"[controller]/[action]/{{{nameof(Schedule.Id)}}}")]
	[Authorize("role:admin")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
		=> Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Schedule.Id), Id) })));
}
