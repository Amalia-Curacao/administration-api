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
	[ForeignKey(nameof(Schedule.Id))]
	public int? ScheduleId { get; set; }
	public Schedule? Schedule { get; set; }

	private static DateTime DefaultExperationTime => DateTime.Now.AddDays(7);

	public void TryToRedeem(User user, out bool redeemed)
	{
		redeemed = false;
		if (IsRedeemable())
		{
			redeemed = true;
			UserId = user.Id;
			User = user;
			RedeemedAt = DateTime.Now;
			Code = null;
		}

	}

	// is not disabled, has not been redeemed, and has not expired
	public bool IsRedeemable() => !Disabled && RedeemedAt is null && ExpiresAt > DateTime.Now;

	public void Disable()
	{
		if (Disabled) return;
		else
		{
			Disabled = true;
		}
	}

	public void Enable()
	{
		if (!Disabled) return;
		else
		{
			Disabled = false;
		}
	}

	public static ScheduleInviteLink Generate(Schedule? schedule, UserRoles role, User? user = null)
	{
		if (schedule is null || role is UserRoles.Admin)
		{
			if (role is UserRoles.Admin)
			{
				return GenerateAdminInvite();
			}
		}
		else
		{
			return GenerateInviteLink();
		}

		throw new InvalidOperationException("Invalid schedule invite link generation.");

		static ScheduleInviteLink GenerateAdminInvite() => new()
		{
			Role = UserRoles.Admin,
			Code = GenerateCode(),
			CreatedAt = DateTime.Now,
			ExpiresAt = DefaultExperationTime
		};

		ScheduleInviteLink GenerateInviteLink() => new()
		{
			Role = role,
			Code = GenerateCode(),
			CreatedAt = DateTime.Now,
			ExpiresAt = DefaultExperationTime,
			Schedule = schedule,
			ScheduleId = schedule.Id,
			User = user,
			UserId = user?.Id
		};

		static string GenerateCode() => new string(Guid.NewGuid().ToString().Take(8).ToArray())!;
	}

	public void AutoIncrementPrimaryKey()
		=> Id = null;

	public HashSet<Key> GetPrimaryKey()
		=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys)
		=> Id = keys.Single(key => key.Name == nameof(Id)).Value as int?;
}
