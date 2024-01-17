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
    => options.DatabaseSrc switch
    {
        DatabaseSrc.Sqlite => InitializeAsSqlite((SqliteOptions)options),
        DatabaseSrc.SqlServer => InitializeAsSqlServer((SqlServerOptions)options),
        DatabaseSrc.PostgreSql => InitializeAsPostgreSql((PostgreSqlOptions)options),
        _ => throw new NotImplementedException("Database src is not supported.")
    };
    
    private static ScheduleDb InitializeAsPostgreSql(PostgreSqlOptions options)
    {
		options.DbOptions = PostgreSqlContextTool.InitDbContextOptions<ScheduleDb>(options);
		return new ScheduleDb(options);
	}   

    private static ScheduleDb InitializeAsSqlite(SqliteOptions options)
    {
        options.DbOptions = SqliteContextTool.InitDbContextOptions<ScheduleDb>(options);
        return new ScheduleDb(options);
    }

    private static ScheduleDb InitializeAsSqlServer(SqlServerOptions options)
    {
        options.DbOptions = SqlServerContextTool.InitDbContextOptions<ScheduleDb>(options);
        return new ScheduleDb(options);
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
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Housekeeper> Housekeepers { get; set; }
    public DbSet<HousekeepingTask> HousekeepingTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
