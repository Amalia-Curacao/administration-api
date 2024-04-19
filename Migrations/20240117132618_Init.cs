using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Scheduler.Api.Migrations
{
	/// <inheritdoc />
	public partial class Init : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Schedules",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					Name = table.Column<string>(type: "text", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Schedules", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Housekeepers",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					FirstName = table.Column<string>(type: "text", nullable: true),
					LastName = table.Column<string>(type: "text", nullable: true),
					Note = table.Column<string>(type: "text", nullable: true),
					ScheduleId = table.Column<int>(type: "integer", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Housekeepers", x => x.Id);
					table.ForeignKey(
						name: "FK_Housekeepers_Schedules_ScheduleId",
						column: x => x.ScheduleId,
						principalTable: "Schedules",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Rooms",
				columns: table => new
				{
					Number = table.Column<int>(type: "integer", nullable: false),
					ScheduleId = table.Column<int>(type: "integer", nullable: false),
					Type = table.Column<int>(type: "integer", nullable: false),
					Floor = table.Column<int>(type: "integer", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Rooms", x => new { x.Number, x.ScheduleId });
					table.ForeignKey(
						name: "FK_Rooms_Schedules_ScheduleId",
						column: x => x.ScheduleId,
						principalTable: "Schedules",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "HousekeepingTasks",
				columns: table => new
				{
					Date = table.Column<DateOnly>(type: "date", nullable: false),
					RoomNumber = table.Column<int>(type: "integer", nullable: false),
					RoomScheduleId = table.Column<int>(type: "integer", nullable: true),
					ScheduleId = table.Column<int>(type: "integer", nullable: true),
					Type = table.Column<int>(type: "integer", nullable: true),
					HousekeeperId = table.Column<int>(type: "integer", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_HousekeepingTasks", x => new { x.Date, x.RoomNumber });
					table.ForeignKey(
						name: "FK_HousekeepingTasks_Housekeepers_HousekeeperId",
						column: x => x.HousekeeperId,
						principalTable: "Housekeepers",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_HousekeepingTasks_Rooms_RoomNumber_RoomScheduleId",
						columns: x => new { x.RoomNumber, x.RoomScheduleId },
						principalTable: "Rooms",
						principalColumns: new[] { "Number", "_scheduleId" });
					table.ForeignKey(
						name: "FK_HousekeepingTasks_Schedules_ScheduleId",
						column: x => x.ScheduleId,
						principalTable: "Schedules",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Reservations",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					RoomNumber = table.Column<int>(type: "integer", nullable: true),
					RoomScheduleId = table.Column<int>(type: "integer", nullable: true),
					ScheduleId = table.Column<int>(type: "integer", nullable: true),
					CheckIn = table.Column<DateOnly>(type: "date", nullable: true),
					CheckOut = table.Column<DateOnly>(type: "date", nullable: true),
					RoomType = table.Column<int>(type: "integer", nullable: true),
					FlightArrivalNumber = table.Column<string>(type: "text", nullable: true),
					FlightDepartureNumber = table.Column<string>(type: "text", nullable: true),
					FlightArrivalTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
					FlightDepartureTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
					BookingSource = table.Column<int>(type: "integer", nullable: true),
					Remarks = table.Column<string>(type: "text", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Reservations", x => x.Id);
					table.ForeignKey(
						name: "FK_Reservations_Rooms_RoomNumber_RoomScheduleId",
						columns: x => new { x.RoomNumber, x.RoomScheduleId },
						principalTable: "Rooms",
						principalColumns: new[] { "Number", "_scheduleId" });
					table.ForeignKey(
						name: "FK_Reservations_Schedules_ScheduleId",
						column: x => x.ScheduleId,
						principalTable: "Schedules",
						principalColumn: "Id");
				});

			migrationBuilder.CreateTable(
				name: "Guests",
				columns: table => new
				{
					Id = table.Column<int>(type: "integer", nullable: false)
						.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
					ReservationId = table.Column<int>(type: "integer", nullable: true),
					FirstName = table.Column<string>(type: "text", nullable: true),
					LastName = table.Column<string>(type: "text", nullable: true),
					Age = table.Column<int>(type: "integer", nullable: true),
					Note = table.Column<string>(type: "text", nullable: true),
					Prefix = table.Column<int>(type: "integer", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Guests", x => x.Id);
					table.ForeignKey(
						name: "FK_Guests_Reservations_ReservationId",
						column: x => x.ReservationId,
						principalTable: "Reservations",
						principalColumn: "Id");
				});

			migrationBuilder.CreateIndex(
				name: "IX_Guests_ReservationId",
				table: "Guests",
				column: "ReservationId");

			migrationBuilder.CreateIndex(
				name: "IX_Housekeepers_ScheduleId",
				table: "Housekeepers",
				column: "_scheduleId");

			migrationBuilder.CreateIndex(
				name: "IX_HousekeepingTasks_HousekeeperId",
				table: "HousekeepingTasks",
				column: "HousekeeperId");

			migrationBuilder.CreateIndex(
				name: "IX_HousekeepingTasks_RoomNumber_RoomScheduleId",
				table: "HousekeepingTasks",
				columns: new[] { "RoomNumber", "RoomScheduleId" });

			migrationBuilder.CreateIndex(
				name: "IX_HousekeepingTasks_ScheduleId",
				table: "HousekeepingTasks",
				column: "_scheduleId");

			migrationBuilder.CreateIndex(
				name: "IX_Reservations_RoomNumber_RoomScheduleId",
				table: "Reservations",
				columns: new[] { "RoomNumber", "RoomScheduleId" });

			migrationBuilder.CreateIndex(
				name: "IX_Reservations_ScheduleId",
				table: "Reservations",
				column: "_scheduleId");

			migrationBuilder.CreateIndex(
				name: "IX_Rooms_ScheduleId",
				table: "Rooms",
				column: "_scheduleId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Guests");

			migrationBuilder.DropTable(
				name: "HousekeepingTasks");

			migrationBuilder.DropTable(
				name: "Reservations");

			migrationBuilder.DropTable(
				name: "Housekeepers");

			migrationBuilder.DropTable(
				name: "Rooms");

			migrationBuilder.DropTable(
				name: "Schedules");
		}
	}
}
