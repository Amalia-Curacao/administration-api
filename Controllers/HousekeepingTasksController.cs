using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;

[Authorize("role:admin/body=scheduleId")]
[Authorize("role:owner/body=scheduleId")]
[Authorize("role:manager/body=scheduleId")]
public class HousekeepingTasksController : Controller
{
	private ICrud<HousekeepingTask> Crud { get; }
	private IValidator<HousekeepingTask> Validator { get; }
	public HousekeepingTasksController(ScheduleDb db, IValidator<HousekeepingTask> validator)
	{
		Crud = new Crud<HousekeepingTask>(db);
		Validator = validator;
	}
	/// <summary> Creates a new housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeeping tasks, when the housekeeping task has been created.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] HousekeepingTask task)
	{
		var isValid = Validator.Validate(task);
		if (!isValid.IsValid)
			return BadRequest(isValid.Errors);
		return Ok(await Crud.Add(false, task));
	}

	/// <summary> Updates a housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeeping tasks, when the housekeeping task has been updated.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] HousekeepingTask task)
	{
		var isValid = Validator.Validate(task);
		if (!isValid.IsValid)
			return BadRequest(isValid.Errors);
		return Ok(await Crud.Update(task));
	}

	/// <summary> Deletes a housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) when the housekeeping task has been deleted.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Delete([FromBody] HousekeepingTask task)
		=> Ok(await Crud.Delete(task.GetPrimaryKey()));

}