# Frontend (React + TypeScript)

Single-page app for the Mini Project Manager.

## Run
```bash
cd frontend
npm install
npm start
```
Default: `http://localhost:3000`.

Pages:
- `/login`, `/register`
- `/dashboard` (projects)
- `/projects/:id` (tasks within a project)

Provide the JWT from login/register automatically via localStorage (configured in the app services).
