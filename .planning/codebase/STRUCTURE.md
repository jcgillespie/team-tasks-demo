# STRUCTURE

## Repository Layout
- `client/`: React + Vite TypeScript application.
- `server/TeamTasks.Api/`: ASP.NET Core Web API project.
- `tests/TeamTasks.Api.Tests/`: backend unit tests.
- `docs/`: architecture and project notes.
- `scripts/`: developer helper scripts.
- `Prompts/`: prompt artifacts for guided setup flows.
- `TeamTasks.slnx`: solution entry for .NET projects.

## Frontend Structure (`client/src`)
- `main.tsx`: React bootstrap.
- `App.tsx`: root shell and route/page handoff.
- `pages/TasksPage.tsx`: feature page orchestrating data load and actions.
- `components/TaskForm.tsx`: task creation form.
- `components/TaskList.tsx`: task list presentation and toggle trigger.
- `api/tasksApi.ts`: HTTP client wrappers.
- `types/task.ts`: shared frontend type contracts.
- `test/setupTests.ts`: test environment setup for jest-dom.

## Backend Structure (`server/TeamTasks.Api`)
- `Program.cs`: service registration and middleware pipeline.
- `Controllers/TasksController.cs`: HTTP endpoint layer.
- `Services/ITaskService.cs`: service contract.
- `Services/TaskService.cs`: business operations implementation.
- `Data/AppDbContext.cs`: EF Core context and model configuration.
- `Data/DbInitializer.cs`: startup seed logic.
- `Models/TaskItem.cs`: persistence entity.
- `Contracts/CreateTaskRequest.cs` and `Contracts/TaskResponse.cs`: API DTOs.
- `appsettings*.json`: runtime configuration.

## Test Structure
- Backend test file currently concentrated in `tests/TeamTasks.Api.Tests/TaskServiceTests.cs`.
- Frontend tests currently concentrated in `client/src/App.test.tsx`.
- Project-level test dependencies declared in each project manifest (`*.csproj`, `client/package.json`).

## Organization Patterns
- Backend follows folder-by-layer organization (Controllers/Services/Data/Models/Contracts).
- Frontend follows light feature-oriented structure centered around the tasks domain.
- Naming uses singular task concept consistently across client and server (`TaskItem`, `TaskResponse`, `TasksPage`).

## Supporting Assets
- `README.md`: onboarding and command references.
- `docs/architecture.md`: concise architecture narrative.
- `scripts/dev.sh`: local workflow shortcut for starting both app processes.

## Generated/Build Outputs
- `bin/` and `obj/` directories exist under .NET projects.
- Frontend `dist/` is ignored by lint configuration (`globalIgnores(['dist'])`) in `client/eslint.config.js`.
