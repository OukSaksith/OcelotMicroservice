using Microsoft.AspNetCore.Builder;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Configuration.AddJsonFile("ocelot.json",optional:false,reloadOnChange:true);
builder.Services.AddOcelot(builder.Configuration);
builder.WebHost.UseUrls("http://*:80");




var app = builder.Build();

// Enable Prometheus metrics
// Log requests
app.Use(async (context, next) =>
{
    // Generate a reference UUID per request
    var refId = Guid.NewGuid().ToString();

    // Optional: Use thread/task ID for debugging
    var threadId = Environment.CurrentManagedThreadId;
    var taskId = Task.CurrentId?.ToString() ?? "none";

    // Add RefId to request headers (to be used by downstream services)
    context.Request.Headers["X-Ref-Id"] = refId;

    // Add it to the response as well
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Ref-Id"] = refId;
        return Task.CompletedTask;
    });

    var method = context.Request.Method;
    var path = context.Request.Path;
    var start = DateTime.UtcNow;

    await next.Invoke();

    var duration = DateTime.UtcNow - start;
    var status = context.Response.StatusCode;

    Console.WriteLine($"[{DateTime.Now}] REF_ID_={refId} | Thread={threadId} | Task={taskId} | {method} {path} => {status} in {duration.TotalMilliseconds}ms");
});

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMetricServer();    // Exposes /metrics
app.UseHttpMetrics();

app.MapControllers();

await app.UseOcelot();

app.Run();
