# Team Tasks — System Specification

**Document type:** Technology-agnostic behavioral specification.  
**Sources:** `server/TeamTasks.Api/**/*.cs`, `client/src/**/*.{ts,tsx}`, `tests/**/*`, configuration JSON.  
**Evidence tags:** `[CONFIRMED BY TEST: name]` from passing automated tests; `[inferred from code]`; `[ASSUMPTION]`; `[NEEDS CLARIFICATION]`.

---

## 1. Purpose

The system lets users **list tasks**, **create tasks** with a title and optional description, and **toggle completion** on each task. A browser-based client talks to a single HTTP JSON API backed by a persistent relational store in a local file.

---

## 2. Actors

| Actor | Capabilities |
|-------|----------------|
| **Human user** | Uses the browser client to view and manage tasks. |
| **HTTP client (any)** | May call the public API without authentication. |

`[inferred from code]` — no identity or roles are enforced.

---

## 3. Persistent data — task entity

**Logical entity:** Task.

| Field | Meaning | Constraints |
|-------|---------|-------------|
| Identifier | Surrogate key | Positive integer; unique |
| Title | Short label | Required in store; max **120** characters |
| Description | Optional detail | Max **1000** characters when present; may be absent |
| Completion flag | Done vs not done | Boolean |
| Created instant | When the task was created | Required; new tasks use current UTC instant at creation |

**Table name in store:** `Tasks`.  
**Sources:** `Models/TaskItem.cs`, `Data/AppDbContext.cs:14-21`.

---

## 4. Startup and initialization

1. The API host loads configuration from default sources (JSON files, environment variables, command line).  
2. **Connection string** for the default store: read from `ConnectionStrings:DefaultConnection`; if absent, use `Data Source=teamtasks.db` (relative file path).  
3. **Cross-origin browser policy:** Allowed browser origins read from `Cors:AllowedOrigins`; if absent, default to a single development origin for the common local client port (`http://localhost:5173`).  
4. Before serving traffic, the host ensures the database exists (`EnsureCreated` semantics).  
5. **Seed:** If the task table has **no** rows, insert exactly **three** predefined tasks with fixed English titles and descriptions, with `CreatedAt` set to UTC “now” minus 30, 20, and 10 minutes respectively, and completion flags `true`, `false`, `false`. If **any** row exists, seed is skipped.

**Sources:** `Program.cs:11-16`, `Program.cs:20-21`, `Program.cs:42-46`, `Data/DbInitializer.cs:8-42`.

---

## 5. Business rules

### BR-1 — List tasks

- **Trigger:** Client requests the task collection.  
- **Action:** Return all tasks as records with fields: identifier, title, description, completion flag, created instant.  
- **Ordering:** Sort by **created instant descending** (newest first). **`[CONFIRMED BY TEST: GetTasksAsync_ReturnsTasksOrderedByCreatedAtDescending]`**  
- **Read optimization:** List query does not track entities for change detection after load.  
**Source:** `Services/TaskService.cs:10-18`.

### BR-2 — Create task

- **Trigger:** Client submits a create request with JSON body.  
- **Validation (request):** Title required; length between **1** and **120** inclusive. Description optional; if present as a string, max length **1000**. **`[CONFIRMED BY TEST: CreateTaskRequest_Title_IsRequired]`** (validation helper, not full HTTP stack).  
- **Service behavior:**  
  - Trim leading/trailing whitespace from title before persistence.  
  - If description is null/empty/whitespace-only after considering input, store **no** description (absent). Otherwise trim and store.  
  - Set completion flag to **not completed**.  
  - Set created instant to **current UTC** at save time.  
- **Outcome:** Success returns the new task record including generated identifier.  
**Source:** `Contracts/CreateTaskRequest.cs`, `Services/TaskService.cs:21-34`.

**`[inferred from code]`** — A title containing only spaces may satisfy length rules **before** trim (length ≥ 1) while trimming to an **empty** string in the service. Automated tests do not lock this behavior.

### BR-3 — Toggle completion

- **Trigger:** Client requests toggle for a task identifier.  
- **If no task exists:** Signal **not found** to the HTTP layer (no mutation).  
- **If task exists:** Invert the completion flag, persist, return updated record.  
**`[CONFIRMED BY TEST: ToggleTaskAsync_FlipsCompletionState]`**  
**Source:** `Services/TaskService.cs:37-48`, `Controllers/TasksController.cs:34-42`.

### BR-4 — HTTP status mapping (behavioral)

- List: **success** → response with status indicating success and a JSON array (may be empty).  
- Create: **success** → status indicating “created” and response body with the new task; **validation failure** → status indicating “bad request” with structured validation details.  
- Toggle: **success** → status indicating success with body; **not found** → status indicating “not found” with no body from this action.  
**Source:** `Controllers/TasksController.cs`.

**`[NEEDS CLARIFICATION]`** — The success response for create includes a `Location` header produced by linking to the list action with a route value `id`; the list action does not take `id` in its path. Consumers should rely on the response body, not the `Location` URL shape, unless verified.

---

## 6. Client behavior (browser application)

### 6.1 Configuration

- Base URL for the API defaults to `http://localhost:5276` unless overridden at build time via environment.  
**Source:** `client/src/api/tasksApi.ts:3`.

### 6.2 Remote calls

- **List:** GET `{base}/api/tasks` → JSON array.  
- **Create:** POST `{base}/api/tasks` with JSON `{ title, description? }`.  
- **Toggle:** PATCH `{base}/api/tasks/{id}/toggle` with no body.  
- If HTTP status is not successful, throw an error whose message includes the numeric status.  
**Source:** `client/src/api/tasksApi.ts`.

### 6.3 Task page

- On mount, load tasks: show **“Loading tasks…”** until the request settles.  
- On load failure: show **“Could not load tasks.”** and do not show the list.  
- On load success: show the list or empty state **“No tasks yet. Create your first one.”**  
**`[CONFIRMED BY TEST: shows loading then renders task list]`**, **`[CONFIRMED BY TEST: shows error state when loading fails]`** — `App.test.tsx`.

### 6.4 Create form

- On submit: if trimmed title is empty, show **“Title is required.”** and do not call the API.  
- Otherwise POST with trimmed title; description omitted if empty after trim.  
- On API failure: **“Could not create task. Please try again.”**  
- On success: clear fields; parent adds the returned task to the **front** of the local list.  
**`[CONFIRMED BY TEST: creates a task from the form]`**

### 6.5 Task list rows

- Show title, optional description, and **“Created {local datetime}”** from the task’s created instant (host locale).  
- Button: **“Mark as Complete”** or **“Mark as Incomplete”** depending on flag.  
**`[CONFIRMED BY TEST: toggles completion status]`**

### 6.6 Toggle failure

- On toggle API failure: set page error **“Could not update task status.”**  
- **`[inferred from code]`** — The same error flag hides the list (display logic requires no loading error and no error). Error is only cleared when a new load starts successfully; there is no in-app retry for the initial load failure or post-toggle recovery without full page reload.

---

## 7. Security and access control

- No authentication or per-user authorization. Any caller that can reach the API may perform all operations.  
- Browser cross-origin access is restricted to configured allowlisted origins; all HTTP methods and headers are permitted from those origins. Credentials are not enabled in the cross-origin policy.  
- HTTP-to-HTTPS redirection is enabled for the host.  
- Host allowlist in default configuration permits any host name.  
**Sources:** `Program.cs`, `appsettings.json`, `Controllers/TasksController.cs`.

---

## 8. Errors and edge cases

- **Validation errors** on create are returned before service logic runs.  
- **Missing task on toggle** maps to HTTP not-found.  
- **Client** does not parse server validation bodies for display; users see fixed strings for load/create/toggle failures.  
- **No** automatic retries, backoff, or circuit breakers in application code.  
**Sources:** `TasksController.cs`, `TasksPage.tsx`, `TaskForm.tsx`, `tasksApi.ts`.

---

## 9. Non-functional behavior

- Default log levels are configuration-driven (informational default; reduced noise for host framework category).  
- Development-only interactive API documentation is served when the environment is “Development.”  
- No background job processors or scheduled workers are registered.  
- User-visible strings are fixed in source; no internationalization layer is present in the client tree.  
**Sources:** `appsettings*.json`, `Program.cs:35-40`, client source scan.

---

## 10. Traceability index (primary)

| Topic | Location |
|-------|----------|
| Host pipeline | `server/TeamTasks.Api/Program.cs` |
| Routes | `server/TeamTasks.Api/Controllers/TasksController.cs` |
| Rules | `server/TeamTasks.Api/Services/TaskService.cs` |
| Persistence mapping | `server/TeamTasks.Api/Data/AppDbContext.cs` |
| Seed | `server/TeamTasks.Api/Data/DbInitializer.cs` |
| Client | `client/src/pages/TasksPage.tsx`, `components/TaskForm.tsx`, `components/TaskList.tsx`, `api/tasksApi.ts` |
| API tests | `tests/TeamTasks.Api.Tests/TaskServiceTests.cs` |
| Client tests | `client/src/App.test.tsx` |

---

## 11. Known gaps

- No automated HTTP-level tests for all status codes; behaviors marked `[inferred from code]` or `[ASSUMPTION]` where noted.  
- Ordering when two tasks share the same `CreatedAt` is **unspecified** (single sort key).  
- Whitespace-only title on the API: **`[inferred from code]`** — likely persists empty title; confirm with a live HTTP check when strict parity is required.
