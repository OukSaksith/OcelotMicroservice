using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotGateway;
using Prometheus;
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
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing");
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", opts =>
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

// Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Swagger for gateway
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("gateway", new OpenApiInfo { Title = "API Gateway", Version = "v1" });

    // JWT auth in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// Controllers needed for auth
builder.Services.AddControllers();

builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

// Apply migrations for gateway user store
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
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

// Metrics middleware (should come early)
app.UseHttpMetrics();      // records HTTP request metrics
app.UseMetricServer();     // exposes /metrics

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

// Standard middleware order
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/gateway/swagger.json", "API Gateway v1");
    c.RoutePrefix = "docs"; // e.g., /docs
});

// Map auth controller (register/login)
app.MapControllers();

// Ocelot should be after authentication so routes requiring JWT are enforced
await app.UseOcelot();

app.Run();
