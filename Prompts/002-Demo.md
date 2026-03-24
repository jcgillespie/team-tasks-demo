# Demo script


SpecKit demo
CLI
* `specify —help`
* `specify init --ai copilot --script sh --here --force`

Copilot
* `/speckit.constitution Focus on code quality, TDD, documentation, and UX consistency`
* `/speckit.specify I want to add Infrastructure as Code and build/deployment pipelines with CI/CD to this application. This will enable us to ship faster with reliable, repeatable releases. It will reduce production risk through automated quality and security gates. It'll improve uptime and recovery with safer deployments and rollback plans, and it'll lower operational overhead with standardized environments`
* `/speckit.clarify 1. Define production-ready IaC for dev, test, and prod environments. 2. Define CI/CD pipelines for build, test, security checks, artifact publishing, and deployment of both the IaC and application. 3. Keep the solution reproducible, secure, and cost-aware.`
* 
```
/speckit.plan Plan for the following.
Tech Stack
 - Azure
 - OpenTofu for IaC
 - GitHub Actions for pipelines

IaC
  - Model all required resources for hosting frontend + API, networking basics, config/secrets integration, logging/monitoring, and environment-specific settings.
  - Support separate state/workspaces per environment and clear naming conventions.
  - Include modules structure, variables, outputs, and example tfvars (or equivalent).
  - Include drift detection and plan/apply workflow strategy.

Pipelines
  - CI on pull requests: lint, unit tests, build frontend, build API, API tests, dependency caching, test reports.
  - CD on main/release branches with environment promotions (dev -> staging -> prod) and approval gates.
  - Artifact versioning and retention strategy.

DevEx and governance
  - OIDC/workload identity for cloud auth (no long-lived credentials).
  - Secret management via platform secret store and CI secret references.
  - Documentation for local setup, pipeline triggers, and operational runbooks.

Constraints:
- Prioritize code quality, TDD, documentation, and UX consistency.
- Prefer incremental rollout with low blast radius.
```
* `/speckit.tasks`
* `/speckit.analyze`
* `analysis remediations + /speckit.analyze`
* `speckit.implement` yolo