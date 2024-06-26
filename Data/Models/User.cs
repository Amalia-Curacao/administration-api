﻿using Creative.Api.Data;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Api.Data.Models;

[PrimaryKey(nameof(Id))]
public class User : IModel
{
	public int? Id { get; set; }
	public string? Auth0Id { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	[NotMapped]
	public string Note { get; set; } = "";
	[InverseProperty(nameof(HousekeepingTask.Housekeeper))]
	public ICollection<HousekeepingTask>? Tasks { get; set; } = new List<HousekeepingTask>();
	[InverseProperty(nameof(ScheduleInviteLink.User))]
	public ICollection<ScheduleInviteLink>? Invites { get; set; } = new List<ScheduleInviteLink>();

	[InverseProperty(nameof(AccessToken.User))]
	public ICollection<AccessToken>? AccessTokens { get; set; }

	public bool IsAdmin() => Role(null) == UserRoles.Admin;
	public UserRoles Role(int? scheduleId)
	{
		if (Invites is null) throw new NullReferenceException("Invites is null; Add to eager loading.");
		var validInvites = Invites.Where(invite => !invite.Disabled);

		if (scheduleId is null)
		{
			return validInvites.Any(invite => invite.Role == UserRoles.Admin) 
				? UserRoles.Admin 
				: UserRoles.None;
		}

		var inviteLink = validInvites!.FirstOrDefault(invite => invite.ScheduleId == scheduleId);
		return inviteLink is null 
			? UserRoles.None 
			: inviteLink.Role!.Value;
	}

	public void AutoIncrementPrimaryKey()
		=> Id = null;

	public HashSet<Key> GetPrimaryKey()
		=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys)
		=> Id = keys.Single(key => key.Name == nameof(Id)).Value as int?;
}
