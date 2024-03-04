﻿using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;


[Authorize("role:admin/body=scheduleId")]
[Authorize("role:owner/body=scheduleId")]
[Authorize("role:manager/body=scheduleId")]
public class ReservationsController : Controller
{
	private ICrud<Reservation> Crud { get; }
	private IValidator<Reservation> Validator { get; }

	private static ValidationFailure RoomFull { get; } =
		new(nameof(Reservation.RoomNumber), "Reservation could not be added, because it was overlapping with an existing reservation.");
	private static ValidationFailure ReservationNotFound { get; } =
		new($"{nameof(Reservation.RoomNumber)}", "Reservation could not be located");

	public ReservationsController(ScheduleDb db, IValidator<Reservation> validator)
	{
		Crud = new Crud<Reservation>(db);
		Validator = validator;
	}

	/// <summary> Api endpoint for getting all reservations in the database.</summary>
	/// <returns> Status 200 (OK) with all reservations in the database.</returns>
	[HttpGet("[controller]/[action]")]
	public async Task<ObjectResult> Get()
		=> Ok(await Crud.Get());

	/// <summary> Api endpoint for getting all reservations from a room in the database.</summary>
	/// <returns>
	/// Status 200 (OK) with all reservations from a room in the database.
	/// </returns>
	/// <remarks> Returns empty list when room is not found/does not exist. </remarks>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	// [HttpGet($"[controller]/[action]/{{{nameof(Reservation.RoomScheduleId)}}}/{{{nameof(Reservation.RoomNumber)}}}")]
	private async Task<ObjectResult> Get([FromRoute] int RoomScheduleId, [FromRoute] int RoomNumber)
		=> Ok(await Crud.Get(r => r.RoomScheduleId == RoomScheduleId && r.RoomNumber == RoomNumber));


	/// <summary> Api endpoint for getting a reservation by id.</summary>
	/// <returns> 
	/// Status 200 (OK) with the reservation with the given id.
	/// Status 400 (Bad request) with error message, when the reservation could not be found.
	/// </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	// [HttpGet($"[controller]/[action]/{{{nameof(Reservation.Id)}}}")]
	private async Task<ObjectResult> Get([FromRoute] int Id)
	{
		var reservation = await Crud.Get(new HashSet<Key>(new Key[] { new(nameof(Reservation.Id), Id) }));
		return reservation is null ? BadRequest(ReservationNotFound) : Ok(reservation);
	}

	/// <summary> Api endpoint for creating reservations in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the new reservation, when the reservation has been added.
	/// Status 400 (Bad request) with error message, when the reservation overlaps another, or properties are invalid.
	/// </returns>
	[HttpPost("[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] Reservation reservation)
	{
		if(reservation is null) return BadRequest("Reservation cannot be null.");
		// Validates properties
		var result = Validator.Validate(reservation);
		// Checks if the reservation can fit in the room.
		if (!await CanFit(reservation)) result.Errors.Add(RoomFull);

		return result.IsValid
			? Ok((await Crud.Add(true, reservation))[^1])
			: BadRequest(result.Errors);
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
		var result = Validator.Validate(reservation);
		// Checks if the reservation exists
		if (await Crud.TryGet(reservation.GetPrimaryKey()) is null) result.Errors.Add(ReservationNotFound);
		// Checks if the reservation can fit in the room.
		if (!await CanFit(reservation)) result.Errors.Add(RoomFull);

		return result.IsValid
			? Ok(await Crud.Update(reservation))
			: BadRequest(result.Errors);
	}

	/// <summary> Api endpoint for deleting reservations in the database. </summary>
	/// <returns> 
	/// Status 200 (OK) with the deleted reservation, when the reservation has been deleted.
	/// Status 400 (Bad request) with error message, when the reservation does not exist.
	/// </returns>
	[HttpDelete($"[controller]/[action]/{{{nameof(Reservation.Id)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
		=> Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Reservation.Id), Id) })));

	// Checks if the reservation can fit in the room from the same schedule and ignoers reservation with the same Id.
	private async Task<bool> CanFit([FromBody] Reservation reservation)
		=> !(await Crud.Get())
			.Where(r => 
				r.ScheduleId == reservation.ScheduleId && 
				r.RoomNumber == reservation.RoomNumber && 
				r.Id != reservation.Id)
			.Any(r => r.Overlap(reservation));
}