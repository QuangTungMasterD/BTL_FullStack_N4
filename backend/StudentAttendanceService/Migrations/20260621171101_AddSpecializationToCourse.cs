using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentAttendanceService.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecializationToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TargetRole = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StudentCount = table.Column<int>(type: "int", nullable: false),
                    LecturerCount = table.Column<int>(type: "int", nullable: false),
                    Head = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lecturers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LecturerId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Faculty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecializationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descrt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Faculty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Major = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Class = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Faculty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LecturerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    Faculty = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LecturerId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: true),
                    Schedule = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Room = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxStudents = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Lecturers_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Courses_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Week = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendances_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalGrade = table.Column<double>(type: "float", nullable: true),
                    LetterGrade = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearningResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Score = table.Column<double>(type: "float", nullable: true),
                    MaxScore = table.Column<double>(type: "float", nullable: true),
                    Weight = table.Column<double>(type: "float", nullable: true),
                    Grade = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningResults_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Announcements",
                columns: new[] { "Id", "Author", "Content", "Date", "Priority", "TargetRole", "Title" },
                values: new object[,]
                {
                    { 1, "Phòng Đào tạo", "Lịch thi cuối kỳ học kỳ Fall 2024 đã được công bố...", new DateTime(2026, 6, 16, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(465), "high", "ALL", "Thông báo lịch thi cuối kỳ" },
                    { 2, "Phòng Đào tạo", "Thời gian đăng ký học phần bắt đầu từ ngày 15/12/2024...", new DateTime(2026, 6, 13, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(470), "medium", "STUDENT", "Đăng ký học phần học kỳ Spring 2025" }
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "CreatedAt", "Head", "LecturerCount", "Name", "StudentCount" },
                values: new object[,]
                {
                    { 1, "CNTT", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(340), "PGS.TS Trần Văn X", 35, "Công nghệ thông tin", 486 },
                    { 2, "QTKD", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(343), "TS. Nguyễn Thị Y", 28, "Quản trị kinh doanh", 325 },
                    { 3, "NNKT", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(345), "ThS. Lê Văn Z", 20, "Ngôn ngữ Anh", 218 }
                });

            migrationBuilder.InsertData(
                table: "Lecturers",
                columns: new[] { "Id", "CreatedAt", "Email", "Faculty", "FullName", "LecturerId", "Phone", "Specialization", "Status", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(389), "tranvanx@university.edu.vn", "CNTT", "PGS.TS Trần Văn X", "GV001", "0912345678", "Khoa học máy tính", "Active", "Phó Giáo sư", null },
                    { 2, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(394), "nguyenthiy@university.edu.vn", "QTKD", "TS. Nguyễn Thị Y", "GV002", "0912345679", "Quản trị chiến lược", "Active", "Tiến sĩ", null },
                    { 3, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(396), "levanz@university.edu.vn", "NNKT", "ThS. Lê Văn Z", "GV003", "0912345680", "Ngôn ngữ học", "Active", "Thạc sĩ", null }
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "Id", "CreatedAt", "Descrt", "IsActive", "SpecializationName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(363), "Chuyên ngành nghiên cứu và phát triển phần mềm", true, "Khoa học máy tính", null },
                    { 2, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(365), "Chuyên ngành phát triển ứng dụng và hệ thống", true, "Kỹ thuật phần mềm", null },
                    { 3, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(366), "Chuyên ngành quản lý và điều hành doanh nghiệp", true, "Quản trị kinh doanh", null },
                    { 4, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(368), "Chuyên ngành ngôn ngữ và văn hóa Anh", true, "Ngôn ngữ Anh", null },
                    { 5, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(369), "Chuyên ngành AI và Machine Learning", true, "Trí tuệ nhân tạo", null }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "Class", "CreatedAt", "Email", "Faculty", "FullName", "Major", "Phone", "Status", "StudentId", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { 1, null, "K63-CLC", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(438), "nguyenvana@university.edu.vn", "CNTT", "Nguyễn Văn A", "Khoa học máy tính", "0901234567", "Active", "2021001234", null, 2021 },
                    { 2, null, "K63-CLC", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(442), "tranthib@university.edu.vn", "CNTT", "Trần Thị B", "Khoa học máy tính", "0901234568", "Active", "2021001235", null, 2021 },
                    { 3, null, "K63-QTKD", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(444), "levanc@university.edu.vn", "QTKD", "Lê Văn C", "Quản trị kinh doanh", "0901234569", "Active", "2021001236", null, 2021 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Class", "CreatedAt", "Email", "Faculty", "FullName", "IsActive", "LecturerId", "Major", "Password", "Phone", "Role", "Specialization", "StudentId", "Title" },
                values: new object[] { 1, null, new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(223), "admin@example.com", "Phòng Đào tạo", "Trần Văn Admin", true, null, null, "admin123", "0912345678", "ADMIN", null, null, "Quản trị viên" });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Code", "CreatedAt", "Credits", "Faculty", "LecturerId", "MaxStudents", "Name", "Room", "Schedule", "Semester", "SpecializationId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CS101", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(412), 3, "CNTT", 1, 60, "Nhập môn lập trình", "A101", "Thứ 2, 13:30-16:30", "Fall 2024", 1, null },
                    { 2, "CS201", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(417), 4, "CNTT", 1, 60, "Cấu trúc dữ liệu", "B202", "Thứ 3, 08:00-11:00", "Fall 2024", 1, null },
                    { 3, "CS301", new DateTime(2026, 6, 21, 17, 11, 1, 519, DateTimeKind.Utc).AddTicks(419), 3, "CNTT", 2, 60, "Cơ sở dữ liệu", "C303", "Thứ 4, 13:30-16:30", "Fall 2024", 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CourseId",
                table: "Attendances",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_LecturerId",
                table: "Courses",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SpecializationId",
                table: "Courses",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningResults_EnrollmentId",
                table: "LearningResults",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_SpecializationName",
                table: "Specializations",
                column: "SpecializationName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "LearningResults");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Lecturers");

            migrationBuilder.DropTable(
                name: "Specializations");
        }
    }
}
