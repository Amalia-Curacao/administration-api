using Creative.Api.Data;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Api.Data.Models;

[PrimaryKey(nameof(Id))]
public class ScheduleInviteLink : IModel
{
	[Required]
	public int? Id { get; set; }
	[Required]
	public UserRoles? Role { get; set; }
	[Required]
	public string? Code { get; set; }
	[Required]
	public DateTime? CreatedAt { get; set; }
	[Required]
	public DateTime? ExpiresAt { get; set; }
	public DateTime? RedeemedAt { get; set; }
	public bool Disabled { get; set; } = false;
	[ForeignKey(nameof(User.Id))]
	public int? UserId { get; set; }
	public User? User { get; set; }
	[Required]
	[ForeignKey(nameof(Schedule.Id))]
	public int? ScheduleId { get; set; }
	public Schedule? Schedule { get; set; }

	public void Redeem(User user)
	{
		if(Redeemed()) throw new InvalidOperationException("Invite link has already been redeemed");
		UserId = user.Id;
		User = user;
		RedeemedAt = DateTime.Now;
	}
	public bool Redeemed() => RedeemedAt is not null;

	public void AutoIncrementPrimaryKey() 
		=> Id = null;

	public HashSet<Key> GetPrimaryKey() 
		=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys) 
		=> Id = keys.Single(key => key.Name == nameof(Id)).Value as int?;
}
