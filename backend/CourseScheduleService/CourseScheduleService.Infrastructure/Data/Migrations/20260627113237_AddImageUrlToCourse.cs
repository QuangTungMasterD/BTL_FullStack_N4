using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_url",
                table: "courses");
        }
    }
}
