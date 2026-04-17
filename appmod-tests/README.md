# Team Tasks — Legacy behavioral test suite

Black-box tests for the **legacy** Team Tasks stack, derived from `.modernization/output/team-tasks-test-suite.md` and the detailed TC files under `.modernization/test-cases/`. Tests exercise the running HTTP API and the browser client (no in-repo server startup from the test projects).

## Prerequisites

- **.NET SDK** (10.x; matches `net10.0` in the API technology profile).
- **Node.js** and **pnpm** (for client-side tests).
- **Legacy app running**: API reachable and **Vite dev server** serving the client (Playwright opens the real UI).

## Environment variables

| Variable | Purpose | Default (this repo) |
|----------|---------|---------------------|
| `TEAM_TASKS_API_BASE_URL` | Scheme + host + port of the API **without** trailing slash (paths append `/api/tasks`, etc.) | `http://127.0.0.1:5276` |
| `TEAM_TASKS_CLIENT_BASE_URL` | Origin of the Vite app for Playwright | `http://127.0.0.1:5173` |
| `TEAM_TASKS_CORS_TEST_ORIGIN` | Origin sent on OPTIONS requests in TC-3.06 (must match server CORS allowlist) | `http://127.0.0.1:5173` |

Example for local development:

```bash
export TEAM_TASKS_API_BASE_URL=http://127.0.0.1:5276
export TEAM_TASKS_CLIENT_BASE_URL=http://127.0.0.1:5173
```

The client must call the API on a host/port the browser can reach; tests intercept `http://localhost:5276` and `http://127.0.0.1:5276` under `/api/tasks`.

## Installation

**API tests (C#):**

```bash
cd appmod-tests/legacy/api
dotnet restore
```

**Client tests (Vitest + Playwright):**

```bash
cd appmod-tests/legacy/client
pnpm install
pnpm exec playwright install chromium
```

## Running tests

**API only (xUnit):**

```bash
cd appmod-tests/legacy/api
dotnet test
```

**Client unit test (TC-6.01 — adapter error message):**

```bash
cd appmod-tests/legacy/client
pnpm run test:unit
```

**Browser E2E (Playwright — TC-2.05, TC-4.x, TC-6.x except TC-6.01 unit):**

With the **client dev server** already running:

```bash
cd appmod-tests/legacy/client
pnpm run test:e2e
```

**Full client package (unit + E2E):**

```bash
cd appmod-tests/legacy/client
pnpm test
```

**Run a single Playwright file:**

```bash
pnpm exec playwright test specs/tc-4-ui-ux.spec.ts
```

## Directory layout

| Path | Contents |
|------|----------|
| `legacy/api/` | xUnit tests against the live API (`TC-1`, `TC-2`, `TC-3`, `TC-5`) |
| `legacy/client/unit/` | Vitest tests for `getTasks` error shape (`TC-6.01`) |
| `legacy/client/specs/` | Playwright specs (`TC-2.05`, `TC-4`, `TC-6`) |

## Interpreting results (legacy mode)

- **Passing API/UI checks** — Observed behavior matches the spec documents for the exercised scenario.
- **Failing API test** — Treat as a **spec** mismatch or environment issue (per orchestration contract for legacy: the legacy system is the oracle; investigate data left by prior runs, CORS origin, or spec ambiguity).
- **Failing client test** — Same: confirm the dev server URL, API base URL in the client build, and that Playwright can open the app.

Infrastructure problems (wrong URL, connection refused, missing browser) are not “spec vs product” failures until connectivity is fixed.

## TC-1.04 — Seed idempotency (manual)

Automated HTTP tests cannot stop and restart the API process. The skipped xUnit case `TC104_SeedIdempotency_ManualOnly` documents this gap.

**Manual check:**

1. Note `GET {TEAM_TASKS_API_BASE_URL}/api/tasks` (e.g. task count or snapshot of `id` values).
2. Restart the API process using the **same** SQLite file as before (same working directory and `ConnectionStrings:DefaultConnection` as usual).
3. Call `GET` again. The task set must **not** gain three additional rows compared to the pre-restart response (seed runs only when the table was empty at startup).

## Traceability

- API: each `[Fact(DisplayName = "TC-x.xx — …")]` maps to the behavioral suite ID.
- Client: file and test titles include `TC-x.xx`.

## Spec sources

- `.modernization/output/team-tasks-specification.md`
- `.modernization/output/team-tasks-test-suite.md`
- `.modernization/test-cases/TC-*.md`
