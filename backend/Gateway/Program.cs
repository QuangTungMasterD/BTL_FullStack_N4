using System.Security.Claims;
using Gateway.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userId))
            context.Request.Headers["X-User-Id"] = userId;

        var roles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);
        if (roles.Any())
            context.Request.Headers["X-Roles"] = string.Join(",", roles);
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await app.UseOcelot();

app.Run();
