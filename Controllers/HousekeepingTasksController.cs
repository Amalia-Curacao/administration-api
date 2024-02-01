using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;

public class HousekeepingTasksController : Controller
{
	private ICrud<HousekeepingTask> _crud { get; init; }
	private IValidator<HousekeepingTask> _validator { get; init; }
	public HousekeepingTasksController(ScheduleDb db, IValidator<HousekeepingTask> validator)
	{
		_crud = new Crud<HousekeepingTask>(db);
		_validator = validator;
	}
	/// <summary> Creates a new housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeeping tasks, when the housekeeping task has been created.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] HousekeepingTask task)
	{
		var isValid = _validator.Validate(task);
		if (!isValid.IsValid)
			return BadRequest(isValid.Errors);
		return Ok(await _crud.Add(false, task));
	}

	/// <summary> Updates a housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeeping tasks, when the housekeeping task has been updated.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] HousekeepingTask task)
	{
		var isValid = _validator.Validate(task);
		if (!isValid.IsValid)
			return BadRequest(isValid.Errors);
		return Ok(await _crud.Update(task));
	}

	/// <summary> Deletes a housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) when the housekeeping task has been deleted.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Delete([FromBody] HousekeepingTask task)
		=> Ok(await _crud.Delete(task.GetPrimaryKey()));

}
