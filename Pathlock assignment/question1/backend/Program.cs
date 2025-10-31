using PathlockTasks.Models;
using PathlockTasks.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("all");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health
app.MapGet("/", () => Results.Redirect("/swagger"));

// API (no MapGroup to ensure delegate overloads resolve cleanly)
app.MapGet("/api/tasks", (ITaskRepository repo) => Results.Ok(repo.GetAll()));

app.MapPost("/api/tasks", (ITaskRepository repo, TaskItem item) =>
{
    if (string.IsNullOrWhiteSpace(item.Description))
        return Results.BadRequest("Description is required");
    var created = repo.Add(new TaskItem { Description = item.Description, IsCompleted = item.IsCompleted });
    return Results.Created($"/api/tasks/{created.Id}", created);
});

app.MapPut("/api/tasks/{id}", (ITaskRepository repo, Guid id, TaskItem item) =>
{
    var updated = repo.Update(id, item);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

app.MapDelete("/api/tasks/{id}", (ITaskRepository repo, Guid id) =>
{
    var ok = repo.Delete(id);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.Run();


