using System;
using System.Collections.Generic;
using CourseScheduleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseScheduleService.Infrastructure.Data;

public partial class CourseScheduleDbContext : DbContext
{
    public CourseScheduleDbContext()
    {
    }

    public CourseScheduleDbContext(DbContextOptions<CourseScheduleDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassSession> ClassSessions { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
    public DbSet<TeacherSpecialization> TeacherSpecializations { get; set; }
    public DbSet<ScheduleChangeRequest> ScheduleChangeRequests { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=course_schedule_db;User Id=sa;Password=Tung@2005;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleChangeRequest>(entity =>
        {
            entity.HasOne(r => r.Teacher)
                .WithMany()
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ClassSession)
                .WithMany()
                .HasForeignKey(r => r.ClassSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.SuggestedRoom)
                .WithMany()
                .HasForeignKey(r => r.SuggestedRoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
