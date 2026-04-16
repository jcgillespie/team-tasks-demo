# Domain 3 — Business Logic & Rules

**Sources:** `TaskService.cs`, `CreateTaskRequest.cs`, `TasksController.cs`, `TaskForm.tsx`; tests `TaskServiceTests.cs`, `App.test.tsx`.

---

## 1. List tasks — sort order

**Behavior:** Newest first by creation instant (`CreatedAt` descending).

**Trace:** `server/TeamTasks.Api/Services/TaskService.cs:12-15`.

**`[CONFIRMED BY TEST: GetTasksAsync_ReturnsTasksOrderedByCreatedAtDescending]`** — `TaskServiceTests.cs:27-49`.

---

## 2. Create task — service rules

| Step | Logic |
|------|--------|
| Title | `title.Trim()` stored |
| Description | If `string.IsNullOrWhiteSpace(description)` → stored **null**; else `description.Trim()` |
| Defaults | `IsCompleted = false`, `CreatedAt = DateTime.UtcNow` |
| Persist | Insert and save, return mapped response |

**Trace:** `TaskService.cs:21-34`.

**Cross-layer gap:** Validation may accept a title that is only spaces (length ≥ 1 before trim); service would store an **empty** title after trim. `[ASSUMPTION]` product intent; **not** covered by tests.

---

## 3. Toggle completion

| Branch | Result |
|--------|--------|
| No row for `id` | Return absent (`null`); no persistence change |
| Row found | Negate `IsCompleted`, save, return mapped response |

**Trace:** `TaskService.cs:37-48`.

**`[CONFIRMED BY TEST: ToggleTaskAsync_FlipsCompletionState]`** — `TaskServiceTests.cs:53-73`.

---

## 4. Create request — validation

| Field | Rules |
|-------|--------|
| `Title` | Required; length 1–120 |
| `Description` | Optional; max 1000 |

**Trace:** `CreateTaskRequest.cs:7-12`.

**`[CONFIRMED BY TEST: CreateTaskRequest_Title_IsRequired]`** — `TaskServiceTests.cs:12-24`.

`[ASSUMPTION]` Invalid POST bodies yield HTTP 400 with validation problem details via API controller conventions; not exercised via HTTP integration tests.

---

## 5. HTTP controller outcomes

| Operation | Success | Failure |
|-----------|---------|---------|
| GET list | 200 + collection | Not modeled for errors |
| POST create | 201 + body; `Location` via `CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created)` | 400 when validation fails |
| PATCH toggle | 200 + body | 404 when service returns absent |

**Trace:** `TasksController.cs:11-43`.

`[NEEDS CLARIFICATION]` `CreatedAtAction` targets `GetTasks` which has no `{id}` route — generated `Location` may not be a canonical single-resource URL (`TasksController.cs:28`).

**`[CONFIRMED BY TEST: toggles completion status]`** (client) — `App.test.tsx:63-79` — does not assert HTTP 404.

---

## 6. Client form — `TaskForm`

| Order | Branch | Behavior |
|-------|--------|----------|
| 1 | `title.trim()` empty | Error “Title is required.”; no API call |
| 2 | Else | Clear error, submitting state, call `onCreate` with trimmed title; `description` as `trim` or `undefined` if empty after trim |
| 3 | Success | Clear fields |
| 4 | Failure | “Could not create task. Please try again.” |

**Trace:** `client/src/components/TaskForm.tsx:15-37`.

**`[CONFIRMED BY TEST: creates a task from the form]`** — `App.test.tsx:35-60`.

---

## 7. Post-create list order (client)

After create, new task is **prepended** to local state (`TasksPage.tsx:30-33`). Server list API returns newest-first; prepend matches that ordering for the new item.

---

## Rule catalog

| ID | Trigger | Action |
|----|---------|--------|
| BR-TASK-001 | List | Return tasks sorted by `CreatedAt` descending |
| BR-TASK-002 | Create (service) | Trim title; null whitespace-only description; set incomplete; UTC now |
| BR-TASK-003 | Create (API) | Reject invalid body before service |
| BR-TASK-004 | Toggle | Absent id → not found to HTTP layer |
| BR-TASK-005 | Toggle | Flip `IsCompleted` when found |
| BR-TASK-006 | Form | Block empty trimmed title client-side |
