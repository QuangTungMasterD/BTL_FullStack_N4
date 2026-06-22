using Microsoft.EntityFrameworkCore;
using PaymentReportService.API.Models;

namespace PaymentReportService.API.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<PaymentInvoice> PaymentInvoices => Set<PaymentInvoice>();
    public DbSet<DebtRecord> DebtRecords => Set<DebtRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================== User ====================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.FullName).IsRequired().HasMaxLength(200);
            entity.Property(u => u.Email).HasMaxLength(200);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
        });

        // ==================== PaymentInvoice ====================
        modelBuilder.Entity<PaymentInvoice>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.PaidAmount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.CourseName).HasMaxLength(300);
            entity.Property(p => p.StudentName).HasMaxLength(200);
            entity.Property(p => p.Status).HasMaxLength(30);
            entity.Property(p => p.PaymentMethod).HasMaxLength(30);

            entity.HasOne(p => p.CreatedByUser)
                  .WithMany(u => u.Invoices)
                  .HasForeignKey(p => p.CreatedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==================== DebtRecord ====================
        modelBuilder.Entity<DebtRecord>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(d => d.TotalPaid).HasColumnType("decimal(18,2)");
            entity.Property(d => d.Status).HasMaxLength(20);
            entity.Property(d => d.StudentName).HasMaxLength(200);
            entity.Property(d => d.CourseName).HasMaxLength(300);

            // RemainingDebt là computed property, không map vào DB
            entity.Ignore(d => d.RemainingDebt);

            entity.HasOne(d => d.LatestInvoice)
                  .WithMany(p => p.DebtRecords)
                  .HasForeignKey(d => d.LatestInvoiceId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.User)
                  .WithMany(u => u.DebtRecords)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ==================== SEED DATA ====================
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed 3 tài khoản demo: 1 Admin, 1 GiaoVien, 1 HocVien
        // Password cho cả 3: "Password123"
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = passwordHash,
                FullName = "Quản trị viên",
                Email = "admin@trungtam.edu.vn",
                Role = "ADMIN",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "giaovien01",
                PasswordHash = passwordHash,
                FullName = "Nguyễn Văn A",
                Email = "giaovien01@trungtam.edu.vn",
                Role = "LECTURER",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new User
            {
                Id = 3,
                Username = "hocvien01",
                PasswordHash = passwordHash,
                FullName = "Trần Thị B",
                Email = "hocvien01@trungtam.edu.vn",
                Role = "STUDENT",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            }
        );

        // Seed 2 phiếu thu mẫu
        modelBuilder.Entity<PaymentInvoice>().HasData(
            new PaymentInvoice
            {
                Id = 1,
                StudentId = 1,
                CourseId = 1,
                ClassId = 1,
                CourseName = "Tiếng Anh Giao Tiếp A1",
                StudentName = "Trần Thị B",
                TotalAmount = 3000000,
                PaidAmount = 3000000,
                Status = "DaThanhToan",
                PaymentMethod = "ChuyenKhoan",
                Note = "Đã thanh toán đủ",
                InvoiceDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                PaidDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            },
            new PaymentInvoice
            {
                Id = 2,
                StudentId = 2,
                CourseId = 2,
                ClassId = 2,
                CourseName = "Tin học văn phòng",
                StudentName = "Lê Văn C",
                TotalAmount = 2500000,
                PaidAmount = 1000000,
                Status = "ThanhToanMotPhan",
                PaymentMethod = "TienMat",
                Note = "Còn nợ 1.500.000đ",
                InvoiceDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                PaidDate = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedByUserId = 1
            }
        );

        // Seed debt records
        modelBuilder.Entity<DebtRecord>().HasData(
            new DebtRecord
            {
                Id = 1,
                StudentId = 1,
                StudentName = "Trần Thị B",
                CourseId = 1,
                CourseName = "Tiếng Anh Giao Tiếp A1",
                TotalAmount = 3000000,
                TotalPaid = 3000000,
                Status = "DaThanhToan",
                DueDate = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                LatestInvoiceId = 1,
                UserId = 3
            },
            new DebtRecord
            {
                Id = 2,
                StudentId = 2,
                StudentName = "Lê Văn C",
                CourseId = 2,
                CourseName = "Tin học văn phòng",
                TotalAmount = 2500000,
                TotalPaid = 1000000,
                Status = "ConNo",
                DueDate = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                LatestInvoiceId = 2,
                UserId = 3
            }
        );
    }
}