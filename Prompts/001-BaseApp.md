You are a senior full-stack engineer. Build a clean, production-style starter application for a future demo of AI agentic workflows using Git worktrees.

Goal:
Create a small but solid “Team Tasks” application with:

- Backend: latest stable .NET LTS (use ASP.NET Core on .NET 10)
- Frontend: React + Vite
- Database: SQLite
- Standard automated testing
- Clear project structure
- Clean developer experience
- Simple baseline functionality only

This is the BASE app before any later multi-agent enhancements. Do not add advanced features like tags, due dates, filtering, auth, or multi-user support yet.

Functional requirements:

1. Users can view a list of tasks
2. Users can create a task with:
   - title (required)
   - optional description
3. Users can mark a task complete / incomplete
4. Tasks should persist in SQLite
5. The UI should be simple, clean, and reliable

Architecture requirements:

- Monorepo layout
- Separate frontend and backend folders
- Prefer a structure like:
  /app
  /server
  /client
  /tests
  /docs
  /scripts

Backend requirements:

- Use ASP.NET Core Web API on .NET 10
- Use Entity Framework Core with SQLite
- Use a minimal but clean architecture appropriate for a demo:
  - Entities / Models
  - DbContext
  - Services or application layer if useful
  - Controllers or minimal APIs, but choose the approach that is clearest for a demo
- Add CORS configured for the frontend dev server
- Include Swagger / OpenAPI
- Add basic validation for requests
- Include configuration via appsettings where appropriate
- Use async patterns correctly
- Seed a few sample tasks on first run

Frontend requirements:

- Use React with Vite
- Use TypeScript
- Keep styling simple and professional
- No heavy design system unless truly necessary
- Prefer lightweight CSS or CSS modules
- App should include:
  - page header
  - task list
  - create-task form
  - complete/incomplete toggle
  - loading and error states
- Organize code cleanly:
  - components
  - api client
  - types
  - pages or feature folders
- Keep frontend state management simple; do not add Redux unless necessary

Testing requirements:
Backend:

- xUnit test project
- Cover:
  - task creation validation
  - task retrieval
  - complete/incomplete toggle logic
- Use an appropriate test setup; in-memory provider is acceptable for logic tests, but keep overall testing realistic

Frontend:

- Vitest + React Testing Library
- Cover:
  - rendering the task list
  - creating a task
  - toggling completion
  - basic loading/error state behavior
- Use `pnpm` rather than `npm` for package management

Optional but preferred:

- One lightweight end-to-end smoke test using Playwright

Non-functional requirements:

- Code should be straightforward and demo-friendly
- Avoid overengineering
- Use sensible naming
- Add comments only where they help
- Make the app easy for later agents to extend
- Keep dependencies reasonable
- Favor clarity over cleverness

API requirements:
Implement a small REST API such as:

- GET /api/tasks
- POST /api/tasks
- PATCH /api/tasks/{id}/toggle

Data model:
Task

- id
- title
- description
- isCompleted
- createdAt

Deliverables:

1. Full project scaffold
2. Working backend and frontend
3. SQLite persistence configured
4. Tests passing
5. README with:
   - prerequisites
   - how to run backend
   - how to run frontend
   - how to run tests
   - brief architecture summary
6. scripts or commands for local setup if useful

Implementation constraints:

- Do not add authentication
- Do not add Docker unless it materially improves the starter
- Do not add Kubernetes, messaging, caching, or cloud infrastructure
- Do not add CQRS/event sourcing unless there is a very strong reason
- Do not add advanced UI libraries unless necessary
- Do not invent unnecessary abstractions

Quality bar:

- The app should run locally without drama
- Tests should be green
- The codebase should feel like a clean starting point for later worktree-based agent tasks

Output format:

- First show the proposed folder structure
- Then generate the code files
- Then provide run/test commands
- Then provide a short explanation of design choices
