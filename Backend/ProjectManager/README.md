# ProjectManager (Question 2 Backend)

.NET 8 Web API implementing a mini project manager with JWT auth and EF Core (SQLite).

## Run
```bash
cd Backend/ProjectManager
# first time only
dotnet tool install --global dotnet-ef || true
# migrations (skip if already created)
dotnet ef migrations add InitialCreate || true
dotnet ef database update
# run
dotnet run
```
Open Swagger at the URL printed by the app (e.g., `/swagger`).

## Auth
- POST `/api/auth/register`  → `{ token, username }`
- POST `/api/auth/login`     → `{ token, username }`

Use `Authorization: Bearer <token>` for all endpoints below.

## Projects
- GET  `/api/projects`
- POST `/api/projects` { title }
- GET  `/api/projects/{id}` (includes tasks)
- PUT  `/api/projects/{id}` { title }
- DELETE `/api/projects/{id}`

## Tasks (under a project)
- POST   `/api/projects/{projectId}/tasks` { title, description?, dueDate? }
- PUT    `/api/projects/{projectId}/tasks/{taskId}` { title, description?, dueDate?, isCompleted }
- DELETE `/api/projects/{projectId}/tasks/{taskId}`

## Tech
- ASP.NET Core 8, EF Core (SQLite), JWT (HMAC)
