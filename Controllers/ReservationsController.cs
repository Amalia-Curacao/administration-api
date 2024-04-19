using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;

namespace Scheduler.Api.Controllers;

public class ReservationsController : Controller
{
	private ICrud<Reservation> Crud { get; }
	private IValidator<Reservation> Validator { get; }
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }

	private static ValidationFailure RoomFull { get; } =
		new(nameof(Reservation.RoomNumber), "Reservation could not be added, because it was overlapping with an existing reservation.");
	private static ValidationFailure ReservationNotFound { get; } =
		new($"{nameof(Reservation.RoomNumber)}", "Reservation could not be located");

	public ReservationsController(ScheduleDb db, IValidator<Reservation> validator, UserProcessor userProcessor)
	{
		Crud = new Crud<Reservation>(db);
		RoleRequirement = new RoleRequirement(db);
		Validator = validator;
		UserProcessor = userProcessor;
	}

	/// <summary> Api endpoint for creating reservations in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the new reservation, when the reservation has been added.
	/// Status 400 (Bad request) with error message, when the reservation overlaps another, or properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] Reservation reservation)
	{
		if (reservation is null) return BadRequest("Reservation cannot be null.");

		// Validates properties
		var validationResult = Validator.Validate(reservation);
		if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);

		var requirement = new RoleRequirement(RoleRequirement, reservation.ScheduleId!.Value, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);
		// Checks if the reservation can fit in the room.
		if (!await CanFit(reservation)) validationResult.Errors.Add(RoomFull);

		return validationResult.IsValid
			? Ok((await Crud.Add(true, reservation))[^1])
			: BadRequest(validationResult.Errors);
	}

	/// <summary> Api endpoint for editing reservations in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the edited reservation, when the reservation has been successfully edited.
	/// Status 400 (Bad request) with error message, when the reservation overlaps another, or properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] Reservation reservation)
	{
		// Validates properties
		var validationResult = Validator.Validate(reservation);
		if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var requirement = new RoleRequirement(RoleRequirement, reservation.ScheduleId!.Value, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		// Checks if the reservation exists
		if (await Crud.TryGet(reservation.GetPrimaryKey()) is null) validationResult.Errors.Add(ReservationNotFound);
		// Checks if the reservation can fit in the room.
		if (!await CanFit(reservation)) validationResult.Errors.Add(RoomFull);

		return validationResult.IsValid
			? Ok(await Crud.Update(reservation))
			: BadRequest(validationResult.Errors);
	}

	/// <summary> Api endpoint for deleting reservations in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the deleted reservation, when the reservation has been deleted.
	/// Status 400 (Bad request) with error message, when the reservation does not exist.
	/// </returns>
	[HttpDelete($"[controller]/[action]/{{{nameof(Reservation.Id)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
	{
		var accessToken = HttpContext.AccessToken();
		if (accessToken is null) return BadRequest(HttpContextExtensions.MissingAccessTokenException);
		var reservation = await Crud.Get(new HashSet<Key>([new(nameof(Reservation.Id), Id)]));
		var requirement = new RoleRequirement(RoleRequirement, reservation.ScheduleId!.Value, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager);
		var result = await UserProcessor.Process(accessToken, requirement);
		if (!result.IsAuthorized) return Unauthorized(result.Errors);

		return Ok(await Crud.Delete(new HashSet<Key>([new(nameof(Reservation.Id), Id)])));
	}

	// Checks if the reservation can fit in the room from the same schedule and ignoers reservation with the same Id.
	private async Task<bool> CanFit(Reservation reservation)
		=> !(await Crud.Get())
			.Where(r =>
				r.ScheduleId == reservation.ScheduleId &&
				r.RoomNumber == reservation.RoomNumber &&
				r.Id != reservation.Id)
			.Any(r => r.Overlap(reservation));
}