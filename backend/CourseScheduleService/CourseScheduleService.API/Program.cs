using CourseScheduleService.API.Filters;
using CourseScheduleService.API.Middlewares;
using CourseScheduleService.Application.Interfaces.Services;
using CourseScheduleService.Application.Mapping;
using CourseScheduleService.Application.Services;
using CourseScheduleService.Domain.Interfaces.Repositories;
using CourseScheduleService.Infrastructure.Data;
using CourseScheduleService.Infrastructure.Repositories;
using CourseScheduleService.interfaces.services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CourseScheduleDbContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);


// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICourseRepository), typeof(CourseRepository));
builder.Services.AddScoped(typeof(ISpecializationRepository), typeof(SpecializationRepository));
builder.Services.AddScoped(typeof(ITeacherRepository), typeof(TeacherRepository));
builder.Services.AddScoped(typeof(IClassRepository), typeof(ClassRepository));
builder.Services.AddScoped(typeof(IRoomRepository), typeof(RoomRepository));
builder.Services.AddScoped(typeof(ITeacherAssignmentRepository), typeof(TeacherAssignmentRepository));
builder.Services.AddScoped(typeof(IClassSessionRepository), typeof(ClassSessionRepository));

// Register services
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
builder.Services.AddScoped<IClassSessionService, ClassSessionService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(ClassMapping));
builder.Services.AddAutoMapper(typeof(ClassSessionMapping));
builder.Services.AddAutoMapper(typeof(CourseMapping));
builder.Services.AddAutoMapper(typeof(RoomMapping));
builder.Services.AddAutoMapper(typeof(SpecializationMapping));
builder.Services.AddAutoMapper(typeof(TeacherAssignmentMapping));
builder.Services.AddAutoMapper(typeof(TeacherMapping));
builder.Services.AddAutoMapper(typeof(TeacherSpecializationMapping));

// Register Filter
builder.Services.AddScoped<ValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("========================================");
Console.WriteLine($"🔥 Connection String đang dùng: {connectionString}");
Console.WriteLine("========================================");

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<CourseScheduleDbContext>();

    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/health");

app.UseMiddleware<HeaderClaimsMiddleware>();

app.UseAuthorization();

app.Run();
