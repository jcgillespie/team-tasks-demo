# Adversarial Review — Ambiguity & Edge Cases

---

## Ambiguous phrases to tighten in reimplementation specs

| Phrase | Issue | Resolution |
|--------|--------|------------|
| “Newest first” | Clock skew / equal `CreatedAt` | **[ASSUMPTION]** Stable secondary sort not defined — two tasks same instant: order unspecified (`TaskService` orders only by `CreatedAt`) |
| “Invalid Date” in UI | Malformed ISO string | Document as acceptable display glitch or validate in reimplementation |
| “Generic error” on create | 400 vs 500 | Both map to same user string — intentional? |

---

## Untagged assumptions in domain docs

- Configuration precedence (env vs JSON) — tagged `[ASSUMPTION]` in Domain 1 ✓
- Concurrent toggle — not tagged in TC-4; add if stress-testing

---

## Can tests be implemented from spec alone?

| Test ID | Implementable? | Missing detail |
|---------|----------------|----------------|
| TC-3.05 CORS | Partial | Exact allowed origins must be copied from deployment config |
| TC-2.04 | No | Needs clarification row for whitespace title |

---

## Unicode / encoding

- Title/description: max lengths are **character** counts in validation — Unicode code points **[ASSUMPTION]** per typical host behavior; not tested with emoji or combining characters.

---

## Timezone

- Server stores `DateTime` (UTC for new items at `TaskService.cs:28`).
- Client displays `toLocaleString()` — local zone **[ASSUMPTION]**.
