using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;
using Scheduler.Api.Security.Authorization;

namespace Scheduler.Api.User_Process.Authorization.Requirement;

public class HasAuthorityOverRequirement : IAuthorizeRequirement<User>
{
	public string ErrorMessage => "User does not have authority over the user.";
	private IRead<ScheduleInviteLink> InviteLinkReader { get; }
	private int ScheduleId { get => _scheduleId ?? throw new NullReferenceException("SchduleId is null; Pass the \"Schedule Id\" in the constructor."); }
	private int? _scheduleId { get; init; }
	private User RequestUser { get => _requestUser ?? throw new NullReferenceException("RequestUser is null; Pass the \"Request User\" in the constructor."); }
	private User? _requestUser { get; init; }

	public HasAuthorityOverRequirement(ScheduleDb db)
	{
		InviteLinkReader = new Crud<ScheduleInviteLink>(db, db.Set<ScheduleInviteLink>().Include(inv => inv.User!));
		_scheduleId = null;
		_requestUser = null;
	}
	public HasAuthorityOverRequirement(HasAuthorityOverRequirement requirement, User requestUser, int? scheduleId = null)
	{
		InviteLinkReader = requirement.InviteLinkReader;
		_scheduleId = scheduleId;
		_requestUser = requestUser;
	}

	public async Task<bool> Authorize(User? user)
	{
		if (user is null) return false;

		var requestedUserRoles = await UserRoles(RequestUser);
		var userRoles = await UserRoles(user);
		return requestedUserRoles.Any(r => userRoles.Any(ur => r.HasAuthorityOver(ur)));
	}

	private async Task<IEnumerable<UserRoles>> UserRoles(User user)
	{
		var userRoles = await InviteLinkReader.Get(l => l.User is not null && l.User.Id == user.Id);
		return userRoles.Select(inv => inv.User!.Role(ScheduleId));
	}
}
