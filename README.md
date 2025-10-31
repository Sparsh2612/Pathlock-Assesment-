# Pathlock Assignment

This repository contains a full-stack Mini Project Manager (Question 2) and a Smart Scheduler API extension (Question 2 Extension).

## Repository Structure

```
Backend/
  ProjectManager/                 # Question 2: .NET 8 API with JWT + EF Core (SQLite)
frontend/                         # Question 2: React + TypeScript SPA
Question 2 Extension/
  Backend/
    SchedulerApi/                 # Smart Scheduler API (.NET 8 minimal API)
  frontend/                       # Tiny static UI to call Scheduler API
```

## Prerequisites
- .NET SDK 8.x
- Node.js 18+
- npm 9+

---

## Question 2: Mini Project Manager

### Backend (ProjectManager)
Features:
- Auth: register/login with JWT
- User-scoped Projects (CRUD)
- Tasks under projects (CRUD, complete)
- SQLite via EF Core

Run:
```bash
cd "Backend/ProjectManager"
# one-time: install EF tools
dotnet tool install --global dotnet-ef || true
# create DB
dotnet ef migrations add InitialCreate || true
dotnet ef database update
# run API
dotnet run
```
The API prints its listening URLs; open Swagger at `http://localhost:xxxx/swagger`.

Key endpoints:
- POST `/api/auth/register`  { username, password }
- POST `/api/auth/login`     -> JWT
- GET/POST `/api/projects`
- GET/PUT/DELETE `/api/projects/{id}`
- POST `/api/projects/{projectId}/tasks`
- PUT/DELETE `/api/projects/{projectId}/tasks/{taskId}`

### Frontend (React + TS)
Run:
```bash
cd "frontend"
npm install
npm start
```
Default: `http://localhost:3000`.
If the backend runs on a different port, adjust API base URLs in `src/api/*.ts`.

---

## Question 2 Extension: Smart Scheduler API

### Backend (SchedulerApi)
- Endpoint: `POST /api/v1/projects/{projectId}/schedule`
- Input: list of tasks with `title`, `estimatedHours`, `dueDate?`, `dependencies[]`
- Output: `{ "recommendedOrder": string[] }`
- Logic: dependency-aware topological sort; ties by earliest due date, then lower estimated hours, then title.

Run:
```bash
cd "Question 2 Extension/Backend/SchedulerApi"
# ensure Swagger package present
# dotnet add package Swashbuckle.AspNetCore
# run on a fixed port for convenience
dotnet run --urls http://localhost:5000
```
Open Swagger: `http://localhost:5000/swagger` and test with the sample body from the statement.

### Frontend (Static demo UI)
```bash
cd "Question 2 Extension/frontend"
# Option A: open the file directly in a browser
open index.html
# Option B: serve locally (Python)
python3 -m http.server 8080
# then open http://localhost:8080
```
Set Base URL to your Scheduler API (e.g., `http://localhost:5000`) and click "Recommend Order".

---

## GitHub
Initialize and push to your repo `Pathlock-Assesment-`:
```bash
git init
git add .
git commit -m "Pathlock assignment: Mini Project Manager + Scheduler Extension"
git branch -M main
git remote add origin https://github.com/Sparsh2612/Pathlock-Assesment-.git
git push -u origin main
```

---

## Notes
- Replace JWT key in production and secure CORS appropriately.
- The Scheduler API is stateless and independent; it can be deployed separately (e.g., Render/Heroku/Fly.io).
