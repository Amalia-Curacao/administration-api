using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;

public class GuestsController : Controller
{
	private ICrud<Guest> Crud { get; }
	private IValidator<Guest> Validator { get; }
	private readonly ValidationException PersonNotFound = new($"{nameof(Guest)} could not be located.");

	public GuestsController(ScheduleDb db, IValidator<Guest> validator)
	{
		Crud = new Crud<Guest>(db);
		Validator = validator;
	}

	/// <summary> Api endpoint for creating guests in the database. </summary>
	/// <returns> Status 200 (OK) with the new guest, when the guest has been added. </returns>
	[HttpPost("[controller]/[action]")]
	[Authorize("role:admin/body=scheduleId")]
	[Authorize("role:owner/body=scheduleId")]
	[Authorize("role:manager/body=scheduleId")]
	public async Task<ObjectResult> Create([FromBody] Guest guest)
	{
		var result = Validator.Validate(guest);
		return result.IsValid ? Ok((await Crud.Add(true, guest))[0])
			: BadRequest(result.Errors);
	}

	/// <summary> Api endpoint for editing guests in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the edited guest, when the guest has been edited.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	[Authorize("role:admin/body=scheduleId")]
	[Authorize("role:owner/body=scheduleId")]
	[Authorize("role:manager/body=scheduleId")]
	public async Task<ObjectResult> Update([FromBody] Guest guest)
	{
		var result = Validator.Validate(guest);
		return result.IsValid ? Ok(await Crud.Update(guest))
			: BadRequest(result.Errors);
	}

	/// <summary> Api endpoint for deleting guests in the database. </summary>
	/// <returns> Status 200 (OK) and a boolean true, when the guest has been deleted. </returns>
	[HttpDelete($"[controller]/[action]/{{{nameof(Guest.Id)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
		=> Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Guest.Id), Id) })));

}