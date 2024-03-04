using Creative.Api.Data;
using Creative.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduler.Api.Data.Models;

[PrimaryKey(nameof(Id))]
public sealed class Schedule : IModel
{
    public int? Id { get; set; }
	public string? Name { get; set; }

    [InverseProperty(nameof(Room.Schedule))]
    public ICollection<Room>? Rooms { get; set; } = new HashSet<Room>();

    [InverseProperty(nameof(ScheduleInviteLink.Schedule))]
    public ICollection<ScheduleInviteLink>? InviteLinks { get; set; } = new HashSet<ScheduleInviteLink>();

    [InverseProperty(nameof(Reservation.Schedule))]
    public ICollection<Reservation>? Reservations { get; set; } = new HashSet<Reservation>();

    [InverseProperty(nameof(Guest.Schedule))]
    public ICollection<Guest>? Guests { get; set; } = new HashSet<Guest>();

    [InverseProperty(nameof(HousekeepingTask.Schedule))]
    public ICollection<HousekeepingTask>? HousekeepingTasks { get; set; } = new HashSet<HousekeepingTask>();

    public void AutoIncrementPrimaryKey() 
        => Id = null;

	public void SetPrimaryKey(HashSet<Key> keys)
        => Id = keys.Single(key => key.Name == nameof(Id)).Value as int?;

	public HashSet<Key> GetPrimaryKey()
		=> new() { new Key(nameof(Id), Id) };
	
}