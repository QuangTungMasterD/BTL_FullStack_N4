using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiPreferredSessionToScheduleChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "suggested_date",
                table: "schedule_change_requests",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "suggested_date",
                table: "schedule_change_requests");
        }
    }
}
