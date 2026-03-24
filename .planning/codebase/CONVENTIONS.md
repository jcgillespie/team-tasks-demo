# CONVENTIONS

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
