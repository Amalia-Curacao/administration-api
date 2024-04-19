using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using System.Data;

namespace Scheduler.Api.Controllers.Services;

public class ScheduleInviteLinkService
{

	private ICrud<ScheduleInviteLink> Crud { get; }
	public ScheduleInviteLinkService(ScheduleDb db)
	{
		Crud = new Crud<ScheduleInviteLink>(db, db.ScheduleInviteLinks.Include(inv => inv.User!).Include(inv => inv.Schedule!));
	}

	public async Task<string> InviteLinkFor(UserRoles userRole, Schedule? schedule = null) 
		=> await GetExistingInviteLink(userRole, schedule) ?? await GenerateInviteLink(userRole, schedule);

	private async Task<string?> GetExistingInviteLink(UserRoles userRole, Schedule? schedule = null)
	{
		var existing = schedule is null
				? await Crud.Get(i => i.Role == userRole)
				: await Crud.Get(i => i.Role == userRole && i.ScheduleId == schedule.Id);
		existing = existing.Where(e => e.IsRedeemable()).ToArray();
		if (existing.Length > 0) return existing[0].Code!;
		return null;
	}

	private async Task<string> GenerateInviteLink(UserRoles userRole, Schedule? schedule = null)
	{
		var inviteLink = ScheduleInviteLink.Generate(schedule, userRole);
		var duplicate = await Crud.Get(i => i.Code == inviteLink.Code);
		if (duplicate.Length > 0)
		{
			return await GenerateInviteLink(userRole, schedule);
		}
		else
		{
			await Crud.Add(true, inviteLink);
			return inviteLink.Code!;
		}
	}

	public async Task<bool> Revoke(int scheduleId, int userId, UserRoles? role)
	{
		var inviteLink = (await Crud.Get(i => i.ScheduleId == scheduleId
			&& i.UserId == userId
			&& (role is null || i.Role == role))).FirstOrDefault();

		return inviteLink is null ? false : await Crud.Delete(inviteLink.GetPrimaryKey());
	}


	public async Task<UserRoles?> TryRedeem(User user, string code)
	{
		var inviteLink = (await Crud.Get(i => i.Code == code)).FirstOrDefault();
		if (inviteLink is null) return null;
		inviteLink.TryToRedeem(user, out var redeemed);
		if (!redeemed) return null;
		await Crud.Update(inviteLink);
		return inviteLink.Role;
	}
}
