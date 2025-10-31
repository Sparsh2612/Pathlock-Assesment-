## Pathlock Projects (Question 2) — .NET 8 + React + TypeScript

Advanced assignment implementing JWT auth, projects, and tasks with a clean UI.

### Prerequisites
- .NET SDK 8.0+
- Node.js 18+

### Run Backend
```
cd backend
dotnet restore
dotnet run
```
API will listen on `http://localhost:5280` with Swagger at `/swagger`.

### Run Frontend
```
cd frontend
npm install
npm run dev
```
App runs on `http://localhost:5290` and targets API at `http://localhost:5280`.

### Endpoints
- Auth: `POST /api/auth/register`, `POST /api/auth/login`
- Projects: `GET /api/projects`, `POST /api/projects`, `GET /api/projects/{id}`, `DELETE /api/projects/{id}`
- Tasks: `POST /api/projects/{projectId}/tasks`, `PUT /api/tasks/{taskId}`, `DELETE /api/tasks/{taskId}`

### Notes
- Data stored in-memory; scoped per authenticated user.
- DTO validation via DataAnnotations.
- Clean, modern dark UI with Pathlock branding.


