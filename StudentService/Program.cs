using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StudentService;
using StudentService.Service;
using StudentService.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(connection));

// Business service
builder.Services.AddScoped<IStudentService, StudentServiceImpl>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StudentService API",
        Version = "v1",
        Description = "CRUD API for Students"
    });
});

builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

// Auto-migrate in development
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
    var pending = await db.Database.GetPendingMigrationsAsync();
    try
    {
        if (pending.Any())
        {
            await db.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
        throw;
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentService v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();
