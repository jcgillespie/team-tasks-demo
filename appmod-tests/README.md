# Team Tasks — Behavioral tests (legacy)

Executable black-box tests derived from `.modernization/output/team-tasks-test-suite.md` and `.modernization/test-cases/TC-*.md`. They do not reference implementation source for expectations—only the published spec documents.

## Layout

| Directory | Role |
|-----------|------|
| `api/` | xUnit + `HttpClient` — TC-1, TC-2 (API), TC-3, TC-5 |
| `client/` | Vitest + Testing Library — TC-2.05, TC-4, TC-6 (imports `../../client` UI modules) |

## Prerequisites

- **.NET SDK** compatible with `net10.0` (see `.modernization/output/team-tasks-technology-profile.md`).
- **Node.js** and **pnpm** for `client/` tests.
- **Running legacy API** for `api/` tests: start the Team Tasks HTTP API and point tests at it with the base URL (no trailing slash required).

Example (from repo root, after building the API project):

```bash
cd server/TeamTasks.Api
dotnet run --launch-profile http
```

Default profile listens on `http://localhost:5276`.

## Environment variables

| Variable | Used by | Purpose |
|----------|---------|---------|
| `TEAM_TASKS_API_BASE_URL` | `api/` (required) | Root URL of the legacy API, e.g. `http://localhost:5276`. |
| `TEAM_TASKS_CORS_ALLOWED_ORIGIN` | `api/` (optional) | Origin sent on OPTIONS preflight for **TC-3.06**; defaults to `http://localhost:5173` (matches the system specification default). |
| `TEAM_TASKS_API_BASE_URL` | `client/` Vitest (optional) | Injected as `import.meta.env.VITE_API_BASE_URL` for modules under test; defaults to `http://localhost:5276` when unset. |

All API connection details are taken from environment variables—nothing is hardcoded as a permanent production URL.

## Run — API (HTTP) suite

From repo root:

```bash
export TEAM_TASKS_API_BASE_URL=http://localhost:5276
dotnet test appmod-tests/api/TeamTasks.Behavioral.Api.Tests.csproj
```

Or from `appmod-tests/api/`:

```bash
export TEAM_TASKS_API_BASE_URL=http://localhost:5276
dotnet test
```

**TC-1.04** (seed idempotency across two process starts) is **skipped** in this project: automating it requires a controlled database file and two application lifetimes. Perform it manually if needed (see spec TC-1.04).

## Run — Client (browser) suite

Install once:

```bash
cd appmod-tests/client
pnpm install
```

Run:

```bash
cd appmod-tests/client
pnpm test
```

These tests mock `fetch` and render components from `client/`; they do not require the API to be running unless you change them to hit a live server.

## Traceability

Each test file or case is named for its **spec test ID** (e.g. `TC-3.01`, `TC-4.05`). The mapping is 1:1 with `team-tasks-test-suite.md` where that document lists IDs.

## Spec sources

- `.modernization/output/team-tasks-specification.md`
- `.modernization/output/team-tasks-test-suite.md`
- `.modernization/test-cases/TC-1-data-model.md` … `TC-6-error-handling.md`
- `.modernization/output/team-tasks-review-summary.md` (context only)
