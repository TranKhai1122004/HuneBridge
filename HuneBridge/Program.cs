var contentRoot = AppContext.BaseDirectory;
Directory.SetCurrentDirectory(contentRoot);

if (!HuneBridge.Autostart.TryBecomeSingleInstance())
    return;

HuneBridge.Autostart.Register();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot
});

var urls = builder.Configuration["Urls"] ?? "http://localhost:5050";
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

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
