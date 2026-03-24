# CONCERNS

## Summary
The codebase is intentionally small and clean, but several concerns could become blockers as scope grows.

## Technical Debt and Fragility
- Data initialization uses `EnsureCreated` in `server/TeamTasks.Api/Data/DbInitializer.cs`.
  - Concern: bypasses migration workflow and can limit schema evolution practices.
- `CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created)` in `server/TeamTasks.Api/Controllers/TasksController.cs`.
  - Concern: references an action that does not accept `id`; location header semantics may be misleading.
- Frontend API error handling in `client/src/api/tasksApi.ts` only reports status code.
  - Concern: limited diagnostics and no typed error envelope.
- Startup script `scripts/dev.sh` is macOS Terminal/AppleScript specific.
  - Concern: developer experience portability gap on Linux/Windows.

## Security and Operational Concerns
- CORS defaults are permissive for methods/headers in `server/TeamTasks.Api/Program.cs`.
  - Concern: acceptable for local dev, but production hardening policy is not represented yet.
- No authentication/authorization in API pipeline.
  - Concern: all task operations are anonymous and globally accessible.
- No explicit secret management or environment separation strategy beyond appsettings files.

## Performance and Scalability Concerns
- No pagination/filtering endpoints for task listing (`GET /api/tasks` returns all).
- EF query and app state are fine at demo scale, but unbounded lists can degrade UX and API latency.
- No caching strategy or concurrency controls are present.

## Testing and Reliability Concerns
- No HTTP-level integration tests for controller routing and model-state behavior.
- No E2E tests for browser + API in a real environment.
- No CI configuration discovered to enforce lint/test/build checks on every change.

## Documentation and Process Concerns
- High-level docs exist, but there is no ADR history for architectural decisions.
- No contribution guide or coding standards document beyond implied patterns.

## Suggested Mitigations (Near-Term)
- Introduce EF migrations and replace `EnsureCreated` for production-like workflows.
- Add `GET /api/tasks/{id}` and align `CreatedAtAction` with a routable resource endpoint.
- Standardize API error response shape and map client-side errors consistently.
- Add a cross-platform dev bootstrap path (or document OS-specific script constraints).
- Add CI pipeline executing backend and frontend tests plus lint.
