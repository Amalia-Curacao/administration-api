using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;

public class HousekeepersController : Controller
{
	public ICrud<Housekeeper> _crud { get; init; }
	public IValidator<Housekeeper> _validator { get; init; }
	public HousekeepersController(ScheduleDb db, IValidator<Housekeeper> validator)
	{
		_crud = new Crud<Housekeeper>(db);
		_validator = validator;
	}

	/// <summary> Api endpoint for getting all housekeepers from a schedule. </summary>
	/// <returns>
	/// Status 200 (OK) with all housekeepers from a schedule in the database.
	/// </returns>
	[HttpGet($"[controller]/[action]/{{{nameof(Housekeeper.ScheduleId)}}}")]
	public async Task<ObjectResult> GetAll([FromRoute] int ScheduleId)
		=> Ok((await _crud.GetAll()).Where(housekeeper => housekeeper.ScheduleId == ScheduleId));

	/// <summary> Api endpoint for getting a specific housekeeper in the database.</summary>
	/// <returns>
	/// Status 200 (OK) with the housekeeper with the given id.
	/// Status 400 (Bad request) with error message, when the housekeeper could not be found.
	/// </returns>
	[HttpGet($"[controller]/[action]/{{{nameof(Housekeeper.Id)}}}")]
	public async Task<ObjectResult> Get([FromRoute] int Id)
	{
		var housekeeper = await _crud.TryGet(new HashSet<Key>(new Key[] { new(nameof(Housekeeper.Id), Id) }));
		return housekeeper is null ? BadRequest(new ValidationException($"{nameof(Housekeeper)} could not be located.")) : Ok(housekeeper);
	}

	/// <summary> Api endpoint for creating housekeepers in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeepers, when the housekeepers has been created.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] Housekeeper housekeeper)
	{
		var result = _validator.Validate(housekeeper);
		if (!result.IsValid)
			return BadRequest(result.Errors);

		return Ok(await _crud.Add(true, housekeeper));
	}

	/// <summary> Api endpoint for updating housekeepers in the database. </summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeepers, when the housekeepers has been updated.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Edit([FromBody] Housekeeper housekeeper)
	{
		var result = _validator.Validate(housekeeper);
		if (!result.IsValid)
			return BadRequest(result.Errors);

		return Ok(await _crud.Update(housekeeper));
	}

	/// <summary> Api endpoint for deleting a housekeeper in the database.</summary>
	/// <returns>
	/// Status 200 (OK) with the new set of housekeepers, when the housekeeper has been deleted.
	/// </returns>
	[HttpDelete("[controller]/[action]/{Id}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
		=> Ok(await _crud.Delete(new HashSet<Key>() { new Key(nameof(Housekeeper.Id), Id)}));
	
}
