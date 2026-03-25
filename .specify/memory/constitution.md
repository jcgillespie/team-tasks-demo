<!--
SYNC IMPACT REPORT
==================
Version change: (new) → 1.0.0

Modified principles:
	- All placeholders filled for the first time; no prior principles to compare.

Added sections:
	- I. Code Quality
	- II. Test-Driven Development (NON-NEGOTIABLE)
	- III. Clean Documentation
	- IV. UX Consistency
	- V. Simplicity
	- Quality Gates
	- Development Workflow
	- Governance

Removed sections:
	- None (initial fill)

Templates reviewed:
	- .specify/templates/plan-template.md       ✅ Constitution Check section aligns
	- .specify/templates/spec-template.md       ✅ User Scenarios & Requirements aligns
	- .specify/templates/tasks-template.md      ✅ TDD task ordering (tests before impl) aligns

Deferred TODOs:
	- None
-->

# Team Tasks Constitution

## Core Principles

### I. Code Quality (NON-NEGOTIABLE)

Every line of code merged into the project MUST be clean, readable, and maintainable:

- Code MUST follow established language conventions (C# .NET/ASP.NET style for the server;
	TypeScript/React best practices for the client).
- Functions and methods MUST have a single, clear responsibility. Complex logic MUST be
	broken into well-named, composable units.
- Magic numbers, unexplained booleans, and ad-hoc inline logic MUST NOT be introduced
	without named constants or explanatory context.
- All linting and formatting rules MUST pass before a PR is considered ready for review.
- Code reviews MUST explicitly verify clarity and maintainability, not just correctness.

**Rationale**: Agentic workflows extend this codebase repeatedly. Degraded readability
compounds across worktrees and agents; quality gates prevent accumulation of technical debt.

### II. Test-Driven Development (NON-NEGOTIABLE)

Tests MUST be written before implementation code for all non-trivial logic:

- The Red-Green-Refactor cycle is mandatory: write a failing test → confirm it fails →
	implement the minimum code to pass → refactor.
- Tests MUST be reviewed and approved before implementation begins.
- Unit tests are required for all service-layer logic (backend: `TaskService`; frontend:
	component behaviour and state transitions).
- Integration tests are required for API contract changes, new endpoints, and
	cross-layer interactions (e.g., controller → service → database).
- Frontend tests MUST cover: component rendering, user interactions, loading states,
	and error states using React Testing Library.
- A feature MUST NOT be considered complete until all its associated tests pass and
	coverage for the changed code is ≥ 80%.

**Rationale**: TDD prevents regressions during iterative extension by agents and human
developers alike, and produces a living specification of expected behaviour.

### III. Clean Documentation

Documentation MUST be accurate, concise, and co-located with the code it describes:

- Public API endpoints MUST be documented via OpenAPI/Swagger annotations in the
	controller layer. Documentation MUST stay in sync with actual behaviour.
- Architecture decisions that deviate from the existing structure MUST be recorded in
	`docs/architecture.md` before implementation begins.
- README files MUST reflect the current state of the project (prerequisites, setup
	commands, test commands). Outdated instructions MUST be corrected in the same PR
	that changes the behaviour.
- Inline comments are reserved for non-obvious logic only. Self-explanatory code MUST
	NOT be commented; verbose unnecessary comments MUST be removed.
- Feature specs (`specs/*/spec.md`) and plans (`specs/*/plan.md`) are first-class
	documentation artifacts and MUST be kept up to date throughout implementation.

**Rationale**: Agents and developers depend on documentation to operate safely on this
codebase. Stale or absent documentation causes incorrect assumptions and cascading errors.

### IV. UX Consistency

All user-facing UI and API responses MUST follow consistent patterns:

- UI components MUST share a consistent visual language: spacing, typography, colour,
	and interactive state (loading, error, empty, success) patterns MUST match across
	all pages and components.
- Loading and error states MUST always be handled explicitly; a component MUST NEVER
	silently fail or render in an indeterminate state.
- API responses MUST use consistent shapes: all list endpoints return arrays, all error
	responses return `{ message: string }`, HTTP status codes MUST follow REST conventions.
- Form validation feedback MUST be immediate and human-readable; generic error messages
	MUST NOT be exposed directly to users.
- New UI patterns MUST reuse existing components before introducing new ones. Novel
	interaction patterns require explicit justification in the feature spec.

**Rationale**: A consistent UX reduces cognitive load for users and simplifies testing.
Consistent API contracts simplify client integration and agent-driven extension.

### V. Simplicity

Complexity MUST be justified; the simplest solution that meets requirements is preferred:

- YAGNI (You Aren't Gonna Need It) applies. Features, abstractions, and infrastructure
	MUST NOT be added speculatively.
- New dependencies require explicit justification in the feature plan. Prefer standard
	library or already-present dependencies over new ones.
- Abstractions MUST NOT be introduced for single use cases. Extract only when a second
	concrete use exists.
- The data model MUST remain as flat and direct as possible; premature normalization or
	over-engineering of the schema is prohibited.

**Rationale**: This codebase is a demonstration starter. Unnecessary complexity obscures
intent and creates risk during agentic extension.

## Quality Gates

All pull requests and agent-generated changesets MUST satisfy the following gates before
merge:

- **Linting**: `dotnet format` (server) and ESLint (client) MUST report zero errors.
- **Tests pass**: `dotnet test TeamTasks.slnx` and `pnpm test` (client) MUST pass with
	zero failures.
- **Coverage**: Changed code MUST maintain ≥ 80% line coverage.
- **Documentation sync**: Swagger/OpenAPI annotations, README, and `docs/architecture.md`
	MUST reflect any changes made in the PR.
- **Constitution Check**: The plan.md for any feature MUST include a completed
	Constitution Check section that explicitly verifies alignment with each principle
	above before implementation begins.
- **No console errors**: Frontend MUST render without browser console errors or warnings
	in the primary happy-path flows.

## Development Workflow

Feature development MUST follow the speckit workflow:

1. **Specify** (`speckit.specify`): Capture user stories, acceptance scenarios, and
	 requirements in `specs/[###-feature-name]/spec.md`.
2. **Plan** (`speckit.plan`): Produce research notes, data model, API contracts, and
	 implementation plan. Complete the Constitution Check gate.
3. **Tasks** (`speckit.tasks`): Generate a dependency-ordered task list. Test tasks
	 MUST precede their corresponding implementation tasks.
4. **Implement** (`speckit.implement`): Execute tasks in order. Write and confirm failing
	 tests before writing implementation code (see Principle II).
5. **Review**: Verify all Quality Gates pass. Update documentation as required (see
	 Principle III).

Branch naming: `[###-feature-name]` (e.g., `042-task-filtering`).

## Governance

This constitution supersedes all other development practices for the Team Tasks project.

- Amendments MUST be documented with a version bump per the semantic versioning policy:
	- **MAJOR**: Removal or incompatible redefinition of an existing principle.
	- **MINOR**: New principle or section added; material expansion of existing guidance.
	- **PATCH**: Clarifications, wording improvements, or typo corrections.
- Any PR that conflicts with this constitution MUST either be revised to comply or
	include a constitution amendment in the same changeset with documented justification.
- Complexity exceptions MUST be recorded in the plan.md Complexity Tracking table with
	rationale and rejected simpler alternatives.
- All agents operating on this repository MUST load this constitution before beginning
	feature planning or implementation.
- Compliance reviews are expected at the start of each feature plan (Constitution Check
	gate) and before merging any PR.

**Version**: 1.0.0 | **Ratified**: 2026-03-25 | **Last Amended**: 2026-03-25
