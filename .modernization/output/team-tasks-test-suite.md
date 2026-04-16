# Team Tasks — Behavioral Test Suite

**Document type:** Technology-agnostic black-box acceptance tests.  
**Scope:** Public interfaces only (HTTP API, browser UI, environment configuration).  
**Referenced code paths** are for traceability to the legacy implementation, not for white-box assertions.

---

## Coverage matrix

| Area | Test IDs | Automated in legacy repo? |
|------|----------|---------------------------|
| Data model / persistence | TC-1.01 – TC-1.05 | Partial (unit-level only) |
| Business rules | TC-2.01 – TC-2.05 | TC-2.01, TC-2.02 confirmed (service tests); TC-2.04 needs HTTP |
| API contracts | TC-3.01 – TC-3.06 | Not fully — no HTTP integration suite |
| UI/UX | TC-4.01 – TC-4.06 | TC-4.01–04 confirmed (client tests); TC-4.05–06 partial |
| Security | TC-5.01 – TC-5.04 | Manual / inferred |
| Error handling | TC-6.01 – TC-6.05 | TC-6 partial (load error confirmed) |

---

## TC-1 — Data model & persistence

### TC-1.01 — Title over max length
- **Preconditions:** Clean store.  
- **Input:** Create with title length **121**, valid description.  
- **Expected:** Rejection without insert (**400** class).  
- **Path:** `CreateTaskRequest.cs:7-9`, `AppDbContext.cs:18`.

### TC-1.02 — Description over max length
- **Input:** Valid title; description length **1001**.  
- **Expected:** Rejection without insert.  
- **Path:** `CreateTaskRequest.cs:11-12`.

### TC-1.03 — Trim persistence
- **Input:** Title `"  hello  "`, description `"  x  "`.  
- **Expected:** Stored/displayed title `hello`, description `x`.  
- **Path:** `TaskService.cs:25-26`.

### TC-1.04 — Seed idempotency
- **Preconditions:** Database already contains ≥1 task.  
- **Input:** Application restart.  
- **Expected:** No duplicate injection of the three seed titles.  
- **Path:** `DbInitializer.cs:12-15`.

### TC-1.05 — Distinct identifiers
- **Input:** Two successful creates.  
- **Expected:** Two different positive integer identifiers.  
- **Path:** `[ASSUMPTION]` auto-increment semantics.

---

## TC-2 — Business logic

### TC-2.01 — Sort order
- **`[CONFIRMED BY TEST: GetTasksAsync_ReturnsTasksOrderedByCreatedAtDescending]`**

### TC-2.02 — Double toggle
- **`[CONFIRMED BY TEST: ToggleTaskAsync_FlipsCompletionState]`**

### TC-2.03 — Toggle unknown id
- **Expected:** **404**; no data change.  
- **Path:** `TaskService.cs:39-42`, `TasksController.cs:37-39`.

### TC-2.04 — Whitespace-only title (API)
- **Input:** Title `"   "` (spaces only).  
- **Expected:** **`[NEEDS CLARIFICATION / inferred from code]`** — validation may pass; empty title after trim. Document actual HTTP status and body after live verification.

### TC-2.05 — Client empty title
- **Expected:** “Title is required.”; no network create.  
- **Path:** `TaskForm.tsx:18-21`.

---

## TC-3 — API contracts

### TC-3.01 — GET success
- **Expected:** **200** + JSON array.

### TC-3.02 — POST success
- **Expected:** **201** + task body.

### TC-3.03 — POST validation failure
- **Expected:** **400** + validation structure.

### TC-3.04 — PATCH success
- **Expected:** **200** + updated task.

### TC-3.05 — PATCH not found
- **Expected:** **404**.

### TC-3.06 — CORS preflight
- **`[ASSUMPTION]`** — Allowed origin receives appropriate CORS headers for GET/POST/PATCH.

### TC-3.07 — Location header
- **`[NEEDS CLARIFICATION]`** — Document observed `Location` value on **201**; do not assume RESTful resource URL.

---

## TC-4 — UI/UX

### TC-4.01 — Loading then list
- **`[CONFIRMED BY TEST: shows loading then renders task list]`**

### TC-4.02 — Create
- **`[CONFIRMED BY TEST: creates a task from the form]`**

### TC-4.03 — Toggle labels
- **`[CONFIRMED BY TEST: toggles completion status]`**

### TC-4.04 — Load error
- **`[CONFIRMED BY TEST: shows error state when loading fails]`**

### TC-4.05 — Empty list
- **Expected:** “No tasks yet. Create your first one.”

### TC-4.06 — Toggle error hides list
- **`[inferred from code]`** — `TasksPage.tsx:58-60`.

---

## TC-5 — Security

### TC-5.01 — Anonymous list
- **Expected:** **200** without credentials.

### TC-5.02 — Anonymous mutate
- **Expected:** Success/failure same as unauthenticated baseline (no **401** solely for missing credentials).

### TC-5.03 — Disallowed origin
- **`[ASSUMPTION]`** — Browser blocks or server withholds CORS success.

### TC-5.04 — No secrets in JSON
- **Expected:** Responses do not echo connection strings or keys.

---

## TC-6 — Error handling

### TC-6.01 — Non-OK HTTP
- **Expected:** Client surfaces failure; message includes status (`tasksApi.ts:6`).

### TC-6.02 — Create failure message
- **Expected:** Generic create error string on any failure.

### TC-6.03 — Toggle failure message
- **Expected:** “Could not update task status.”

### TC-6.04 — No auto-retry load
- **`[inferred from code]`**

### TC-6.05 — Server fault
- **`[ASSUMPTION]`** — **5xx**; client shows generic error path.

---

## Postconditions template (all tests)

After each test, system state should remain consistent: no orphaned partial writes except where the specification defines transactional behavior (single save per operation in current design).

---

## Traceability

| Suite | Legacy files |
|-------|----------------|
| API unit tests | `tests/TeamTasks.Api.Tests/TaskServiceTests.cs` |
| Client tests | `client/src/App.test.tsx` |
