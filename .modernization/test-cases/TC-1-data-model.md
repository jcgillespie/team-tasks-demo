# TC-1 — Data Model & Persistence (Black-Box)

Technology-agnostic acceptance tests. **Public interfaces:** HTTP API for persisted state; observable DB file effects described behaviorally.

---

## TC-1.01 — Task title length boundary (server)

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | `CreateTaskRequest` validation → reject before service |
| **Preconditions** | Empty task store or isolated test database |
| **Input** | POST create body with `title` of length **121** (any ASCII), optional description valid |
| **Expected output** | HTTP **400**; no new task persisted |
| **Side effects** | No additional rows in `Tasks` |
| **Postconditions** | Store unchanged except possible unrelated data |

---

## TC-1.02 — Description max length (server)

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Code path** | Validation on `description` max length |
| **Preconditions** | Clean store |
| **Input** | POST with valid `title`, `description` length **1001** |
| **Expected output** | HTTP **400** |
| **Side effects** | No row inserted |

---

## TC-1.03 — Persisted field lengths match service trim

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Code path** | `TaskService.CreateTaskAsync` trim + save |
| **Preconditions** | Clean store |
| **Input** | POST with `title` = `"  hello  "`, `description` = `"  x  "` |
| **Expected output** | HTTP **201**; response `title` `"hello"`, `description` `"x"` |
| **Side effects** | Row stored with trimmed values |
| **Postconditions** | `[CONFIRMED BY TEST: creates a task from the form]` behavior aligned for trim (client); server trim from `TaskService.cs:25-26` |

---

## TC-1.04 — Seed idempotency

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Code path** | `DbInitializer`: skip seed when any row exists |
| **Preconditions** | Start with empty DB file; start app once (seed runs). **Or** pre-insert one row. |
| **Input** | Second process start against same file that already has ≥1 task |
| **Expected output** | Startup succeeds; seed does not insert duplicate fixed titles **[inferred from code]** — only if no rows existed initially |
| **Side effects** | Row count does not increase by 3 on second start if data already present |

---

## TC-1.05 — Primary key monotonicity **[ASSUMPTION]**

| Field | Value |
|-------|--------|
| **Priority** | Low |
| **Code path** | Insert two tasks sequentially |
| **Input** | Two successful creates |
| **Expected output** | Two distinct positive integer `id` values |
| **Postconditions** | Second id greater than first **[ASSUMPTION]** typical for auto-increment |
