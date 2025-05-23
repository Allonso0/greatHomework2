using FileStoringService.Data;
using FileStoringService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__PostgresConnection");

Console.WriteLine($"[DB] Connection string: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("PostgreSQL connection string is missing!");
}

builder.Services.AddDbContext<FileDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<FileStorageService>();

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
    var db = scope.ServiceProvider.GetRequiredService<FileDbContext>();
    db.Database.Migrate();
}

app.Run();