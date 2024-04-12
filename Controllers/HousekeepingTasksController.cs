using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;
public class HousekeepingTasksController : Controller
{
	private ICrud<HousekeepingTask> Crud { get; }
	private IValidator<HousekeepingTask> Validator { get; }
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }
	public HousekeepingTasksController(ScheduleDb db, IValidator<HousekeepingTask> validator, UserProcessor userProcessor)
	{
		Crud = new Crud<HousekeepingTask>(db);
		RoleRequirement = new RoleRequirement(db);
		Validator = validator;
		UserProcessor = userProcessor;
	}

	/// <summary> Creates a new housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeeping tasks, when the housekeeping task has been created.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] HousekeepingTask task)
	{
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var requirement = new RoleRequirement(RoleRequirement, task.RoomScheduleId!.Value, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		var isValid = Validator.Validate(task);
		if (!isValid.IsValid) return BadRequest(isValid.Errors);

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
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var requirement = new RoleRequirement(RoleRequirement, task.RoomScheduleId!.Value, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		var validationResult = Validator.Validate(task);
		return validationResult.IsValid ? Ok(await Crud.Update(task)) : BadRequest(validationResult.Errors);
	}

	/// <summary> Deletes a housekeeping task in the database. </summary>
	/// <returns>
	/// Status 200 (OK) when the housekeeping task has been deleted.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Delete([FromBody] HousekeepingTask task)
	{
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var requirement = new RoleRequirement(RoleRequirement, task.RoomScheduleId!.Value, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		return Ok(await Crud.Delete(task.GetPrimaryKey()));
	}
}