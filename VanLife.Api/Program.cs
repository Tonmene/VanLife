using VanLife.Api.Data;
using VanLife.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Default to in-memory database. To use PostgreSQL, install a compatible Npgsql EF Core provider
// and replace this registration with UseNpgsql(connectionString).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("VanLife"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<VanService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<IncomeService>();
builder.Services.AddScoped<ImageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.MapFallback(() =>
{
    return Results.NotFound(new
    {
        statusCode = 404,
        message = "Endpoint not found. Please check your URL."
    });
});

app.Run();

