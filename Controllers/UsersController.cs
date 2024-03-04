using Creative.Api.Data;
using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Controllers;
public class UsersController : Controller
{
	public ICrud<User> Crud { get; }
	public IValidator<User> Validator { get; }
	public UsersController(ScheduleDb db, IValidator<User> validator)
	{
		Crud = new Crud<User>(db, db.Users
			.Include(u => u.Invites!)
				.ThenInclude(i => i.Schedule!));
		Validator = validator;
	}

	[HttpGet($"[controller]/[action]/{{{nameof(Data.Models.User.Id)}}}")]
	[Authorize("role:admin")]
	public async Task<ObjectResult> Get([FromRoute] int Id)
		=> Ok(await Crud.Get(new HashSet<Key>([new Key(nameof(Data.Models.User.Id), Id)])));

	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}")]
	public async Task<ObjectResult> Housekeepers([FromRoute] int ScheduleId)
		=> Ok(await Crud.Get(u => u.Role(ScheduleId) == UserRoles.Housekeeper));

	[HttpPost($"[controller]/[action]")]
	public async Task<ObjectResult> Create([FromBody] User user)
	{
		var result = Validator.Validate(user);

		if(result.IsValid) return BadRequest(result.Errors);

		var createdUser = await Crud.Add(true, user);

		return Ok(createdUser);
	}

	[HttpPut($"[controller]/[action]")]
	public async Task<ObjectResult> Update([FromBody] User user)
	{
		var result = Validator.Validate(user);

		if(result.IsValid) return BadRequest(result.Errors);

		var updatedUser = await Crud.Update(user);

		return Ok(updatedUser);
	}

	[HttpDelete($"[controller]/[action]/{{{nameof(Data.Models.User.Id)}}}")]
	public async Task<ObjectResult> Delete([FromRoute] int Id)
		=> Ok(await Crud.Delete(new HashSet<Key>([new Key(nameof(Data.Models.User.Id), Id)])));

	/// <summary> Api endpoint for getting a user role in a schedule. </summary>
	/// <returns>
	/// Status 200 (OK) with user role of user in requested schedule.
	/// </returns>
	[HttpGet($"[controller]/[action]/{{{nameof(ScheduleInviteLink.ScheduleId)}}}/{{{nameof(Data.Models.User.Id)}}}")]
	public async Task<ObjectResult> Role([FromRoute] int ScheduleId, [FromRoute] int Id)
		=> Ok((await Crud.Get(new HashSet<Key>() { new(nameof(Data.Models.User.Id), Id) })).Role(ScheduleId));

}