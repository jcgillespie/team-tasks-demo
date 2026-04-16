# Adversarial Review — Completeness

**Reviewer role:** Compare documentation and test cases to **actual source code**. Untrusted prior docs.

---

## Findings

| ID | Severity | Gap | Source to add |
|----|----------|-----|---------------|
| C-01 | Medium | **Partial class Program** at `Program.cs:55` — supports test host/WebApplicationFactory patterns; not documented in architecture domain as a test hook | `server/TeamTasks.Api/Program.cs:55` |
| C-02 | Medium | **No integration tests** for HTTP pipeline — documented in discovery but TC-3 should stress that several API tests are **not** `[CONFIRMED BY TEST]` at HTTP layer | `tests/TeamTasks.Api.Tests/` |
| C-03 | Low | **`TeamTasks.Api.http`** file may contain manual API examples — not reflected in spec | `server/TeamTasks.Api/TeamTasks.Api.http` |
| C-04 | High | **Whitespace-only title** server behavior — test case TC-2.04 flags `[NEEDS CLARIFICATION]`; spec must record outcome after manual HTTP test | `CreateTaskRequest.cs` + `TaskService.cs` |
| C-05 | Low | **`docs/architecture.md`** may add narrative not merged into domains | `docs/architecture.md` |

## Branch coverage vs tests

| Branch | Covered by automated black-box? |
|--------|-------------------------------------|
| GET always 200 | Partial (client mock only) |
| POST 400 validation | Not HTTP-level |
| PATCH 404 | Not automated |
| Seed skip when rows exist | TC-1.04 partial |

## Severity summary

- **Medium:** Test infrastructure (`Program` partial), HTTP gap for acceptance tests.
- **High:** Whitespace title ambiguity needs one explicit HTTP experiment to close.
