using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Api.Migrations
{
    /// <inheritdoc />
    public partial class Auth0ScheduleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "HousekeepingTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Guests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HousekeepingTasks_ScheduleId",
                table: "HousekeepingTasks",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_ScheduleId",
                table: "Guests",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Schedules_ScheduleId",
                table: "Guests",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HousekeepingTasks_Schedules_ScheduleId",
                table: "HousekeepingTasks",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Schedules_ScheduleId",
                table: "Guests");

            migrationBuilder.DropForeignKey(
                name: "FK_HousekeepingTasks_Schedules_ScheduleId",
                table: "HousekeepingTasks");

            migrationBuilder.DropIndex(
                name: "IX_HousekeepingTasks_ScheduleId",
                table: "HousekeepingTasks");

            migrationBuilder.DropIndex(
                name: "IX_Guests_ScheduleId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "HousekeepingTasks");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Guests");
        }
    }
}
