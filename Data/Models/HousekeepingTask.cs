using Creative.Api.Data;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Api.Data.Models;

[PrimaryKey(nameof(Date), nameof(RoomNumber), nameof(RoomScheduleId))]
[Display(Name = "Housekeeping task")]
public class HousekeepingTask : IModel
{
	public DateOnly? Date { get; set; }
	[EnumDataType(typeof(HousekeepingTaskType))]
	public HousekeepingTaskType? Type { get; set; } = HousekeepingTaskType.None;
	
	[ForeignKey(nameof(Room.Number))]
	public int? RoomNumber { get; set; }

	[ForeignKey(nameof(Room.ScheduleId))]
	public int? RoomScheduleId { get; set; }

	public Room? Room { get; set; }

	[ForeignKey(nameof(Housekeeper.Id))]
	public int? HousekeeperId { get; set; }

	public User? Housekeeper { get; set; }

	[ForeignKey(nameof(Models.Schedule.Id))]
	public int? ScheduleId { get; set; }
	public Schedule? Schedule { get; set; }

	public void AutoIncrementPrimaryKey() { }

	public HashSet<Key> GetPrimaryKey()
		=> new()
		{
			new Key(nameof(Date), Date!),
			new Key(nameof(RoomNumber), RoomNumber!),
			new Key(nameof(RoomScheduleId), RoomScheduleId!)
		};

	public void SetPrimaryKey(HashSet<Key> keys)
	{
		var date = keys.Single(key => key.Name == nameof(Date)).Value;
		var roomNumber = keys.Single(key => key.Name == nameof(RoomNumber)).Value;
		var roomScheduleId = keys.Single(key => key.Name == nameof(RoomScheduleId)).Value;

		if (date is DateOnly d &&
			roomNumber is int rn &&
			roomScheduleId is int rsi)
		{
			Date = d;
			RoomNumber = rn;
			RoomScheduleId = rsi;
		}

		throw new Exception("Invalid key type.");
	}
}
