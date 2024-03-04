using Creative.Api.Data;
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
	public string? Note { get; set; } = "";
	[InverseProperty(nameof(HousekeepingTask.Housekeeper))]
	public ICollection<HousekeepingTask>? Tasks { get; set; } = new List<HousekeepingTask>();
	[InverseProperty(nameof(ScheduleInviteLink.User))]
	public ICollection<ScheduleInviteLink>? Invites { get; set; } = new List<ScheduleInviteLink>();

	public UserRoles Role(int scheduleId)
	{
		var inviteLink = Invites!.SingleOrDefault(invite => invite.ScheduleId == scheduleId);
		if(inviteLink is null) return UserRoles.None;
		return inviteLink.Role!.Value;
	}

	public void AutoIncrementPrimaryKey()
		=> Id = null;

	public HashSet<Key> GetPrimaryKey()
		=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys)
		=> Id = keys.Single(key => key.Name == nameof(Id)).Value as int?;
}
