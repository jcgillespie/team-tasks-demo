# Discovery Report — Team Tasks Demo (`appmod`)

**Generated:** 2026-04-16  
**Scope:** Full repository (no module boundary specified).

## 1. File tree (catalog)

| Area | Pattern / notes |
|------|-----------------|
| Root | `TeamTasks.slnx`, `README.md`, `DemoSteps.md`, `AGENTS.md`, `scripts/dev.sh` |
| Server | `server/TeamTasks.Api/` — single web API project |
| Tests | `tests/TeamTasks.Api.Tests/` — xUnit project referencing API |
| Client | `client/` — Vite + React + TypeScript SPA |
| Docs | `docs/architecture.md`, `Prompts/001-BaseApp.md` |

**Approximate file count:** ~46 tracked source/config files (excluding `node_modules`, `bin`, `obj`).

## 2. Technology stack (high level)

| Layer | Evidence |
|-------|----------|
| Server | .NET SDK Web project, target `net10.0` (`server/TeamTasks.Api/TeamTasks.Api.csproj:4`) |
| Persistence | File-backed relational store via embedded provider; ORM maps `TaskItem` (`server/TeamTasks.Api/Data/AppDbContext.cs`) |
| Client | Node package manager workspace; Vite bundler; React 19 (`client/package.json`) |
| Tests | xUnit + EF in-memory provider (API); Vitest + Testing Library (client) |

Detailed versions and package inventory belong in **Domain 9 — Technology Profile** (from `.csproj`, `pnpm-lock.yaml`).

## 3. Entry points

| Entry | Location |
|-------|----------|
| HTTP API host | `server/TeamTasks.Api/Program.cs` — builds web app, registers services, maps controllers, runs |
| SPA bootstrap | `client/index.html` → `client/src/main.tsx` → `client/src/App.tsx` |
| Dev script | `scripts/dev.sh` — orchestrates local dev (see script for ports/commands) |

## 4. Entities and major components

| Category | Count / items |
|----------|----------------|
| Domain model | **1** entity: `TaskItem` (`server/TeamTasks.Api/Models/TaskItem.cs`) |
| API contracts | `CreateTaskRequest`, `TaskResponse` (`server/TeamTasks.Api/Contracts/`) |
| Controllers | **1**: `TasksController` — route prefix `api/tasks` (`server/TeamTasks.Api/Controllers/TasksController.cs:8`) |
| Services | `ITaskService` / `TaskService` (`server/TeamTasks.Api/Services/`) |
| DbContext | `AppDbContext` with `DbSet<TaskItem>` (`server/TeamTasks.Api/Data/AppDbContext.cs`) |
| Migrations | **None** — schema created via `EnsureCreatedAsync` at startup (`server/TeamTasks.Api/Data/DbInitializer.cs:10`) |

## 5. External dependencies (runtime integrations)

| Kind | Usage |
|------|--------|
| HTTP | Client calls configurable API base URL (`client/src/api/tasksApi.ts:3`) — default `http://localhost:5276` |
| CORS | Server allows configured origins (`server/TeamTasks.Api/Program.cs:20-30`, `appsettings.json`) |

No queues, external auth providers, or third-party HTTP APIs in application code (beyond browser `fetch` to same-origin-configured API).

## 6. Existing tests — catalog

### API (`tests/TeamTasks.Api.Tests/`)

| Framework | Count | Files |
|-----------|-------|-------|
| xUnit | 3 tests | `TaskServiceTests.cs` |

**Tests:**

1. `CreateTaskRequest_Title_IsRequired` — data annotation validation on empty title.
2. `GetTasksAsync_ReturnsTasksOrderedByCreatedAtDescending` — ordering newest first.
3. `ToggleTaskAsync_FlipsCompletionState` — toggle twice restores original completion flag.

### Client (`client/src/App.test.tsx`)

| Framework | Count |
|-----------|-------|
| Vitest + Testing Library | 4 tests |

**Tests:** loading → list; create task flow; toggle completion (mocked API); load error message.

### Gaps (static)

- No integration tests for `TasksController` HTTP layer (validation pipeline, status codes, `CreatedAtAction` location header).
- No tests for not-found toggle path, seed data behavior, or CORS.

## 7. Legacy test execution

**Status:** Executed with user consent implied by “run the orchestrator” (full suite).

| Suite | Command | Result |
|-------|---------|--------|
| API | `dotnet test` (repo root) | **Passed:** 3/3 |
| Client | `pnpm install && pnpm test` in `client/` | **Passed:** 4/4 |

**Requirements used:** default — no extra env vars. In-memory DB for API tests; client tests mock API module.

**Classification:** Passing tests → behaviors may be tagged `[CONFIRMED BY TEST: …]` in downstream docs.

## 8. Code paths (major branches)

### `TaskService` (`server/TeamTasks.Api/Services/TaskService.cs`)

- **GetTasksAsync:** query all tasks, order by `CreatedAt` descending, map to response DTOs.
- **CreateTaskAsync:** trim title; normalize description (null if whitespace-only); set `IsCompleted = false`, `CreatedAt = UtcNow`; persist.
- **ToggleTaskAsync:** load by id — **branch** null → return null; else flip `IsCompleted`, save, return DTO.

### `TasksController` (`server/TeamTasks.Api/Controllers/TasksController.cs`)

- GET `api/tasks` → 200 + collection.
- POST `api/tasks` → 201 + body (implicit validation via API controller conventions for `CreateTaskRequest`).
- PATCH `api/tasks/{id}/toggle` → 404 if service returns null; else 200.

### `TasksPage` / UI (`client/src/pages/TasksPage.tsx`, `TaskForm.tsx`, `TaskList.tsx`)

- Load: loading → success (list) or catch → error message; while loading/error, list hidden.
- Create: prepend returned task to state (optimistic ordering: new task first in UI regardless of `createdAt` vs server list order — **[NEEDS CLARIFICATION]** if strict consistency with API sort is required).
- Toggle: replace task in state on success; catch → set error string.

### Startup (`Program.cs`)

- Development: OpenAPI + Swagger UI.
- Scoped DB + seed: `DbInitializer.SeedAsync` after `EnsureCreatedAsync`.

## 9. Notes for downstream domains

- **UI domain applies** — React SPA present.
- **Security domain** — no authentication/authorization in code; HTTPS redirection + CORS only (`Program.cs:48-51`).
- **Repository size:** &lt; 500 files — full-repo analysis appropriate.

---

*Traceability: facts above traced to files as listed; test counts from test files and runner output.*
