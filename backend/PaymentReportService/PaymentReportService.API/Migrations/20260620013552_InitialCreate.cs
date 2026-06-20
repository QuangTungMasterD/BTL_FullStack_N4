using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaymentReportService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentInvoices_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LatestInvoiceId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtRecords_PaymentInvoices_LatestInvoiceId",
                        column: x => x.LatestInvoiceId,
                        principalTable: "PaymentInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DebtRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@trungtam.edu.vn", "Quản trị viên", true, "$2a$11$AktP7r048dlnRp3jr1eeE.9rNTfdbmzT5Tf8zQ7xJjYkw07wYWWd.", "Admin", "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "giaovien01@trungtam.edu.vn", "Nguyễn Văn A", true, "$2a$11$AktP7r048dlnRp3jr1eeE.9rNTfdbmzT5Tf8zQ7xJjYkw07wYWWd.", "GiaoVien", "giaovien01" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hocvien01@trungtam.edu.vn", "Trần Thị B", true, "$2a$11$AktP7r048dlnRp3jr1eeE.9rNTfdbmzT5Tf8zQ7xJjYkw07wYWWd.", "HocVien", "hocvien01" }
                });

            migrationBuilder.InsertData(
                table: "PaymentInvoices",
                columns: new[] { "Id", "ClassId", "CourseId", "CourseName", "CreatedByUserId", "InvoiceDate", "Note", "PaidAmount", "PaidDate", "PaymentMethod", "Status", "StudentId", "StudentName", "TotalAmount" },
                values: new object[,]
                {
                    { 1, 1, 1, "Tiếng Anh Giao Tiếp A1", 1, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Đã thanh toán đủ", 3000000m, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "ChuyenKhoan", "DaThanhToan", 1, "Trần Thị B", 3000000m },
                    { 2, 2, 2, "Tin học văn phòng", 1, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Còn nợ 1.500.000đ", 1000000m, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "TienMat", "ThanhToanMotPhan", 2, "Lê Văn C", 2500000m }
                });

            migrationBuilder.InsertData(
                table: "DebtRecords",
                columns: new[] { "Id", "CourseId", "CourseName", "DueDate", "LatestInvoiceId", "Status", "StudentId", "StudentName", "TotalAmount", "TotalPaid", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "Tiếng Anh Giao Tiếp A1", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 1, "DaThanhToan", 1, "Trần Thị B", 3000000m, 3000000m, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 3 },
                    { 2, 2, "Tin học văn phòng", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "ConNo", 2, "Lê Văn C", 2500000m, 1000000m, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtRecords_LatestInvoiceId",
                table: "DebtRecords",
                column: "LatestInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtRecords_UserId",
                table: "DebtRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInvoices_CreatedByUserId",
                table: "PaymentInvoices",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtRecords");

            migrationBuilder.DropTable(
                name: "PaymentInvoices");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
