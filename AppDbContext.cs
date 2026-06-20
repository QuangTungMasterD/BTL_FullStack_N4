using Microsoft.EntityFrameworkCore;
using StudentAttendanceService.Models;

namespace StudentAttendanceService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
    }
}
