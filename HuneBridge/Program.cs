var builder = WebApplication.CreateBuilder(args);

// Cố định port cho cả Dev (dotnet run) và Exe — không phụ thuộc launchSettings
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
