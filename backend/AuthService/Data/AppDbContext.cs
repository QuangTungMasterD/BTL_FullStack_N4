using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@example.com",
                    Password = "admin123",
                    FullName = "Trần Văn Admin",
                    Role = "ADMIN",
                    Phone = "0912345678",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Email = "lecturer@example.com",
                    Password = "lecturer123",
                    FullName = "PGS.TS Nguyễn Văn Giảng",
                    Role = "LECTURER",
                    LecturerId = "GV001",
                    Phone = "0912345679",
                    Title = "Phó Giáo sư",
                    Faculty = "Công nghệ thông tin",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Email = "student@example.com",
                    Password = "student123",
                    FullName = "Nguyễn Văn A",
                    Role = "STUDENT",
                    StudentId = "2021001234",
                    Phone = "0901234567",
                    Faculty = "Công nghệ thông tin",
                    Major = "Khoa học máy tính",
                    Class = "K63-CLC",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}