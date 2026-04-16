# Domain 2 — Data Model & Persistence

**Scope:** `TaskItem` entity, persistence model configuration, seed data, connection string resolution.  
**Stack-agnostic:** Described by constraints and behavior, not vendor product names.

---

## 1. Entity — `TaskItem`

**Source:** `server/TeamTasks.Api/Models/TaskItem.cs:3-14`

| Logical field | Meaning |
|---------------|---------|
| `Id` | Surrogate key; integer. |
| `Title` | Required human-readable label. |
| `Description` | Optional longer text. |
| `IsCompleted` | Completion flag. |
| `CreatedAt` | Instant when the row was created (UTC at service layer for new items). |

---

## 2. Persisted constraints

**Source:** `server/TeamTasks.Api/Data/AppDbContext.cs:10-21`

| Element | Constraint |
|---------|------------|
| Table name | `Tasks` |
| `Id` | Primary key |
| `Title` | Required; maximum length **120** characters |
| `Description` | Optional; maximum length **1000** when present |
| `CreatedAt` | Required |
| `IsCompleted` | No extra fluent config in snippet; `[ASSUMPTION]` non-null boolean with provider default for new rows |

`[ASSUMPTION]` Integer primary keys are auto-generated for inserts. **Evidence:** `task.Id` used after save in tests (`TaskServiceTests.cs:67-68`).

---

## 3. API request validation alignment

**Source:** `server/TeamTasks.Api/Contracts/CreateTaskRequest.cs:5-13`

| Field | API validation |
|-------|----------------|
| `Title` | Required; length **1–120** inclusive |
| `Description` | Optional; max **1000** |

**Test:** `[CONFIRMED BY TEST: CreateTaskRequest_Title_IsRequired]` — empty `Title` fails validation (`TaskServiceTests.cs:12-24`).

**Service normalization:** `TaskService.CreateTaskAsync` trims `Title` and `Description`, and sets description to null if whitespace-only (`TaskService.cs:25-26`). **`[ASSUMPTION]`** A title consisting only of spaces may pass length checks before trim and persist as empty after trim — not covered by tests (`CreateTaskRequest` + `TaskService` interaction).

---

## 4. Connection string resolution

1. Read `ConnectionStrings:DefaultConnection` from host configuration (`Program.cs:13`).
2. If missing, use `Data Source=teamtasks.db` (`Program.cs:14`).
3. Default in `appsettings.json` matches (`appsettings.json:2-3`).

`[ASSUMPTION]` Relative file path resolves relative to process working directory.

---

## 5. Initialization and seed

**Source:** `server/TeamTasks.Api/Data/DbInitializer.cs:8-42`, invoked at startup `Program.cs:42-45`.

| Step | Behavior |
|------|----------|
| Schema | Ensure database exists (create if not). |
| Idempotency | If any row exists in tasks, return. |
| Seed | Insert three tasks with fixed English copy; `CreatedAt` = `UtcNow` minus 30, 20, 10 minutes respectively. |

---

## 6. Access patterns

- **List:** No change tracking on query; order by `CreatedAt` descending (`TaskService.cs:12-15`).
- **Create:** Add entity, save (`TaskService.cs:31-32`).
- **Toggle:** Load by id with tracking, flip flag, save (`TaskService.cs:37-48`).

**Tests** use in-memory provider (`TaskServiceTests.cs:86-88`), not the file-backed store used at runtime.

---

## Traceability

| Topic | Sources |
|--------|---------|
| Entity | `Models/TaskItem.cs` |
| Fluent mapping | `Data/AppDbContext.cs` |
| Seed | `Data/DbInitializer.cs`, `Program.cs:42-45` |
| Trim rules | `Services/TaskService.cs` |
