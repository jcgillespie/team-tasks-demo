# Domain 4 — API Contracts & Integrations

**Scope:** HTTP API in `server/TeamTasks.Api/`. **Technology names** for serialization defaults appear only where necessary for traceability; default JSON naming is camelCase for public properties.

---

## 1. Processing pipeline

Order in `Program.cs`:

1. **Development only:** OpenAPI document + interactive API docs UI (`35-40`).
2. HTTPS redirection (`48`).
3. CORS policy `ClientCors` (`49`).
4. Authorization middleware (`50`) — no authentication services registered.
5. Controller endpoints (`51`).

No custom exception middleware or global filters in `Program.cs`. `[ApiController]` on `TasksController` enables automatic model validation behavior (`TasksController.cs:7`).

`[ASSUMPTION]` JSON serialization uses stack defaults (camelCase property names in JSON for PascalCase record/DTO members unless configured elsewhere; no custom options in `Program.cs`).

---

## 2. External integrations

- **Inbound only:** Browser or other HTTP clients.
- **Outbound:** None in application code beyond the persistence provider.
- **CORS:** Browser cross-origin policy as configured (`Program.cs:20-30`, `appsettings.json:5-7`).

---

## 3. Routes

Base: **`api/tasks`** (`TasksController.cs:8`).

| Method | Path | Handler |
|--------|------|---------|
| GET | `/api/tasks` | List tasks |
| POST | `/api/tasks` | Create task |
| PATCH | `/api/tasks/{id}/toggle` | Toggle completion (`id` int) |

---

## 4. GET `/api/tasks`

- **Request:** No body; no query parameters.
- **200 OK:** JSON array of task objects: `id`, `title`, `description` (null allowed), `isCompleted`, `createdAt` (ISO 8601).
- **Ordering:** Newest `createdAt` first (`TaskService.cs:12-15`).

---

## 5. POST `/api/tasks`

- **Body:** `{ "title": string, "description"?: string }` per `CreateTaskRequest` constraints (`CreateTaskRequest.cs`).
- **201 Created:** Body = created task object (`TasksController.cs:26-28`).
- **400 Bad Request:** Declared as `ValidationProblemDetails` (`TasksController.cs:21`); typical validation error shape when model invalid.

---

## 6. PATCH `/api/tasks/{id}/toggle`

- **Request:** No body.
- **200 OK:** Updated task object.
- **404 Not Found:** No task for `id` (`TasksController.cs:36-39`).

---

## 7. CORS (behavioral)

- Origins from `Cors:AllowedOrigins` or default single localhost origin for dev client port (`Program.cs:20-21`, `appsettings.json:5-7`).
- Policy allows any header and any method from allowed origins (`Program.cs:27-29`).
- No credentials flag in policy.

---

## Traceability

| Topic | Source |
|--------|--------|
| Routes | `Controllers/TasksController.cs` |
| Request rules | `Contracts/CreateTaskRequest.cs` |
| Response shape | `Contracts/TaskResponse.cs` |
| Semantics | `Services/TaskService.cs` |

**Tests:** No HTTP-level integration tests in test project; validation of `CreateTaskRequest` exercised via `Validator` in unit tests only (`TaskServiceTests.cs:76-81`).
