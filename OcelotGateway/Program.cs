using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway;
//using Prometheus;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot config
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// EF Core for users
builder.Services.AddDbContext<GatewayDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT service
builder.Services.AddSingleton<JwtService>();

// Authentication setup
var jwtSecret = builder.Configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("Jwt:Secret missing");
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// Authorization (no global policy that would block swagger)
builder.Services.AddAuthorization();

// Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Controllers needed for auth
builder.Services.AddControllers();

// Swagger for gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("gateway", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1"
    });

    // JWT auth in Swagger
    //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    Type = SecuritySchemeType.Http,
    //    Scheme = "bearer",
    //    BearerFormat = "JWT",
    //    Name = "Authorization",
    //    In = ParameterLocation.Header,
    //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    //});

    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            },
    //            Scheme = "bearer",
    //            Name = "Bearer",
    //            In = ParameterLocation.Header
    //        },
    //        Array.Empty<string>()
    //    }
    //});
});

// Allow anonymous access to swagger endpoints if any global auth middleware is added elsewhere.
// (If you later add global filters requiring auth, ensure swagger endpoints are exempted.)

builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

// Apply migrations for gateway user store with retry logic
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
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
            // Loop will retry applying migrations
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

// Metrics middleware (early)
//app.UseHttpMetrics();
//app.UseMetricServer();

// Custom request logging with RefId
app.Use(async (context, next) =>
{
    var refId = Guid.NewGuid().ToString();
    var threadId = Environment.CurrentManagedThreadId;
    var taskId = Task.CurrentId?.ToString() ?? "none";
    context.Request.Headers["X-Ref-Id"] = refId;
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Ref-Id"] = refId;
        return Task.CompletedTask;
    });

    var method = context.Request.Method;
    var path = context.Request.Path;
    var start = DateTime.UtcNow;

    await next();

    var duration = DateTime.UtcNow - start;
    var status = context.Response.StatusCode;

    Console.WriteLine($"[{DateTime.Now}] REF_ID={refId} | Thread={threadId} | Task={taskId} | {method} {path} => {status} in {duration.TotalMilliseconds}ms");
});

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/gateway/swagger.json", "API Gateway v1");
    c.RoutePrefix = string.Empty;
});
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/gateway/auth"),
    authApp => authApp.UseRouting().UseAuthentication().UseAuthorization().UseEndpoints(e => e.MapControllers())
);
app.MapControllers();

await app.UseOcelot();

app.Run();
