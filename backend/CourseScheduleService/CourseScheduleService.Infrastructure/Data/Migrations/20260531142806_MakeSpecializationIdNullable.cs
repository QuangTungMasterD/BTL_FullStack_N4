using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeSpecializationIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_specializations_specialization_id",
                table: "courses");

            migrationBuilder.AlterColumn<int>(
                name: "specialization_id",
                table: "courses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_specializations_specialization_id",
                table: "courses",
                column: "specialization_id",
                principalTable: "specializations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_specializations_specialization_id",
                table: "courses");

            migrationBuilder.AlterColumn<int>(
                name: "specialization_id",
                table: "courses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_courses_specializations_specialization_id",
                table: "courses",
                column: "specialization_id",
                principalTable: "specializations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
