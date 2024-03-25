using Creative.Database;
using Creative.Database.Data;
using Microsoft.EntityFrameworkCore;
using Scheduler.Api.Data.Models;

namespace Scheduler.Api.Data;

public class ScheduleDb : DatabaseContext
{
	private ScheduleDb(DatabaseContextOptions options) : base(options)
	{

	}
	public ScheduleDb(DbContextOptions options) : base(options)
	{

	}

	public static ScheduleDb Create(DatabaseContextOptions options)
	{
		return options.DatabaseSrc switch
		{
			DatabaseSrc.Sqlite => InitializeAsSqlite((SqliteOptions)options),
			DatabaseSrc.SqlServer => InitializeAsSqlServer((SqlServerOptions)options),
			DatabaseSrc.PostgreSql => InitializeAsPostgreSql((PostgreSqlOptions)options),
			_ => throw new NotImplementedException("Database src is not supported.")
		};

		ScheduleDb InitializeAsPostgreSql(PostgreSqlOptions options)
		{
			options.DbOptions = PostgreSqlContextTool.InitDbContextOptions<ScheduleDb>(options);
			return new ScheduleDb(options);
		}

		ScheduleDb InitializeAsSqlite(SqliteOptions options)
		{
			options.DbOptions = SqliteContextTool.InitDbContextOptions<ScheduleDb>(options);
			return new ScheduleDb(options);
		}

		ScheduleDb InitializeAsSqlServer(SqlServerOptions options)
		{
			options.DbOptions = SqlServerContextTool.InitDbContextOptions<ScheduleDb>(options);
			return new ScheduleDb(options);
		}
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		switch (Options.DatabaseSrc)
		{
			case DatabaseSrc.Sqlite:
				SqliteContextTool.OnConfiguring(optionsBuilder, (SqliteOptions)Options);
				break;
			case DatabaseSrc.SqlServer:
				SqlServerContextTool.OnConfiguring(optionsBuilder, (SqlServerOptions)Options);
				break;
			case DatabaseSrc.PostgreSql:
				PostgreSqlContextTool.OnConfiguring(optionsBuilder, (PostgreSqlOptions)Options);
				break;
			default:
				throw new NotImplementedException("Database src is not supported.");
		}
	}

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);
		switch (Options.DatabaseSrc)
		{
			case DatabaseSrc.SqlServer:
				ConfigureConventions(configurationBuilder);
				break;
			default:
				break;
		}
	}

	public DbSet<Schedule> Schedules { get; set; }
	public DbSet<Room> Rooms { get; set; }
	public DbSet<Reservation> Reservations { get; set; }
	public DbSet<Guest> Guests { get; set; }
	public DbSet<ScheduleInviteLink> ScheduleInviteLinks { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<HousekeepingTask> HousekeepingTasks { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Schedule>()
			.HasMany(s => s.Rooms)
			.WithOne(r => r.Schedule)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Schedule>()
			.HasMany(s => s.InviteLinks)
			.WithOne(h => h.Schedule)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Reservation>()
			.HasMany(r => r.Guests)
			.WithOne(g => g.Reservation)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Room>()
			.HasMany(r => r.HousekeepingTasks)
			.WithOne(h => h.Room)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Room>()
			.HasMany(r => r.Reservations)
			.WithOne(r => r.Room)
			.OnDelete(DeleteBehavior.SetNull);

		modelBuilder.Entity<User>()
			.HasMany(h => h.Tasks)
			.WithOne(h => h.Housekeeper)
			.OnDelete(DeleteBehavior.SetNull);


	}
}
