using ClazzService;
using ClazzService.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Configuration / Services ----

// DbContext (expects connection string with Database=ClazzDb)
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found.");
builder.Services.AddDbContext<ClazzDbContext>(options =>
    options.UseSqlServer(connection));

// Domain service
builder.Services.AddScoped<IClazzService, ClazzServiceImpl>();

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClazzService API",
        Version = "v1",
        Description = "CRUD API for Class"
    });
});


builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

// ---- Apply pending migrations safely ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClazzDbContext>();
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

// ---- Middleware ----
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClazzService v1");
    c.RoutePrefix = string.Empty; // Serve at root
});

app.MapControllers();

app.Run();
