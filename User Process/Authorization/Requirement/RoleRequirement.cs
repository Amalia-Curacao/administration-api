using Creative.Api.Implementations.EntityFrameworkCore;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Security.Authorization.Roles;

internal class RoleRequirement : IAuthorizeRequirement<User>
{

	public string ErrorMessage => "User roles requirement is not met;";
	private IRead<ScheduleInviteLink> InviteLinkReader { get; }
	public UserRoles[] Roles
	{
		get => _roles ?? throw new NullReferenceException($"Role is null.");
	}
	private UserRoles[]? _roles = [];

	public int? ScheduleId { get; }

	public RoleRequirement(ScheduleDb db)
	{
		InviteLinkReader = new Crud<ScheduleInviteLink>(db, db.Set<ScheduleInviteLink>().Include(inv => inv.User!));
		ScheduleId = null;
		_roles = null;
	}
	public RoleRequirement(RoleRequirement requirement, params UserRoles[] roles)
	{
		InviteLinkReader = requirement.InviteLinkReader;
		_roles = roles;
		ScheduleId = null;
	}
	public RoleRequirement(RoleRequirement requirement, int? scheduleId, params UserRoles[] roles)
	{
		InviteLinkReader = requirement.InviteLinkReader;
		_roles = roles;
		ScheduleId = scheduleId;
	}
	public async Task<bool> Authorize(User? user)
	{
		if (user is null) return false;

		// Has no requirements
		if (Roles.Contains(UserRoles.None) || Roles.Length == 0) return true;

		// Get all invite links for the user
		var userInviteLinks = await InviteLinkReader.Get(l => l.User is not null && l.User.Auth0Id == user.Auth0Id);

		// Admins can access any schedule
		if (Roles.Contains(UserRoles.Admin) && userInviteLinks.Any(inv => inv.Role! == UserRoles.Admin && !inv.Disabled)) return true;

		// If the only role is admin, and the user is not an admin, return false
		if (Roles.Length == 1 && Roles[0] == UserRoles.Admin) return false;

		// If the schedule id is null, throw an exception.
		if (ScheduleId is null) throw new NullReferenceException($"{nameof(ScheduleId)} is null");

		// If the user has an invite link for the schedule with the required role, return true
		return userInviteLinks.Any(inv => inv.ScheduleId == ScheduleId && Roles.Any(r => r == inv.Role!) && !inv.Disabled);
	}


}
