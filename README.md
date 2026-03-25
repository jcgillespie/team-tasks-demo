# Team Tasks (Base App)

Clean starter app for future agentic workflow demos using Git worktrees.

## Folder Structure

```text
main/
	app/
	server/
		TeamTasks.Api/
	client/
	tests/
		TeamTasks.Api.Tests/
	docs/
	scripts/
	Prompts/
	TeamTasks.slnx
```

## Prerequisites

- .NET SDK 10
- Node.js 22+
- `pnpm` 10+

Install `pnpm` globally if needed:

```bash
corepack enable pnpm
```

## Local Setup

```bash
dotnet restore TeamTasks.slnx
cd client
pnpm install
```

## Run Backend

```bash
cd server/TeamTasks.Api
dotnet run
```

API URLs:

- `http://localhost:5276`
- OpenAPI JSON (dev): `http://localhost:5276/openapi/v1.json`
- Swagger UI (dev): `http://localhost:5276/swagger`

## Run Frontend

```bash
cd client
pnpm dev
```

Frontend URL:

- `http://localhost:5173`

## Run Tests

Backend tests:

```bash
dotnet test TeamTasks.slnx
```

Frontend tests:

```bash
cd client
pnpm test
```

## Optional Helper Script

Start backend and frontend in separate Terminal windows:

```bash
./scripts/dev.sh
```

## Architecture Summary

- Backend: ASP.NET Core Web API (.NET 10), EF Core, SQLite, Swagger/OpenAPI.
- Frontend: React + Vite + TypeScript with a small feature-based structure.
- Data model: `Task` with `id`, `title`, `description`, `isCompleted`, `createdAt`.
- API endpoints:
	- `GET /api/tasks`
	- `POST /api/tasks`
	- `PATCH /api/tasks/{id}/toggle`
- Seed data: sample tasks are inserted on first run.
- Tests:
	- xUnit backend tests for validation, retrieval, and toggle behavior.
	- Vitest + RTL frontend tests for list rendering, creation, toggle, and loading/error states.

More detail: `docs/architecture.md`
