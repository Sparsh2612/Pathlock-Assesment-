using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CodingAssignment.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();
// Register in-memory repository as singleton
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
// Allow CORS for local React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();
