using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSpecializationAndAddCourseTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_specializations_specialization_id",
                table: "courses");

            migrationBuilder.DropTable(
                name: "teacher_specializations");

            migrationBuilder.DropTable(
                name: "specializations");

            migrationBuilder.DropIndex(
                name: "IX_courses_specialization_id",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "specialization_id",
                table: "courses");

            migrationBuilder.CreateTable(
                name: "course_teachers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_teachers", x => x.id);
                    table.ForeignKey(
                        name: "FK_course_teachers_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_course_teachers_teachers_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_teachers_course_id",
                table: "course_teachers",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_course_teachers_teacher_id",
                table: "course_teachers",
                column: "teacher_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_teachers");

            migrationBuilder.AddColumn<int>(
                name: "specialization_id",
                table: "courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "specializations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    descrt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    specialization_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_specializations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "teacher_specializations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    specialization_id = table.Column<int>(type: "int", nullable: false),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teacher_specializations", x => x.id);
                    table.ForeignKey(
                        name: "FK_teacher_specializations_specializations_specialization_id",
                        column: x => x.specialization_id,
                        principalTable: "specializations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_teacher_specializations_teachers_teacher_id",
                        column: x => x.teacher_id,
                        principalTable: "teachers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_courses_specialization_id",
                table: "courses",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_specializations_specialization_id",
                table: "teacher_specializations",
                column: "specialization_id");

            migrationBuilder.CreateIndex(
                name: "IX_teacher_specializations_teacher_id",
                table: "teacher_specializations",
                column: "teacher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_specializations_specialization_id",
                table: "courses",
                column: "specialization_id",
                principalTable: "specializations",
                principalColumn: "id");
        }
    }
}
