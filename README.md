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
cd main
dotnet restore TeamTasks.slnx
cd client
pnpm install
```

## Run Backend

```bash
cd main/server/TeamTasks.Api
dotnet run
```

API URLs:

- `http://localhost:5276`
- OpenAPI JSON (dev): `http://localhost:5276/openapi/v1.json`
- Swagger UI (dev): `http://localhost:5276/swagger`

## Run Frontend

```bash
cd main/client
pnpm dev
```

Frontend URL:

- `http://localhost:5173`

## Run Tests

Backend tests:

```bash
cd main
dotnet test TeamTasks.slnx
```

Frontend tests:

```bash
cd main/client
pnpm test
```

## CI Expectations

Pull requests are expected to pass the quality gate workflow in `.github/workflows/ci.yml`.

Required checks include:

- `pnpm lint`, `pnpm build`, and `pnpm test` in `client/`
- `dotnet build TeamTasks.slnx` and `dotnet test TeamTasks.slnx`
- secret scanning and workflow summary publication

If CI fails, fix the failing step and re-run checks before requesting review.

## Delivery Validation Notes

Latest local validation run:

- `dotnet build TeamTasks.slnx` passed
- `dotnet test TeamTasks.slnx` passed
- `cd client && pnpm lint && pnpm build && pnpm test` passed
- `cd tests/delivery && pnpm test` passed
- OpenTofu command execution depends on local `tofu` installation and authenticated Azure context

## Optional Helper Script

Start backend and frontend in separate Terminal windows:

```bash
cd main
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
