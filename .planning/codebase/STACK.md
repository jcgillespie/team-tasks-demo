# STACK

## Languages and Runtimes
- Backend language: C# targeting `.NET 10` in `server/TeamTasks.Api/TeamTasks.Api.csproj`.
- Frontend language: TypeScript + JSX in `client/src/**/*.ts` and `client/src/**/*.tsx`.
- Runtime targets:
  - API: ASP.NET Core (`net10.0`) in `server/TeamTasks.Api/TeamTasks.Api.csproj`.
  - Web client: Browser runtime via Vite in `client/package.json`.
  - Tests: .NET test host and Vitest Node/jsdom runtime.

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
  - `dev`, `build`, `test`, `test:watch`, `lint`, `preview`.
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
