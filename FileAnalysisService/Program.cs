using FileAnalysisService.Data;
using FileAnalysisService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PostgresConnection");
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<AnalysisService>();

builder.Services.AddHttpClient("FileStorage", client =>
{
    client.BaseAddress = new Uri("http://filestoring/api/");
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    db.Database.Migrate();
}

app.Run();