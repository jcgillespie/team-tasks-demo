# Domain 6 — Security & Access Control

**Sources:** Static code review. **Technology-specific security library names** belong in Technology Profile.

---

## 1. Summary

| Topic | Behavior | Evidence |
|-------|----------|----------|
| Authentication | Not configured | `Program.cs:6-18` — no auth services |
| Authorization | Middleware registered; no `[Authorize]` or policies on endpoints | `Program.cs:50`, `TasksController.cs` (no attributes) |
| Effective access | Any caller that can reach the host may use all task operations | Inferred |
| HTTPS | Redirection middleware enabled | `Program.cs:48` |
| CORS | Allowlisted origins; any method/header from those origins | `Program.cs:20-30`, `49` |
| Host filter | `AllowedHosts: "*"` | `appsettings.json:14` |

---

## 2. Authentication

No login, tokens, API keys, or session establishment in `Program.cs` or controllers.

---

## 3. Authorization

`UseAuthorization()` runs without registered authentication schemes; endpoints remain anonymously accessible (`TasksController.cs`).

---

## 4. CORS (behavioral)

- Origins: configuration array or code default dev client origin (`Program.cs:20-21`, `appsettings.json:5-7`).
- CORS restricts **browser** cross-origin calls; non-browser clients are not limited by CORS.

---

## 5. Sensitive configuration

`appsettings.json` contains local DB path and dev CORS origin — no passwords or cloud keys in reviewed files. `*.db` in `.gitignore`.

---

## 6. API documentation exposure

OpenAPI + interactive docs only when environment is Development (`Program.cs:35-39`).

---

## Gaps

`[ASSUMPTION]` Production may add reverse proxy auth, TLS, or network rules not in repo.  
**Tests:** No security-focused automated tests found.

---

## Traceability

`server/TeamTasks.Api/Program.cs`, `Controllers/TasksController.cs`, `appsettings.json`, `client/src/api/tasksApi.ts`.
