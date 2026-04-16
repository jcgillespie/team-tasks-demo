# Domain 5 — UI/UX Flows & User Interactions

**Scope:** `client/src` — `TasksPage`, `TaskForm`, `TaskList`, `api/tasksApi`.  
**Traceability:** `App.test.tsx` where marked `[CONFIRMED BY TEST]`.

---

## 1. Screen inventory

| Region | Purpose |
|--------|---------|
| Application shell | Full-viewport wrapper (`App.tsx`) |
| Page header | Title “Team Tasks”, subtitle (`TasksPage.tsx:46-48`) |
| Create panel | Form: title, description, submit (`TaskForm.tsx`) |
| List panel | Heading “Tasks”, loading line, error line, list or empty (`TasksPage.tsx:56-60`, `TaskList.tsx`) |

**List item:** Title, optional description, “Created {local datetime}”, toggle button text “Mark as Complete” / “Mark as Incomplete” (`TaskList.tsx:16-24`).

**Empty state:** “No tasks yet. Create your first one.” (`TaskList.tsx:9-10`).

---

## 2. Journeys

### Initial load

1. Tasks empty, loading true, error null (`TasksPage.tsx:8-10`).
2. On mount, `loadTasks` runs (`TasksPage.tsx:26-28`).
3. On failure: error “Could not load tasks.” (`TasksPage.tsx:19-20`).
4. While loading: “Loading tasks…” (`TasksPage.tsx:58`).

**`[CONFIRMED BY TEST: shows loading then renders task list]`** — `App.test.tsx:26-32`.

### Create (happy path)

Trimmed title required client-side; on success prepend task to state (`TasksPage.tsx:30-33`).

**`[CONFIRMED BY TEST: creates a task from the form]`** — `App.test.tsx:35-60`.

### Toggle

**`[CONFIRMED BY TEST: toggles completion status]`** — `App.test.tsx:63-79`.

### Load failure

**`[CONFIRMED BY TEST: shows error state when loading fails]`** — `App.test.tsx:82-87`.

### Toggle failure UX

On toggle error, `TasksPage` sets `error` and hides list when `!isLoading && !error` (`TasksPage.tsx:58-60`). Error is only cleared at start of successful `loadTasks` (`TasksPage.tsx:14`). No automatic retry. **`[inferred from code]`** — user must reload page to see list again after toggle failure.

---

## 3. Client vs server validation

| Concern | Client | Server |
|---------|--------|--------|
| Title | Non-empty after trim; HTML `required` on input | Required, 1–120 chars |
| Description | Omitted if empty after trim | Optional, max 1000 |
| Length limits | Not enforced for 120/1000 in UI | Enforced on POST |

---

## 4. Loading / error strings

| Trigger | Message |
|---------|---------|
| Load failure | `Could not load tasks.` |
| Toggle failure | `Could not update task status.` |
| Create failure | `Could not create task. Please try again.` |
| Empty title | `Title is required.` |

---

## 5. Network adapter behavior

Base URL from environment override with localhost default (`tasksApi.ts:3`). Non-OK HTTP → thrown error with message including status (`tasksApi.ts:4-8`).

---

## Traceability

`pages/TasksPage.tsx`, `components/TaskForm.tsx`, `components/TaskList.tsx`, `api/tasksApi.ts`, `types/task.ts`.
