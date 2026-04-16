# Domain 8 — Non-Functional Requirements & Operational Behavior

**Evidence:** Static analysis. Deployment/runtime outside repo: `[NEEDS CLARIFICATION]` / `[ASSUMPTION]`.

---

## 1. Logging

Default log levels via configuration: default category Information; reduced verbosity for host framework category (`appsettings.json:8-12`, `appsettings.Development.json`).

No explicit structured logging calls in `TasksController` or `TaskService` (`[inferred from code]`).

---

## 2. Read performance

List query uses no change tracking on the entity set before projection (`TaskService.cs:12-15`).

---

## 3. Writes

Create: add + save. Toggle: load by key with tracking, mutate, save (`TaskService.cs`).

No application-level response caching.

---

## 4. Startup / database

- Connection string resolution with file default (`Program.cs:11-16`).
- `EnsureCreated` then conditional seed (`DbInitializer.cs:8-15`, `Program.cs:42-46`).

`[ASSUMPTION]` Concurrent multi-instance startup against one file could contend.  
`[NEEDS CLARIFICATION]` Long-term schema evolution strategy (initializer uses ensure-created, not migration artifacts in repo).

---

## 5. Background work

No hosted background services or schedulers in `Program.cs` (`[inferred from code]`).

---

## 6. Internationalization

No i18n resource files or locale libraries in `client/src` (`[inferred from code]` — grep for common i18n markers).

---

## 7. Observability beyond logging

No health check endpoints, metrics, or distributed tracing registration in `Program.cs`. Dev-only API documentation UI (`Program.cs:35-39`).

---

## Summary table

| NFR | Behavior | Source |
|-----|----------|--------|
| Logging | Config-driven defaults | `appsettings*.json` |
| Read path | No tracking on list query | `TaskService.cs:12-15` |
| Startup | Ensure DB + seed if empty | `DbInitializer.cs`, `Program.cs:42-46` |
| Background | None | `Program.cs` |
| i18n | Fixed strings | Client sources |
