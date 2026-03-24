# INTEGRATIONS

## External Interfaces
- Browser client calls backend HTTP API through `client/src/api/tasksApi.ts`.
- API base URL is configurable via `VITE_API_BASE_URL`; default is `http://localhost:5276`.
- OpenAPI and Swagger UI are exposed in development from `server/TeamTasks.Api/Program.cs`.

## API Endpoints (Current)
- `GET /api/tasks` for list retrieval in `server/TeamTasks.Api/Controllers/TasksController.cs`.
- `POST /api/tasks` for task creation in `server/TeamTasks.Api/Controllers/TasksController.cs`.
- `PATCH /api/tasks/{id}/toggle` for status toggle in `server/TeamTasks.Api/Controllers/TasksController.cs`.
- Frontend endpoint usage is centralized in `client/src/api/tasksApi.ts`.

## Data Store Integration
- Backend integrates with SQLite via EF Core provider in `server/TeamTasks.Api/TeamTasks.Api.csproj`.
- DB context abstraction is `AppDbContext` in `server/TeamTasks.Api/Data/AppDbContext.cs`.
- Database initialization/seed logic runs at app startup via `DbInitializer.SeedAsync` in `server/TeamTasks.Api/Program.cs`.

## Dependency Injection Integrations
- Service abstraction: `ITaskService` in `server/TeamTasks.Api/Services/ITaskService.cs`.
- Concrete registration: `TaskService` scoped binding in `server/TeamTasks.Api/Program.cs`.
- Controller depends on service contract only (`TasksController(ITaskService taskService)`).

## CORS and Cross-Origin Integration
- CORS policy name: `ClientCors` in `server/TeamTasks.Api/Program.cs`.
- Allowed origins come from config key `Cors:AllowedOrigins` in `server/TeamTasks.Api/appsettings.json`.
- Default local frontend origin fallback: `http://localhost:5173`.

## Test-Time Integrations
- Backend tests replace SQLite with EF InMemory in `tests/TeamTasks.Api.Tests/TaskServiceTests.cs`.
- Frontend tests mock API module (`vi.mock('./api/tasksApi')`) in `client/src/App.test.tsx`.

## Not Present Yet
- No third-party auth provider integration (OAuth/OIDC absent).
- No external message bus, queue, or webhook integration.
- No cloud service SDKs (Azure/AWS/GCP) in dependency manifests.
- No metrics/telemetry backend integration (App Insights, OpenTelemetry exporter, etc.).

## Operational Interfaces
- Manual API testing file: `server/TeamTasks.Api/TeamTasks.Api.http`.
- Local dual-process launcher: `scripts/dev.sh` opens backend and frontend in separate Terminal sessions.
