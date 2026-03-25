<!-- GSD:project-start source:PROJECT.md -->
## Project

**Team Tasks — IaC & CI/CD Milestone**

Team Tasks is an ASP.NET Core + React application that needs production-grade Infrastructure as Code and automated build/deployment pipelines. This milestone adds OpenTofu-managed Azure infrastructure (Dev + Production environments), GitHub Actions CI/CD with full quality and security gate automation, a rolling deployment strategy for production, and repeatable environment provisioning — enabling the team to ship reliably and safely from day one.

**Core Value:** Every code change must be validated by automated quality and security gates before it can be deployed, and every deployment must be repeatable, rollback-ready, and consistent across environments.

### Constraints

- **IaC**: OpenTofu — not Terraform or Bicep; maintain provider parity with Azure RM
- **CI/CD**: GitHub Actions only — no Azure DevOps, CircleCI, or other platforms
- **Environments**: Dev and Production only for this milestone
- **Release strategy**: Rolling — no Blue/Green or Canary for v1
- **State backend**: Azure Storage Account for OpenTofu remote state
<!-- GSD:project-end -->

<!-- GSD:stack-start source:codebase/STACK.md -->
## Technology Stack

## Languages and Runtimes
- Backend language: C# targeting `.NET 10` in `server/TeamTasks.Api/TeamTasks.Api.csproj`.
- Frontend language: TypeScript + JSX in `client/src/**/*.ts` and `client/src/**/*.tsx`.
- Runtime targets:
## Core Frameworks and Libraries
- API framework: ASP.NET Core Web API in `server/TeamTasks.Api/Program.cs`.
- Data access: EF Core + SQLite provider in `server/TeamTasks.Api/TeamTasks.Api.csproj`.
- API docs: OpenAPI + Swagger (`Microsoft.AspNetCore.OpenApi`, `Swashbuckle.AspNetCore`).
- Frontend UI: React 19 (`react`, `react-dom`) in `client/package.json`.
- Frontend build/dev: Vite 8 with React plugin in `client/vite.config.ts`.
## Testing and Quality Tooling
- Backend tests: xUnit + Microsoft.NET.Test.Sdk in `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj`.
- Backend test DB: EF Core InMemory package in `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj`.
- Frontend tests: Vitest + Testing Library + jsdom in `client/package.json` and `client/vite.config.ts`.
- Linting: ESLint flat config + TypeScript ESLint in `client/eslint.config.js`.
## Build and Dev Toolchain
- Solution root: `TeamTasks.slnx`.
- Backend CLI commands: `dotnet restore`, `dotnet run`, `dotnet test` from `README.md`.
- Frontend package manager: `pnpm` 10+ from `README.md` and `client/pnpm-lock.yaml`.
- Frontend scripts in `client/package.json`:
- Local startup helper: AppleScript-based launcher in `scripts/dev.sh`.
## Configuration Surface
- Backend app config: `server/TeamTasks.Api/appsettings.json` and `server/TeamTasks.Api/appsettings.Development.json`.
- Connection string key: `ConnectionStrings:DefaultConnection`.
- CORS origin list: `Cors:AllowedOrigins` consumed in `server/TeamTasks.Api/Program.cs`.
- Frontend API base URL: `VITE_API_BASE_URL` fallback in `client/src/api/tasksApi.ts`.
## Data and Persistence
- Primary datastore: SQLite file (`teamtasks.db`) via connection string in `server/TeamTasks.Api/appsettings.json`.
- EF model root: `server/TeamTasks.Api/Data/AppDbContext.cs`.
- Primary domain entity: `server/TeamTasks.Api/Models/TaskItem.cs`.
- Seed mechanism: `server/TeamTasks.Api/Data/DbInitializer.cs`.
<!-- GSD:stack-end -->

<!-- GSD:conventions-start source:CONVENTIONS.md -->
## Conventions

## Backend Code Conventions
- C# files use file-scoped namespaces (example: `namespace TeamTasks.Api.Services;`).
- Nullable reference types are enabled in `server/TeamTasks.Api/TeamTasks.Api.csproj`.
- Constructor injection is preferred (example: `TasksController(ITaskService taskService)`).
- Async-first service/controller methods return `Task<T>` and accept optional cancellation tokens.
- DTO mapping is explicit via `ToResponse` helper in `server/TeamTasks.Api/Services/TaskService.cs`.
## Frontend Code Conventions
- TypeScript with explicit exported types in `client/src/types/task.ts`.
- Functional React components and hooks only (`useState`, `useEffect`, `useCallback`).
- Async UI actions use `try/catch/finally` for loading/error state transitions.
- Client API calls are centralized in `client/src/api/tasksApi.ts` rather than inline fetches.
- Import style is relative-path based; no alias path mapping detected.
## Naming Conventions
- PascalCase for React components and C# types (`TaskForm`, `TaskList`, `TaskService`).
- camelCase for JS/TS variables and C# locals (`isLoading`, `allowedOrigins`).
- Boolean names use `is*` prefix in both client and server (`isCompleted`, `isSubmitting`).
- File names mirror primary type/component responsibility (`TaskForm.tsx`, `TaskService.cs`).
## Error Handling Patterns
- Frontend: user-friendly messages are set in component state (`Could not load tasks.`).
- API client: throws generic `Error` on non-OK responses in `parseJsonOrThrow`.
- Backend: not-found handled explicitly in controller (`return NotFound();`).
- Validation: request contract data annotations with `[ApiController]` model validation.
## Styling and UI Patterns
- CSS class-based styling from `client/src/App.css` and `client/src/index.css`.
- Page layout structured with semantic sections in `client/src/pages/TasksPage.tsx`.
- Empty state and status messages are rendered conditionally.
## Tooling and Quality Gates
- ESLint flat config for TS/React in `client/eslint.config.js`.
- React hooks lint rules are enabled via `eslint-plugin-react-hooks`.
- No backend analyzer ruleset or formatter config file is currently present.
## Documentation Conventions
- Primary contributor docs in `README.md` and `docs/architecture.md`.
- API manual probe file in `server/TeamTasks.Api/TeamTasks.Api.http`.
<!-- GSD:conventions-end -->

<!-- GSD:architecture-start source:ARCHITECTURE.md -->
## Architecture

## High-Level Pattern
- Architecture style is a small layered monorepo:
- Domain focus is a single aggregate-like entity (`TaskItem`).
## Entry Points
- Backend entry point: `server/TeamTasks.Api/Program.cs`.
- Frontend entry point: `client/src/main.tsx` -> `client/src/App.tsx` -> `client/src/pages/TasksPage.tsx`.
- Backend test entry point: `tests/TeamTasks.Api.Tests/TaskServiceTests.cs` (xUnit discovery).
- Frontend test entry point: `client/src/App.test.tsx` (Vitest).
## Request and Data Flow
- UI mounts `TasksPage`, which calls `getTasks()` on load in `client/src/pages/TasksPage.tsx`.
- API client functions in `client/src/api/tasksApi.ts` call REST endpoints.
- Controller actions in `server/TeamTasks.Api/Controllers/TasksController.cs` delegate to `ITaskService`.
- `TaskService` reads/writes through `AppDbContext` in `server/TeamTasks.Api/Services/TaskService.cs`.
- Responses are projected as `TaskResponse` records, returned to frontend, then stored in page state.
## Cross-Cutting Decisions
- Dependency injection used for service and DbContext wiring in `server/TeamTasks.Api/Program.cs`.
- CORS policy configured centrally and applied globally (`app.UseCors("ClientCors")`).
- Validation strategy combines `[ApiController]` auto-validation and data annotations on `CreateTaskRequest`.
- No explicit repository pattern; EF Core DbContext is used directly in service layer.
## Data Model Boundary
- Persistence model: `TaskItem` in `server/TeamTasks.Api/Models/TaskItem.cs`.
- API contract types live separately in `server/TeamTasks.Api/Contracts/`.
- Frontend contract mirrors server fields in `client/src/types/task.ts`.
## Environment and Startup Behavior
- Development-only OpenAPI + Swagger via environment guard in `server/TeamTasks.Api/Program.cs`.
- DB seed runs at startup using a scoped service block in `server/TeamTasks.Api/Program.cs`.
- Frontend defaults to local API URL if env var is missing in `client/src/api/tasksApi.ts`.
## Architectural Constraints
- Current architecture is optimized for simplicity and baseline demos, not scale.
- State management is local component state (`useState`/`useEffect`), no shared store.
- Single API resource and controller; horizontal module scaling patterns are not yet established.
<!-- GSD:architecture-end -->

<!-- GSD:workflow-start source:GSD defaults -->
## GSD Workflow Enforcement

Before using Edit, Write, or other file-changing tools, start work through a GSD command so planning artifacts and execution context stay in sync.

Use these entry points:
- `/gsd-quick` for small fixes, doc updates, and ad-hoc tasks
- `/gsd-debug` for investigation and bug fixing
- `/gsd-execute-phase` for planned phase work

Do not make direct repo edits outside a GSD workflow unless the user explicitly asks to bypass it.
<!-- GSD:workflow-end -->



<!-- GSD:profile-start -->
## Developer Profile

> Profile not yet configured. Run `/gsd-profile-user` to generate your developer profile.
> This section is managed by `generate-claude-profile` -- do not edit manually.
<!-- GSD:profile-end -->
