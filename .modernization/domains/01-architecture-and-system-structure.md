# Domain 1 — Architecture & System Structure

**Repository:** Team Tasks demo (`appmod`).  
**Sources:** Code and repo docs only. **Technology-specific product names** are avoided; see Technology Profile for stack details.

---

## 1. System topology

| Layer | Role | Evidence |
|--------|------|----------|
| **Browser** | Renders the single-page client and issues HTTP calls to a configurable API base URL | `client/src/api/tasksApi.ts:3-35` |
| **HTTP API host** | Single process exposing JSON endpoints under `/api/tasks` | `server/TeamTasks.Api/Program.cs:48-51`, `server/TeamTasks.Api/Controllers/TasksController.cs:7-8` |
| **Persistence** | Embedded file-backed relational store beside the API working directory (`teamtasks.db` by default) | `server/TeamTasks.Api/appsettings.json:2-4`, `server/TeamTasks.Api/Program.cs:13-15` |
| **Automated tests** | Test assembly exercises service-layer logic with an in-memory store; client tests exercise UI with mocked remote API | `tests/TeamTasks.Api.Tests/TaskServiceTests.cs:84-91` |

**Communication pattern:** Synchronous request/response over HTTP between client and API; no queues, webhooks, or background workers in application code.

`[ASSUMPTION]` Production deployment (load balancers, TLS termination, separate data host) is not described in-repo.

---

## 2. Module decomposition

### 2.1 HTTP API (`server/TeamTasks.Api/`)

| Area | Responsibility |
|------|----------------|
| **Entry / pipeline** | Build host, register services, optional dev-only API description endpoints, seed database, register middleware, map routes | `Program.cs:5-53` |
| **HTTP surface** | One controller: list tasks, create task, toggle completion | `Controllers/TasksController.cs:7-44` |
| **Application services** | `ITaskService` / `TaskService`: query ordering, create normalization, toggle by id | `Services/TaskService.cs:8-54` |
| **Persistence** | `AppDbContext`: model mapping; `DbInitializer`: create schema if missing, seed sample rows once | `Data/AppDbContext.cs`, `Data/DbInitializer.cs:8-42` |
| **Contracts** | Request/response shapes for HTTP JSON | `Contracts/` |
| **Domain model** | `TaskItem` entity | `Models/TaskItem.cs` |

### 2.2 Client (`client/`)

| Area | Responsibility |
|------|----------------|
| **Bootstrap** | Mount root component | `client/src/main.tsx`, `client/src/App.tsx` |
| **Feature UI** | Task page, form, list | `client/src/pages/TasksPage.tsx`, `client/src/components/` |
| **Remote API adapter** | HTTP client for `/api/tasks` operations | `client/src/api/tasksApi.ts` |

### 2.3 Tests (`tests/TeamTasks.Api.Tests/`)

- References the API project (`tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj:28-30`) but **instantiates `AppDbContext` with an in-memory provider** and `TaskService` directly (`TaskServiceTests.cs:84-91`) — not the full HTTP pipeline.

### 2.4 Solution layout

- Solution file lists API + API tests only (`TeamTasks.slnx`); the client is a sibling folder, not a solution project.

---

## 3. Dependency graph (runtime, logical)

```
Browser client
    │ HTTP (JSON)
    ▼
TasksController ──► ITaskService ──► TaskService ──► AppDbContext ──► embedded file DB
                         │                    │
                         └────────────────────┴──► TaskItem → response mapping
DbInitializer ───────────────────────────────► AppDbContext (startup scope only)
```

---

## 4. Configuration model (defaults → overrides)

| Source | Content |
|--------|---------|
| Base JSON | Connection string `DefaultConnection`, CORS `Cors:AllowedOrigins`, logging, `AllowedHosts` | `appsettings.json` |
| Environment-specific JSON | Logging overrides only | `appsettings.Development.json` |
| Code fallbacks | If `DefaultConnection` missing → `Data Source=teamtasks.db`; if `Cors:AllowedOrigins` missing → `["http://localhost:5173"]` | `Program.cs:13-21` |
| Launch profile | `http` profile: `http://localhost:5276`; `https` profile adds TLS port | `Properties/launchSettings.json:4-20` |
| Client API base | Build-time env override, else `http://localhost:5276` | `client/src/api/tasksApi.ts:3` |

`[ASSUMPTION]` Host merges JSON, environment variables, and command-line configuration in the usual precedence order for this stack.

---

## 5. Initialization sequence (HTTP API host)

Order per `Program.cs`:

1. Build configuration.
2. Register controllers; OpenAPI / interactive API doc services.
3. Register scoped persistence context with resolved connection string.
4. Register scoped `ITaskService`.
5. Read CORS origins (with fallback), register named CORS policy `ClientCors`.
6. Build web application.
7. **If** Development: map OpenAPI + Swagger UI.
8. Create scope, resolve `AppDbContext`, run `DbInitializer.SeedAsync`.
9. HTTPS redirection → CORS → authorization middleware → map controllers → run.

**Seed:** `EnsureCreatedAsync` then skip if any task row exists; else insert three sample tasks (`DbInitializer.cs:8-42`).

**Client bootstrap:** `main.tsx` renders `App` → `TasksPage` (`App.tsx`, `main.tsx`).

---

## 6. Deployment (repo evidence)

| Topic | Evidence |
|--------|----------|
| Run commands | `README.md` — `dotnet restore`, `pnpm install`, `dotnet run`, `pnpm dev` |
| Ports | API `http://localhost:5276`, client `http://localhost:5173` — matches defaults |
| `scripts/dev.sh` | Starts API, waits for GET `/api/tasks`, then client dev server; uses macOS `osascript` for Terminal windows |
| Containers / CI | No `Dockerfile` or `.github` workflows in repo (inferred from filesystem) |

---

## Traceability

| Topic | File:line |
|--------|-----------|
| Host boot & middleware | `server/TeamTasks.Api/Program.cs:5-53` |
| HTTP routes | `server/TeamTasks.Api/Controllers/TasksController.cs` |
| Service → persistence | `server/TeamTasks.Api/Services/TaskService.cs` |
| SPA → API | `client/src/api/tasksApi.ts` |
