# ARCHITECTURE

## High-Level Pattern
- Architecture style is a small layered monorepo:
  - Presentation/UI layer: React app in `client/src/`.
  - API layer: ASP.NET controllers in `server/TeamTasks.Api/Controllers/`.
  - Application/service layer: task operations in `server/TeamTasks.Api/Services/`.
  - Persistence layer: EF Core context/model in `server/TeamTasks.Api/Data/` and `server/TeamTasks.Api/Models/`.
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
