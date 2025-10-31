## Pathlock Tasks - Full Stack (C# .NET 8 + React + TypeScript)

This project implements the Pathlock assignment with a minimal .NET 8 REST API and a React + TypeScript frontend. It includes polished styling and a Pathlock logo.

### Prerequisites
- .NET SDK 8.0+
- Node.js 18+

### Run Backend (API)
```
cd backend
dotnet restore
dotnet run
```
API will run on `http://localhost:5178` by default (see `appsettings.json`). Endpoints under `http://localhost:5178/api/tasks`.

### Run Frontend (React)
```
cd frontend
npm install
npm run dev
```
Frontend will run on `http://localhost:5173`. It is configured to call the API at `http://localhost:5178` by default (see `.env`).

### Build Frontend
```
cd frontend
npm run build
```

### Features
- Display tasks list
- Add task with description
- Toggle completion
- Delete task
- Optional filters: All / Active / Completed
- Clean, modern UI with Pathlock branding

### Structure
```
backend/   # .NET 8 minimal API
frontend/  # React + TypeScript app (Vite)
```


