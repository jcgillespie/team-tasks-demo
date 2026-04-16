# TC-6 — Error Handling (Black-Box)

---

## TC-6.01 — Client maps HTTP error to thrown error

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Preconditions** | API returns 500 or 404 for a call made through client adapter |
| **Expected output** | Promise rejects; message includes status code substring “Request failed:” (`tasksApi.ts:6`) |

---

## TC-6.02 — Create validation 400 → user message

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Preconditions** | Form submits invalid payload that server rejects |
| **Expected output** | User sees “Could not create task. Please try again.” (generic; body not parsed) |

---

## TC-6.03 — Toggle 404 → page error

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Preconditions** | UI has stale id; server returns 404 |
| **Expected output** | “Could not update task status.” |

---

## TC-6.04 — No retry on load failure

| Field | Value |
|-------|--------|
| **Priority** | Low |
| **Preconditions** | Initial load failed |
| **Expected output** | No automatic second fetch; user must reload page **[inferred from code]** |

---

## TC-6.05 — Server unhandled exception **[ASSUMPTION]**

| Field | Value |
|-------|--------|
| **Priority** | Low |
| **Input** | Force internal error (e.g. corrupt DB) |
| **Expected output** | HTTP 5xx; client shows load/create/toggle error string as applicable **[ASSUMPTION]** host default |
