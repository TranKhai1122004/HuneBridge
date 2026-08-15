var contentRoot = AppContext.BaseDirectory;
Directory.SetCurrentDirectory(contentRoot);

if (!HuneBridge.Autostart.TryBecomeSingleInstance())
{
    Console.WriteLine("HuneBridge is already running.");
    return;
}

HuneBridge.Autostart.Register();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot
});

var urls = builder.Configuration["Urls"] ?? "http://127.0.0.1:5050";
builder.WebHost.UseUrls(urls);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.Request.Headers.ContainsKey("Access-Control-Request-Private-Network"))
            context.Response.Headers["Access-Control-Allow-Private-Network"] = "true";
        return Task.CompletedTask;
    });
    await next();
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    Console.WriteLine("Press Enter to exit.");
    Console.ReadLine();
}
