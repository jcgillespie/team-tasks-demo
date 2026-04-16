# TC-2 — Business Logic & Rules (Black-Box)

---

## TC-2.01 — List order newest first

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | `GetTasksAsync` ordering |
| **Preconditions** | At least two tasks with different `createdAt` |
| **Input** | GET list |
| **Expected output** | Array ordered so first element has the **latest** `createdAt` |
| **Evidence** | `[CONFIRMED BY TEST: GetTasksAsync_ReturnsTasksOrderedByCreatedAtDescending]` |

---

## TC-2.02 — Toggle flips twice

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | `ToggleTaskAsync` |
| **Preconditions** | One task with `isCompleted === false` |
| **Input** | PATCH toggle twice for same id |
| **Expected output** | First response `isCompleted true`; second `isCompleted false` |
| **Evidence** | `[CONFIRMED BY TEST: ToggleTaskAsync_FlipsCompletionState]` |

---

## TC-2.03 — Toggle missing id

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | Toggle when no row |
| **Preconditions** | Id not present (e.g. max id + 1) |
| **Input** | PATCH toggle |
| **Expected output** | HTTP **404** |
| **Side effects** | No row created or altered |

---

## TC-2.04 — Whitespace-only title **[NEEDS CLARIFICATION]**

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | Validation vs trim order |
| **Preconditions** | Clean store |
| **Input** | POST with `title` = `"   "` (spaces only, length 3) |
| **Expected output** | **[NEEDS CLARIFICATION]** Either 400 (if validation rejects) or 201 with empty title after trim — document actual behavior when reimplementing |

---

## TC-2.05 — Client blocks empty title

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | `TaskForm` submit |
| **Preconditions** | App loaded |
| **Input** | Submit with title blank or whitespace-only |
| **Expected output** | Error text “Title is required.”; no create HTTP call |
| **Evidence** | Inferred from `TaskForm.tsx:18-21`; partial overlap `[CONFIRMED BY TEST: creates a task from the form]` for happy path only |
