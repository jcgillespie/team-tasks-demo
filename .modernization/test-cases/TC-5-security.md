# TC-5 — Security & Access Control (Black-Box)

**Note:** System has no authentication; tests verify **absence** of gates and transport/config behaviors observable externally.

---

## TC-5.01 — Unauthenticated list access

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Input** | GET `/api/tasks` with no credentials |
| **Expected output** | HTTP **200** (when store available) |
| **Rationale** | Documents anonymous access (`Program.cs` has no auth) |

---

## TC-5.02 — Unauthenticated mutate

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Input** | POST and PATCH without credentials |
| **Expected output** | Same success/failure as with credentials — no 401 solely for missing auth |

---

## TC-5.03 — CORS deny from disallowed origin **[ASSUMPTION]**

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Input** | Cross-origin request from origin not in allowlist |
| **Expected output** | Browser blocks or server omits CORS allow **[ASSUMPTION]** — verify with browser devtools |

---

## TC-5.04 — Secrets not in responses

| Field | Value |
|-------|--------|
| **Priority** | Low |
| **Input** | Any successful JSON response |
| **Expected output** | No connection strings or keys in payload **[inferred from code]** |
