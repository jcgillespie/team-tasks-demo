# Adversarial Review — Accuracy

**Method:** Spot-check claims against repository files.

---

## Verified correct

| Claim | Verification |
|-------|----------------|
| List order `CreatedAt` descending | `TaskService.cs:13-14` matches tests |
| Toggle null → 404 | `TasksController.cs:36-39` |
| CORS fallback `localhost:5173` | `Program.cs:20-21` |
| Seed three tasks when empty | `DbInitializer.cs:17-39` |
| Client prepends created task | `TasksPage.tsx:32` |

---

## Corrections / risks

| ID | Issue | Correct behavior |
|----|---------|------------------|
| A-01 | Subagent typo “TaskTask.cs” | Use `TaskService.cs` only |
| A-02 | JSON camelCase | Default for ASP.NET Core JSON; **accurate** `[ASSUMPTION]` unless `AddJsonOptions` added — **none in Program.cs** ✓ |
| A-03 | `CreatedAtAction` Location URL | Confirmed: `GetTasks` has no `id` parameter — Location may be odd; **NEEDS CLARIFICATION** for consumers is valid |

---

## Formula spot-check

- **Trim:** `"  x  "` → `"x"` for description — `TaskService.cs:26` ✓
- **Toggle:** `!IsCompleted` — `TaskService.cs:45` ✓

---

## Test expected vs code

| Test | Match? |
|------|--------|
| `App.test.tsx` expects “Loading tasks...” | `TasksPage.tsx:58` ✓ |
| `App.test.tsx` “Mark as Complete” | `TaskList.tsx:23` ✓ |

**Conclusion:** No material factual errors found in core paths; Location header and whitespace-title edge case remain explicitly flagged.
