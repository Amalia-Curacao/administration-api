using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;

public class GuestsController : Controller
{
	private ICrud<Guest> Crud { get; }
	private IRead<Reservation> ReservationReader { get; }
	private IValidator<Guest> Validator { get; }
	private RoleRequirement RoleRequirement { get; }
	private UserProcessor UserProcessor { get; }

	public GuestsController(ScheduleDb db, IValidator<Guest> validator, UserProcessor userProcessor)
	{
		Crud = new Crud<Guest>(db);
		ReservationReader = new Crud<Reservation>(db);
		RoleRequirement = new RoleRequirement(db);
		Validator = validator;
		UserProcessor = userProcessor;
	}

	private async Task<int> GetScheduleId(Guest guest)
		=> (int)(await ReservationReader.Get(new HashSet<Key>() { new(nameof(Reservation.Id), guest.ReservationId) })).ScheduleId!;

	/// <summary> Api endpoint for creating guests in the database. </summary>
	/// <returns> Status 200 (OK) with the new guest, when the guest has been added. </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] Guest guest)
	{
		var requirement = new RoleRequirement(RoleRequirement, await GetScheduleId(guest), UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (result.IsAuthorized) return Unauthorized(result.Errors);

		var validationResult = Validator.Validate(guest);
		return validationResult.IsValid ? Ok((await Crud.Add(true, guest))[0])
			: BadRequest(result.Errors);
	}

	/// <summary> Api endpoint for editing guests in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the edited guest, when the guest has been edited.
	/// Status 400 (Bad request) with error message, when properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] Guest guest)
	{
		var requirement = new RoleRequirement(RoleRequirement, await GetScheduleId(guest), UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (result.IsAuthorized) return Unauthorized(result.Errors);

		var validationResult = Validator.Validate(guest);
		return validationResult.IsValid ? Ok(await Crud.Update(guest))
			: BadRequest(result.Errors);
	}

	/// <summary> Api endpoint for deleting guests in the database. </summary>
	/// <returns> Status 200 (OK) and a boolean true, when the guest has been deleted. </returns>
	[HttpDelete($"[controller]/[action]/{{{nameof(Guest.Id)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
	{
		var guest = await Crud.Get(new HashSet<Key>(new Key[] { new(nameof(Guest.Id), Id) }));
		var requirement = new RoleRequirement(RoleRequirement, await GetScheduleId(guest), UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		if (result.IsAuthorized) return Unauthorized(result.Errors);

		return Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Guest.Id), Id) })));
	}

}