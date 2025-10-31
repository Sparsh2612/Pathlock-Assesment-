# SchedulerApi (Question 2 Extension)

Minimal .NET 8 API that recommends an execution order for tasks based on dependencies, due dates and estimated hours.

## Run
```bash
cd "Question 2 Extension/Backend/SchedulerApi"
# ensure swagger package exists (once)
# dotnet add package Swashbuckle.AspNetCore
# run on a known port
dotnet run --urls http://localhost:5000
```
Open Swagger: `http://localhost:5000/swagger`.

## Endpoint
- POST `/api/v1/projects/{projectId}/schedule`

Input model:
```json
{
  "tasks": [
    { "title": "Design API", "estimatedHours": 5, "dueDate": "2025-10-25", "dependencies": [] }
  ]
}
```
Response:
```json
{ "recommendedOrder": ["Design API", "Implement Backend", "Build Frontend", "End-to-End Test"] }
```

## Algorithm
- Build a dependency graph.
- Perform topological sort.
- When multiple tasks are available, choose by:
  1) earliest dueDate (missing = latest),
  2) smaller estimatedHours,
  3) alphabetical title.
- Cycles or unknown dependencies fall back to best-effort prioritized ordering.
