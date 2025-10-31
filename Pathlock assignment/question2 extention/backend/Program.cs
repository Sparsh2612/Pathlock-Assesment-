using SmartScheduler.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => o.AddPolicy("all", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors("all");
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

// POST /api/v1/projects/{projectId}/schedule
app.MapPost("/api/v1/projects/{projectId}/schedule", (int projectId, ScheduleRequest request) =>
{
    // Build graph based on task titles
    var titleToTask = request.Tasks.ToDictionary(t => t.Title, StringComparer.OrdinalIgnoreCase);
    var indegree = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    var graph = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

    foreach (var t in request.Tasks)
    {
        indegree[t.Title] = 0;
        graph[t.Title] = new List<string>();
    }

    foreach (var t in request.Tasks)
    {
        foreach (var dep in t.Dependencies.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!graph.ContainsKey(dep)) continue; // ignore unknown deps
            graph[dep].Add(t.Title);
            indegree[t.Title]++;
        }
    }

    // Kahn's algorithm with priority: earlier due date first, then lower estimated hours, then title
    var queue = new List<string>(indegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
    string? DueKey(string title) => titleToTask.TryGetValue(title, out var tt) ? (tt.DueDate?.ToString("yyyy-MM-dd") ?? "9999-12-31") : "9999-12-31";
    int Hours(string title) => titleToTask.TryGetValue(title, out var tt) ? tt.EstimatedHours : int.MaxValue;

    var result = new List<string>();
    while (queue.Count > 0)
    {
        queue.Sort((a,b) => string.Compare(DueKey(a), DueKey(b), StringComparison.Ordinal)
            is var cmp && cmp != 0 ? cmp : Hours(a).CompareTo(Hours(b)) != 0 ? Hours(a).CompareTo(Hours(b)) : string.Compare(a,b,StringComparison.OrdinalIgnoreCase));
        var current = queue[0];
        queue.RemoveAt(0);
        result.Add(current);
        foreach (var next in graph[current])
        {
            indegree[next]--;
            if (indegree[next] == 0) queue.Add(next);
        }
    }

    // If cycle detected, append remaining nodes in stable order to avoid failure
    if (result.Count < request.Tasks.Count)
    {
        var remaining = indegree.Where(kv => kv.Value > 0).Select(kv => kv.Key).ToList();
        remaining.Sort(StringComparer.OrdinalIgnoreCase);
        result.AddRange(remaining);
    }

    return Results.Ok(new ScheduleResponse { RecommendedOrder = result });
});

app.Run();


