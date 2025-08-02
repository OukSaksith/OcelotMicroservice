using Microsoft.Data.SqlClient;
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
    var maxAttempts = 8;
    var delay = TimeSpan.FromSeconds(5);
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            var pending = await db.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                Console.WriteLine("Applying pending migrations: " + string.Join(", ", pending));
                await db.Database.MigrateAsync();
            }
            else
            {
                Console.WriteLine("No pending migrations.");
            }
            break; // success
        }
        catch (SqlException sqlEx) when (sqlEx.Number == 1801)
        {
            // Database already exists race condition when EF tried to create it concurrently—safe to ignore.
            Console.WriteLine($"Ignored SQL error 1801 (database exists): {sqlEx.Message}");
            // Try again to apply migrations (the loop will repeat)
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Attempt {attempt} - database migration failed: {ex.Message}");
            if (attempt == maxAttempts)
                throw;
            await Task.Delay(delay);
            delay = delay * 2;
        }
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
