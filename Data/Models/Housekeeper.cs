using Creative.Api.Data;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Api.Data.Models;

[PrimaryKey(nameof(Id))]
[Display(Name = "Housekeeper")]
public class Housekeeper : IModel
{
	public int? Id { get; set; }
	public string? FirstName { get; set; }
	public string? LastName { get; set; }
	public string? Note { get; set; } = "";
	[InverseProperty(nameof(HousekeepingTask.Housekeeper))]
	public IEnumerable<HousekeepingTask>? Tasks { get; set; }

	[ForeignKey(nameof(ScheduleId))]
	public int? ScheduleId { get; set; }
	public Schedule? Schedule { get; set; }

	public void AutoIncrementPrimaryKey()
		=> Id = null;

	public HashSet<Key> GetPrimaryKey()
		=> new() { new Key(nameof(Id), Id) };

	public void SetPrimaryKey(HashSet<Key> keys)
		=> Id = keys.Single(key => key.Name == nameof(Id)).Value as int?;
}
