using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Api.Migrations
{
    /// <inheritdoc />
    public partial class MovedNotesFromUserToInviteLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "ScheduleInviteLinks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "ScheduleInviteLinks");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
