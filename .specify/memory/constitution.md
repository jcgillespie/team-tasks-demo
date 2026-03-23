<!--
Sync Impact Report
- Version change: template-initialized (unversioned) -> 1.0.0
- Modified principles:
	- Principle 1 placeholder -> I. Code Quality Is Mandatory
	- Principle 2 placeholder -> II. Test-Driven Development (NON-NEGOTIABLE)
	- Principle 3 placeholder -> III. Documentation Is Part Of Done
	- Principle 4 placeholder -> IV. UX Consistency Is Required
	- Principle 5 placeholder -> V. Keep Solutions Simple And Maintainable
- Added sections:
	- Engineering Standards
	- Delivery Workflow & Quality Gates
- Removed sections:
	- None
- Templates requiring updates:
	- ✅ .specify/templates/plan-template.md
	- ✅ .specify/templates/spec-template.md
	- ✅ .specify/templates/tasks-template.md
	- ⚠ pending: .specify/templates/commands/*.md (directory not present in this repository)
- Follow-up TODOs:
	- None
-->

# Team Tasks Constitution

## Core Principles

### I. Code Quality Is Mandatory
All production code MUST be readable, typed where supported, and reviewed for
correctness and maintainability. Every change MUST pass linting, formatting,
build, and static analysis checks configured for the touched stack. Complex
logic MUST include concise intent-focused comments when the code alone is not
self-explanatory. Rationale: defects prevented early are cheaper than defects
found in runtime or after release.

### II. Test-Driven Development (NON-NEGOTIABLE)
All behavior changes MUST follow Red-Green-Refactor: write tests first, observe
failure, implement minimal code to pass, then refactor safely. Unit tests are
required for domain logic, and integration or contract tests are required when
API contracts, persistence behavior, or cross-boundary interactions change.
Rationale: TDD keeps scope focused and reduces regression risk.

### III. Documentation Is Part Of Done
Documentation MUST be updated in the same change set when behavior, architecture,
setup, APIs, or user workflows change. At minimum, relevant updates MUST be made
to feature specs, quickstart guidance, architecture notes, or API contract docs.
Rationale: current documentation is required for reliable onboarding and safe
handoffs across contributors.

### IV. UX Consistency Is Required
UI changes MUST align with established interaction patterns, visual hierarchy,
terminology, and feedback behavior across the application. New UI MUST include
loading, empty, and error states, and MUST preserve keyboard accessibility and
legible contrast. Rationale: consistency lowers cognitive load and prevents
fragmented user experiences.

### V. Keep Solutions Simple And Maintainable
Changes MUST prefer the simplest design that satisfies current requirements.
Premature abstraction, speculative frameworks, and unnecessary dependencies are
prohibited unless justified in the implementation plan. Rationale: simpler code
accelerates review, testing, and future iteration.

## Engineering Standards

- The canonical stack is ASP.NET Core Web API + React/TypeScript; deviations
	MUST be justified in the implementation plan.
- Public API changes MUST include explicit contract and compatibility notes.
- New dependencies MUST include a short justification and maintenance impact.
- Every change MUST leave the repository in a runnable state locally.

## Delivery Workflow & Quality Gates

- Specs MUST define independently testable user stories with measurable success
	criteria before implementation begins.
- Plans MUST document how each constitutional principle is satisfied.
- Tasks MUST include test-first sequencing and explicit documentation updates.
- Pull requests MUST include evidence of test execution and a summary of UX
	verification for UI-facing changes.

## Governance

This constitution overrides informal team practices for planning, implementation,
and review.

- Amendment process: changes require a documented proposal, reviewer approval,
	and updates to impacted templates before ratification.
- Versioning policy (semantic versioning):
	- MAJOR for incompatible principle removals or redefinitions.
	- MINOR for added principles or materially expanded guidance.
	- PATCH for clarifications, wording refinements, or typo fixes.
- Compliance review expectations: each plan, spec, task list, and pull request
	MUST include an explicit constitution compliance check.

**Version**: 1.0.0 | **Ratified**: 2026-03-23 | **Last Amended**: 2026-03-23
