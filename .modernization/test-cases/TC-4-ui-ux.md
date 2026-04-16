# TC-4 — UI/UX (Black-Box)

**Public interface:** Rendered UI and user events (browser automation or accessibility tree).

---

## TC-4.01 — Loading then list

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Preconditions** | API returns tasks successfully |
| **Input** | Open app |
| **Expected output** | Briefly shows loading text; then task titles visible |
| **Evidence** | `[CONFIRMED BY TEST: shows loading then renders task list]` |

---

## TC-4.02 — Create flow

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Preconditions** | API accepts create |
| **Input** | Type title + description, submit |
| **Expected output** | New task appears in list |
| **Evidence** | `[CONFIRMED BY TEST: creates a task from the form]` |

---

## TC-4.03 — Toggle labels

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Preconditions** | Task incomplete; API returns updated task on toggle |
| **Input** | Click “Mark as Complete” |
| **Expected output** | Button label becomes “Mark as Incomplete” |
| **Evidence** | `[CONFIRMED BY TEST: toggles completion status]` |

---

## TC-4.04 — Load error

| Field | Value |
|-------|--------|
| **Priority** | High |
| **Preconditions** | API list fails |
| **Input** | Open app |
| **Expected output** | Message “Could not load tasks.” |
| **Evidence** | `[CONFIRMED BY TEST: shows error state when loading fails]` |

---

## TC-4.05 — Empty list copy

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Preconditions** | API returns `[]` |
| **Expected output** | “No tasks yet. Create your first one.” |

---

## TC-4.06 — Toggle error hides list **[inferred from code]**

| Field | Value |
|-------|--------|
| **Priority** | Medium |
| **Preconditions** | Tasks loaded; toggle API then fails |
| **Input** | Trigger toggle error |
| **Expected output** | Error “Could not update task status.” **and** list region not shown (`TasksPage.tsx:60`) |
