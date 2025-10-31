## Question 2 Extension — Smart Scheduler API

Standalone project that does not modify `question2/`. It exposes a scheduler endpoint and a small UI to try it.

### Backend (API)
```
cd backend
dotnet restore
dotnet run
```
API: `http://localhost:5380` → Swagger at `/swagger`.

Endpoint:
- `POST /api/v1/projects/{projectId}/schedule`
  - Body: `{ tasks: [{ title, estimatedHours, dueDate, dependencies: [] }] }`
  - Returns: `{ recommendedOrder: string[] }`

### Frontend (Demo UI)
```
cd frontend
npm install
npm run dev
```
App: `http://localhost:5390`

### Notes
- Algorithm: Topological sort honoring dependencies; ties resolved by earlier due date then smaller estimated hours.
- CORS enabled.


