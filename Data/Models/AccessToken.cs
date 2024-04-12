using Creative.Api.Data;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Api.Data.Models;

[PrimaryKey(nameof(Id))]
public class AccessToken : IModel
{
	private static readonly TimeSpan ExpirationTime = TimeSpan.FromHours(2);
	public AccessToken(int userId)
	{
		Token = Guid.NewGuid().ToString();
		var createAt = DateTime.Now;
		CreatedAt = createAt;
		ExpiresAt = createAt + ExpirationTime;
		UserId = userId;
	}
	public int? Id { get; set; }

	// Is distinct
	public string Token { get; set; } = "";

	public DateTime CreatedAt { get; set; }
	public DateTime ExpiresAt { get; set; }
	[ForeignKey(nameof(User.Id))]
	public int UserId { get; set; }
	public User? User { get; set; }

	public bool IsValid()
		=> DateTime.Now < ExpiresAt;

	public void ExtendExpiration()
		=> ExpiresAt = DateTime.Now + ExpirationTime;

	public void AutoIncrementPrimaryKey() 
		=> Id = null;

	public HashSet<Key> GetPrimaryKey()
		=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys)
		=> Id = keys.Single(k => k.Name == nameof(Id)).Value as int?;
}
