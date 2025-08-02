using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TeacherService;
using TeacherService.Services;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<TeacherDbContext>(options =>
    options.UseSqlServer(connection));

builder.Services.AddScoped<ITeacherService, TeacherServiceImpl>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TeacherService API",
        Version = "v1",
        Description = "CRUD API for Teachers"
    });
});

builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TeacherDbContext>();
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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeacherService v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();

app.MapControllers();

app.Run();
