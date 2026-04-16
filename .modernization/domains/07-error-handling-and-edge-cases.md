# Domain 7 — Error Handling & Edge Cases

**Scope:** HTTP API (`server/TeamTasks.Api/`) and client (`client/src/`).

---

## 1. Error taxonomy

| Category | Behavior |
|----------|----------|
| HTTP non-success | Client throws after fetch; message includes status code (`tasksApi.ts:5-7`) |
| Validation (create) | HTTP 400 for invalid body (`CreateTaskRequest.cs`, `TasksController.cs:21`) |
| Missing task (toggle) | Service returns absent → HTTP 404 (`TaskService.cs:39-42`, `TasksController.cs:37-39`) |
| Network / no response | Same client catch paths as HTTP errors |

**Not present:** Retry, backoff, circuit breaker, idempotency keys (`[inferred from code]`).

---

## 2. Client HTTP guard

`parseJsonOrThrow`: if `!response.ok`, throw `Error` with text `Request failed: {status}` (`tasksApi.ts:4-8`). Response body on errors is not parsed for user messaging.

**`[CONFIRMED BY TEST: shows error state when loading fails]`** — `App.test.tsx:82-87`.

---

## 3. User-visible messages

| Location | Message |
|----------|---------|
| Load | `Could not load tasks.` (`TasksPage.tsx:19-20`) |
| Toggle | `Could not update task status.` (`TasksPage.tsx:39-40`) |
| Create | `Could not create task. Please try again.` / `Title is required.` (`TaskForm.tsx`) |

---

## 4. Edge cases

| Scenario | Behavior |
|----------|----------|
| Empty list | Empty-state copy (`TaskList.tsx:9-11`) |
| Whitespace-only title (client) | Blocked (`TaskForm.tsx:18-19`) |
| Whitespace-only title (server) | `[ASSUMPTION]` May pass validation then trim to empty — gap |
| Invalid `createdAt` in JSON | `new Date(...)` for display may show “Invalid Date” (`TaskList.tsx:20`) — not guarded |
| Double-click toggle | No in-flight guard (`TaskList.tsx:22-24`) |

---

## 5. Unhandled server exceptions

No custom global exception handler in `Program.cs`. **`[ASSUMPTION]`** Host default produces 500 with environment-dependent detail.

---

## 6. Test gaps

- No automated HTTP tests for 400 POST or 404 PATCH.
- No test for invalid JSON on 200 response.

**`[CONFIRMED BY TEST: CreateTaskRequest_Title_IsRequired]`** — validation helper only, not HTTP (`TaskServiceTests.cs:12-24`).

---

## Traceability

`client/src/api/tasksApi.ts`, `pages/TasksPage.tsx`, `components/TaskForm.tsx`, `Services/TaskService.cs`, `Controllers/TasksController.cs`.
