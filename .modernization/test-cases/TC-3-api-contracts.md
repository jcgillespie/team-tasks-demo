# TC-3 — API Contracts (Black-Box)

**Public interface:** HTTP endpoints only.

---

## TC-3.01 — GET list success

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Input** | GET `/api/tasks` |
| **Expected output** | HTTP **200**, `Content-Type` JSON, array (possibly empty) |
| **Shape** | Each element has: id (number), title (string), description (string or null), isCompleted (boolean), createdAt (string instant) |

---

## TC-3.02 — POST create success

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Input** | POST `/api/tasks` with JSON `{"title":"A","description":null}` |
| **Expected output** | HTTP **201**; body contains new id and matching fields |
| **Side effects** | New row exists |

---

## TC-3.03 — POST validation failure

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Input** | POST with `{}` or missing title |
| **Expected output** | HTTP **400**; structured validation payload |

---

## TC-3.04 — PATCH toggle success

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Preconditions** | Existing task id |
| **Input** | PATCH `/api/tasks/{id}/toggle` |
| **Expected output** | HTTP **200**; `isCompleted` toggled from prior state |

---

## TC-3.05 — OPTIONS / CORS preflight **[ASSUMPTION]**

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Input** | Browser preflight from allowed origin |
| **Expected output** | CORS headers allow subsequent GET/POST/PATCH **[ASSUMPTION]** standard browser behavior |

---

## TC-3.06 — Location header on 201 **[NEEDS CLARIFICATION]**

| Field | Value |
|-------|--------|
| **Priority** | Low |
| **Input** | Successful POST |
| **Expected output** | `Location` header present per `CreatedAtAction` — exact URL may not reference a GET-by-id route; clients should not rely on Location without verification (`TasksController.cs:28`) |
