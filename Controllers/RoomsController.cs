using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization.Roles;
using Scheduler.Api.UserProcess;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scheduler.Api.Controllers;

public sealed class RoomsController : Controller
{
	private ICrud<Room> Crud { get; }
	private UserProcessor UserProcessor { get; }
	private RoleRequirement RoleRequirement { get; }
	private static JsonSerializerOptions SerializerOptions { get; } = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true,
		ReferenceHandler = ReferenceHandler.IgnoreCycles
	};

	public RoomsController(ScheduleDb db, UserProcessor userProcessor)
	{
		Crud = new Crud<Room>(db, db.Rooms
			.Include(r => r.Reservations!)
			.ThenInclude(r => r.Guests!)
			.Include(r => r.HousekeepingTasks!)
			.ThenInclude(h => h.Housekeeper!));
		RoleRequirement = new RoleRequirement(db);
		UserProcessor = userProcessor;
	}

	/// <summary> Gets all the rooms in the database with a specific schedule id. </summary>
	/// <returns> Status 200 (OK) with the <see cref="Room"/>s, when the <see cref="Room"/>s have been found. </returns>
	/// <remarks> Makes use of Eager loading. </remarks>
	[HttpGet($"[controller]/[action]/{{{nameof(Reservation.ScheduleId)}}}")]
	public async Task<ObjectResult> Get([FromRoute] int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.Admin, UserRoles.Owner, UserRoles.Manager, UserRoles.Housekeeper);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		return result.IsAuthorized
			? Ok(JsonSerializer.Serialize(await Crud.Get(r => r.ScheduleId == ScheduleId), SerializerOptions))
			: Unauthorized(result.Errors);
	}

	/// <summary> Creates a <see cref="Room"/>. </summary>
	/// <returns> Status 200 (OK) with the new <see cref="Room"/>, when the <see cref="Room"/> has been added. </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	[HttpPost($"[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] Room room)
	{
		var requirement = new RoleRequirement(RoleRequirement, room.ScheduleId, UserRoles.Admin, UserRoles.Owner);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		return result.IsAuthorized
			? Ok((await Crud.Add(false, room))[0])
			: Unauthorized(result.Errors);
	}

	/// <summary> Updates a <see cref="Room"/>. </summary>
	/// <returns> Status 200 (OK) with the updated <see cref="Room"/>, when the <see cref="Room"/> has been updated. </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	[HttpPost($"[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] Room room)
	{
		var requirement = new RoleRequirement(RoleRequirement, room.ScheduleId, UserRoles.Admin, UserRoles.Owner);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		return result.IsAuthorized
			? Ok(await Crud.Update(room))
			: Unauthorized(result.Errors);
	}

	/// <summary> Deletes a <see cref="Room"/>. </summary>
	/// <returns> Status 200 (OK) with the deleted <see cref="Room"/>, when the <see cref="Room"/> has been deleted. </returns>
	/// <remarks> Closed for safetly, to open change private to public. </remarks>
	[HttpDelete($"[controller]/[action]/{{{nameof(Room.Number)}}}/{{{nameof(Room.ScheduleId)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Number, [FromRoute] int ScheduleId)
	{
		var requirement = new RoleRequirement(RoleRequirement, ScheduleId, UserRoles.Admin, UserRoles.Owner);
		var result = await UserProcessor.Process(HttpContext.AccessToken(), requirement);
		return result.IsAuthorized
			? Ok(await Crud.Delete(new HashSet<Key>(new Key[] { new(nameof(Room.Number), Number), new(nameof(Room.ScheduleId), ScheduleId) })))
			: Unauthorized(result.Errors);
	}
}