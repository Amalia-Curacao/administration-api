using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Api.Migrations
{
	/// <inheritdoc />
	public partial class Deletecontstraints : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Guests_Reservations_ReservationId",
				table: "Guests");

			migrationBuilder.DropForeignKey(
				name: "FK_Housekeepers_Schedules_ScheduleId",
				table: "Housekeepers");

			migrationBuilder.DropForeignKey(
				name: "FK_HousekeepingTasks_Housekeepers_HousekeeperId",
				table: "HousekeepingTasks");

			migrationBuilder.DropForeignKey(
				name: "FK_HousekeepingTasks_Rooms_RoomNumber_RoomScheduleId",
				table: "HousekeepingTasks");

			migrationBuilder.DropForeignKey(
				name: "FK_HousekeepingTasks_Schedules_ScheduleId",
				table: "HousekeepingTasks");

			migrationBuilder.DropForeignKey(
				name: "FK_Reservations_Rooms_RoomNumber_RoomScheduleId",
				table: "Reservations");

			migrationBuilder.DropForeignKey(
				name: "FK_Reservations_Schedules_ScheduleId",
				table: "Reservations");

			migrationBuilder.AddForeignKey(
				name: "FK_Guests_Reservations_ReservationId",
				table: "Guests",
				column: "ReservationId",
				principalTable: "Reservations",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Housekeepers_Schedules_ScheduleId",
				table: "Housekeepers",
				column: "_scheduleId",
				principalTable: "Schedules",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_HousekeepingTasks_Housekeepers_HousekeeperId",
				table: "HousekeepingTasks",
				column: "HousekeeperId",
				principalTable: "Housekeepers",
				principalColumn: "Id",
				onDelete: ReferentialAction.SetNull);

			migrationBuilder.AddForeignKey(
				name: "FK_HousekeepingTasks_Rooms_RoomNumber_RoomScheduleId",
				table: "HousekeepingTasks",
				columns: new[] { "RoomNumber", "RoomScheduleId" },
				principalTable: "Rooms",
				principalColumns: new[] { "Number", "_scheduleId" },
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_HousekeepingTasks_Schedules_ScheduleId",
				table: "HousekeepingTasks",
				column: "_scheduleId",
				principalTable: "Schedules",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Reservations_Rooms_RoomNumber_RoomScheduleId",
				table: "Reservations",
				columns: new[] { "RoomNumber", "RoomScheduleId" },
				principalTable: "Rooms",
				principalColumns: new[] { "Number", "_scheduleId" },
				onDelete: ReferentialAction.SetNull);

			migrationBuilder.AddForeignKey(
				name: "FK_Reservations_Schedules_ScheduleId",
				table: "Reservations",
				column: "_scheduleId",
				principalTable: "Schedules",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Guests_Reservations_ReservationId",
				table: "Guests");

			migrationBuilder.DropForeignKey(
				name: "FK_Housekeepers_Schedules_ScheduleId",
				table: "Housekeepers");

			migrationBuilder.DropForeignKey(
				name: "FK_HousekeepingTasks_Housekeepers_HousekeeperId",
				table: "HousekeepingTasks");

			migrationBuilder.DropForeignKey(
				name: "FK_HousekeepingTasks_Rooms_RoomNumber_RoomScheduleId",
				table: "HousekeepingTasks");

			migrationBuilder.DropForeignKey(
				name: "FK_HousekeepingTasks_Schedules_ScheduleId",
				table: "HousekeepingTasks");

			migrationBuilder.DropForeignKey(
				name: "FK_Reservations_Rooms_RoomNumber_RoomScheduleId",
				table: "Reservations");

			migrationBuilder.DropForeignKey(
				name: "FK_Reservations_Schedules_ScheduleId",
				table: "Reservations");

			migrationBuilder.AddForeignKey(
				name: "FK_Guests_Reservations_ReservationId",
				table: "Guests",
				column: "ReservationId",
				principalTable: "Reservations",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_Housekeepers_Schedules_ScheduleId",
				table: "Housekeepers",
				column: "_scheduleId",
				principalTable: "Schedules",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_HousekeepingTasks_Housekeepers_HousekeeperId",
				table: "HousekeepingTasks",
				column: "HousekeeperId",
				principalTable: "Housekeepers",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_HousekeepingTasks_Rooms_RoomNumber_RoomScheduleId",
				table: "HousekeepingTasks",
				columns: new[] { "RoomNumber", "RoomScheduleId" },
				principalTable: "Rooms",
				principalColumns: new[] { "Number", "_scheduleId" });

			migrationBuilder.AddForeignKey(
				name: "FK_HousekeepingTasks_Schedules_ScheduleId",
				table: "HousekeepingTasks",
				column: "_scheduleId",
				principalTable: "Schedules",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_Reservations_Rooms_RoomNumber_RoomScheduleId",
				table: "Reservations",
				columns: new[] { "RoomNumber", "RoomScheduleId" },
				principalTable: "Rooms",
				principalColumns: new[] { "Number", "_scheduleId" });

			migrationBuilder.AddForeignKey(
				name: "FK_Reservations_Schedules_ScheduleId",
				table: "Reservations",
				column: "_scheduleId",
				principalTable: "Schedules",
				principalColumn: "Id");
		}
	}
}
