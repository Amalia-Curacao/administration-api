using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Scheduler.Api.Migrations
{
    /// <inheritdoc />
    public partial class UsersAndScheduleInviteLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Housekeepers_Schedules_ScheduleId",
                table: "Housekeepers");

            migrationBuilder.DropForeignKey(
                name: "FK_HousekeepingTasks_Housekeepers_HousekeeperId",
                table: "HousekeepingTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_HousekeepingTasks_Schedules_ScheduleId",
                table: "HousekeepingTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Schedules_ScheduleId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HousekeepingTasks",
                table: "HousekeepingTasks");

            migrationBuilder.DropIndex(
                name: "IX_HousekeepingTasks_ScheduleId",
                table: "HousekeepingTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Housekeepers",
                table: "Housekeepers");

            migrationBuilder.DropIndex(
                name: "IX_Housekeepers_ScheduleId",
                table: "Housekeepers");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "HousekeepingTasks");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Housekeepers");

            migrationBuilder.RenameTable(
                name: "Housekeepers",
                newName: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "RoomScheduleId",
                table: "HousekeepingTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Auth0Id",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HousekeepingTasks",
                table: "HousekeepingTasks",
                columns: new[] { "Date", "RoomNumber", "RoomScheduleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ScheduleInviteLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Disabled = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    ScheduleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleInviteLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleInviteLinks_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleInviteLinks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleInviteLinks_ScheduleId",
                table: "ScheduleInviteLinks",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleInviteLinks_UserId",
                table: "ScheduleInviteLinks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HousekeepingTasks_Users_HousekeeperId",
                table: "HousekeepingTasks",
                column: "HousekeeperId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Schedules_ScheduleId",
                table: "Reservations",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HousekeepingTasks_Users_HousekeeperId",
                table: "HousekeepingTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Schedules_ScheduleId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "ScheduleInviteLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HousekeepingTasks",
                table: "HousekeepingTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Auth0Id",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Housekeepers");

            migrationBuilder.AlterColumn<int>(
                name: "RoomScheduleId",
                table: "HousekeepingTasks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "HousekeepingTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Housekeepers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HousekeepingTasks",
                table: "HousekeepingTasks",
                columns: new[] { "Date", "RoomNumber" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Housekeepers",
                table: "Housekeepers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HousekeepingTasks_ScheduleId",
                table: "HousekeepingTasks",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Housekeepers_ScheduleId",
                table: "Housekeepers",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Housekeepers_Schedules_ScheduleId",
                table: "Housekeepers",
                column: "ScheduleId",
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
                name: "FK_HousekeepingTasks_Schedules_ScheduleId",
                table: "HousekeepingTasks",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Schedules_ScheduleId",
                table: "Reservations",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
