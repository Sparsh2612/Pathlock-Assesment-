using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PathlockProjects.Models;
using PathlockProjects.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDataStore, InMemoryStore>();
builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddCors(o => o.AddPolicy("all", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("all");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

// Helpers
Guid GetUserId(ClaimsPrincipal user) => Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

// Auth
app.MapPost("/api/auth/register", (RegisterRequest req, IDataStore db, IJwtService jwt) =>
{
    if (db.FindUser(req.Username) != null) return Results.BadRequest("Username already exists");
    var user = new User { Username = req.Username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password) };
    db.AddUser(user);
    var token = jwt.GenerateToken(user.Id, user.Username);
    return Results.Ok(new { token, username = user.Username });
});

app.MapPost("/api/auth/login", (LoginRequest req, IDataStore db, IJwtService jwt) =>
{
    var user = db.FindUser(req.Username);
    if (user == null) return Results.Unauthorized();
    if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash)) return Results.Unauthorized();
    var token = jwt.GenerateToken(user.Id, user.Username);
    return Results.Ok(new { token, username = user.Username });
});

// Projects
app.MapGet("/api/projects", (HttpContext ctx, IDataStore db) =>
{
    var uid = GetUserId(ctx.User);
    return Results.Ok(db.GetProjects(uid));
}).RequireAuthorization();

app.MapPost("/api/projects", (HttpContext ctx, IDataStore db, ProjectCreateRequest req) =>
{
    var uid = GetUserId(ctx.User);
    var p = db.AddProject(new Project { Title = req.Title, Description = req.Description, OwnerId = uid });
    return Results.Created($"/api/projects/{p.Id}", p);
}).RequireAuthorization();

app.MapGet("/api/projects/{id}", (HttpContext ctx, IDataStore db, Guid id) =>
{
    var uid = GetUserId(ctx.User);
    var p = db.GetProject(uid, id);
    return p is null ? Results.NotFound() : Results.Ok(p);
}).RequireAuthorization();

app.MapDelete("/api/projects/{id}", (HttpContext ctx, IDataStore db, Guid id) =>
{
    var uid = GetUserId(ctx.User);
    return db.DeleteProject(uid, id) ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

// Tasks
app.MapPost("/api/projects/{projectId}/tasks", (HttpContext ctx, IDataStore db, Guid projectId, TaskCreateRequest req) =>
{
    var uid = GetUserId(ctx.User);
    var p = db.GetProject(uid, projectId);
    if (p is null) return Results.NotFound("Project not found");
    var t = db.AddTask(new TaskItem { Title = req.Title, DueDate = req.DueDate, ProjectId = projectId, OwnerId = uid, IsCompleted = false });
    return Results.Created($"/api/tasks/{t.Id}", t);
}).RequireAuthorization();

app.MapPut("/api/tasks/{taskId}", (HttpContext ctx, IDataStore db, Guid taskId, TaskUpdateRequest req) =>
{
    var uid = GetUserId(ctx.User);
    var existing = db.GetTask(uid, taskId);
    if (existing is null) return Results.NotFound();
    existing.Title = req.Title;
    existing.DueDate = req.DueDate;
    existing.IsCompleted = req.IsCompleted;
    var updated = db.UpdateTask(existing)!;
    return Results.Ok(updated);
}).RequireAuthorization();

app.MapDelete("/api/tasks/{taskId}", (HttpContext ctx, IDataStore db, Guid taskId) =>
{
    var uid = GetUserId(ctx.User);
    return db.DeleteTask(uid, taskId) ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.Run();


