# Team Tasks Architecture

## Overview

This starter app is a monorepo with a small ASP.NET Core API and a React frontend.
It is intentionally simple so later worktree-based agent tasks can extend it safely.

## Structure

- `server/TeamTasks.Api`: ASP.NET Core Web API on .NET 10 with EF Core + SQLite
- `client`: React + Vite + TypeScript frontend
- `tests/TeamTasks.Api.Tests`: xUnit tests for backend validation and task logic
- `docs`: short design and architecture notes
- `scripts`: local helper scripts for common workflows

## Backend

- Entity model: `TaskItem`
- Persistence: `AppDbContext` using SQLite (`teamtasks.db`)
- API routes:
  - `GET /api/tasks`
  - `POST /api/tasks`
  - `PATCH /api/tasks/{id}/toggle`
- Validation: request data annotations through `[ApiController]`
- Seed: `DbInitializer` inserts sample tasks on first run
- CORS: configured from `appsettings.json` for Vite dev server

## Frontend

- Feature-oriented structure:
  - `src/api/tasksApi.ts`
  - `src/types/task.ts`
  - `src/components/TaskForm.tsx`
  - `src/components/TaskList.tsx`
  - `src/pages/TasksPage.tsx`
- State and side effects are local to `TasksPage` (`useState` + `useEffect`)
- Includes loading and error handling for fetch and update flows

## Testing

- Backend: xUnit + EF Core InMemory provider for service logic tests
- Frontend: Vitest + React Testing Library for baseline UI behavior

## Delivery Automation

- Infrastructure is defined in `infra/opentofu/` with separate `dev`, `staging`, and `prod` roots.
- `infra-plan.yml`, `infra-apply.yml`, and `drift-detection.yml` provide plan/apply safeguards and drift visibility.
- `release.yml` builds artifacts once and promotes the same bundle through `dev`, `staging`, and `prod`.
- Production promotion uses staged deployment patterns and rollback support through slot swap or manifest redeploy.
