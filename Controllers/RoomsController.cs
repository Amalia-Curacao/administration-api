using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scheduler.Api.Controllers;

public sealed class RoomsController : Controller
{
	private ICrud<Room> Crud { get; }
	private static ValidationException RoomNotFound { get; } = new("The room could not be located.");
	private static JsonSerializerOptions SerializerOptions { get; } = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true,
		ReferenceHandler = ReferenceHandler.IgnoreCycles
	};

	public RoomsController(ScheduleDb db)
	{
		Crud = new Crud<Room>(db, db.Rooms
			.Include(r => r.Reservations!)
			.ThenInclude(r => r.Guests!)
			.Include(r => r.HousekeepingTasks!)
			.ThenInclude(h => h.Housekeeper!));
	}

	/// <summary> Gets all the rooms in the database with a specific schedule id. </summary>
	/// <returns> Status 200 (OK) with the <see cref="Room"/>s, when the <see cref="Room"/>s have been found. </returns>
	/// <remarks> Makes use of Eager loading. </remarks>
	[HttpGet($"[controller]/[action]/{{{nameof(Reservation.ScheduleId)}}}")]
	[Authorize("role:admin/body=scheduleId")]
	[Authorize("role:owner/body=scheduleId")]
	[Authorize("role:manager/body=scheduleId")]
	[Authorize("role:housekeeper/body=scheduleId")]
	public async Task<ObjectResult> Get([FromRoute] int ScheduleId)	
		=> Ok(JsonSerializer.Serialize(await Crud.Get(r => r.ScheduleId == ScheduleId), SerializerOptions));	

	/// <summary> Creates a <see cref="Room"/>. </summary>
	/// <returns> Status 200 (OK) with the new <see cref="Room"/>, when the <see cref="Room"/> has been added. </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	[HttpPost($"[controller]/[action]")]
	[Authorize("role:admin/body=scheduleId")]
	[Authorize("role:owner/body=scheduleId")]
	public async Task<ObjectResult> Create([FromBody] Room room)
		=> Ok((await Crud.Add(false, room))[0]);

	/// <summary> Updates a <see cref="Room"/>. </summary>
	/// <returns> Status 200 (OK) with the updated <see cref="Room"/>, when the <see cref="Room"/> has been updated. </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	[HttpPost($"[controller]/[action]")]
	[Authorize("role:admin/body=scheduleId")]
	[Authorize("role:owner/body=scheduleId")]
	public async Task<ObjectResult> Update([FromBody] Room room)
		=> Ok(await Crud.Update(room));

	/// <summary> Deletes a <see cref="Room"/>. </summary>
	/// <returns> Status 200 (OK) with the deleted <see cref="Room"/>, when the <see cref="Room"/> has been deleted. </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	[HttpDelete($"[controller]/[action]/{{{nameof(Room.Number)}}}/{{{nameof(Room.ScheduleId)}}}")]
	[Authorize("role:admin/body=scheduleId")]
	[Authorize("role:owner/body=scheduleId")]
	public async Task<ObjectResult> Delete([FromRoute] int Number, [FromRoute] int ScheduleId)
		=> Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Room.Number), Number), new(nameof(Room.ScheduleId), ScheduleId) })));
}