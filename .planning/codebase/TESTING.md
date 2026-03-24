# TESTING

## Test Stack Overview
- Backend test framework: xUnit in `tests/TeamTasks.Api.Tests/TeamTasks.Api.Tests.csproj`.
- Backend test runner/tooling: `Microsoft.NET.Test.Sdk` + `coverlet.collector`.
- Backend test data store: EF Core InMemory provider for isolation.
- Frontend test framework: Vitest in `client/package.json`.
- Frontend assertions/utilities: Testing Library + jest-dom + user-event.
- Frontend test environment: jsdom configured in `client/vite.config.ts`.

## Backend Test Structure
- Tests currently centralized in `tests/TeamTasks.Api.Tests/TaskServiceTests.cs`.
- Covered behaviors:
  - Request model validation for required title (`CreateTaskRequest_Title_IsRequired`).
  - Service retrieval ordering by `CreatedAt` descending.
  - Toggle behavior flips completion state repeatedly.
- Helper pattern:
  - `BuildContext()` creates unique InMemory database per test.
  - `Validate()` performs data annotation validation checks.

## Frontend Test Structure
- Main UI behavior tests in `client/src/App.test.tsx`.
- API module is mocked with `vi.mock('./api/tasksApi')`.
- Covered behaviors:
  - Loading -> rendered list transition.
  - Task creation form submission wiring.
  - Toggle interaction updates rendered button label.
  - Error-state rendering when initial load fails.

## Current Coverage Characteristics
- Strengths:
  - Core happy-path user actions are covered on both client and server.
  - Unit-level service logic is isolated from real DB via InMemory provider.
  - Frontend tests validate async UI transitions with `waitFor`/`findBy*`.
- Gaps:
  - No API integration tests against real HTTP pipeline.
  - No end-to-end browser tests across client + API.
  - No explicit authorization/security test scenarios.
  - No CI pipeline file in repo showing enforced test execution.

## Suggested Next Test Additions
- Add controller integration tests (WebApplicationFactory) for status codes and payload shape.
- Add service tests for create-task trimming/null-description normalization.
- Add frontend tests for create/toggle error states.
- Add smoke E2E to verify frontend-to-backend wiring in one flow.
