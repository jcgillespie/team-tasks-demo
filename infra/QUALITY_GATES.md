# Quality Gates Authoring Guide

This guide explains how to author, configure, and maintain quality gate profiles for Team Tasks environment promotions.

## Overview

Quality gates are YAML configuration files in `infra/quality-gates/` that define the checks that must pass before an artifact can be promoted from one environment to the next.

Gates are evaluated by `scripts/deploy/evaluate-quality-gates.sh` during the promotion workflow.

## Gate Profile Structure

```yaml
profile: <name>
transition:
  source: <environment>
  target: <environment>

gates:
  - name: <gate-identifier>
    description: "Human-readable description of what this gate checks."
    blocking: true|false
    evidence_type: <type>
    evidence_ref: "<reference>"
```

### Fields

| Field | Required | Description |
|-------|----------|-------------|
| `profile` | Yes | Unique identifier for this gate set. Must match the filename (without `.yaml`). |
| `transition.source` | Yes | Source environment for this promotion boundary. |
| `transition.target` | Yes | Target environment for this promotion boundary. |
| `gates[].name` | Yes | Unique gate identifier within the profile. Used in failure messages. |
| `gates[].description` | Yes | Plain-language description of what the gate verifies. |
| `gates[].blocking` | Yes | If `true`, a failed gate blocks promotion. If `false`, failure is reported but does not block. |
| `gates[].evidence_type` | Yes | Type of evidence checked (see below). |
| `gates[].evidence_ref` | Yes | Reference to the evidence source (format depends on `evidence_type`). |

## Supported Evidence Types

| `evidence_type` | `evidence_ref` format | What it checks |
|----------------|----------------------|----------------|
| `github_check` | `"<workflow-name> / <job-name>"` | GitHub check run status for the artifact's source SHA |
| `github_workflow` | `"<workflow-name>"` | GitHub workflow run status on main branch |
| `kubernetes_rollout` | `"<namespace>/<deployment>,..."` | Kubernetes deployment rollout status |
| `http_probe` | `"http://..."` | HTTP GET returns 2xx |
| `quality_gate_result` | `"<profile-name>"` | Result of a previously evaluated gate profile |
| `github_environment_approval` | `"<environment-name>"` | GitHub environment protection approval |

## Current Gate Profiles

| Profile | Transition | Gates |
|---------|-----------|-------|
| `dev-to-stage` | development → stage | validation-passed, frontend-tests-passed, build-succeeded, dev-deployment-healthy |
| `stage-to-production` | stage → production | stage-deployment-healthy, stage-api-health-check, dev-to-stage-gates-passed, approval |

## Adding a New Gate

1. Edit the relevant profile YAML in `infra/quality-gates/`
2. Add the new gate entry with a unique `name` and appropriate `evidence_type`
3. Set `blocking: true` for gates that must pass before promotion is allowed
4. Test the gate evaluation locally:
   ```bash
   bash scripts/deploy/evaluate-quality-gates.sh dev-to-stage <artifact-id> development
   ```

## Removing a Gate

1. Remove the gate entry from the profile YAML
2. Never leave unused gate names — stale gates can confuse operators

## Non-Blocking Gates

Set `blocking: false` for informational checks that should be surfaced but shouldn't stop a promotion. Examples: optional coverage targets, performance benchmarks under investigation.

## Adding a New Transition Profile

1. Create `infra/quality-gates/<transition-name>.yaml` following the structure above
2. Reference it in the relevant promotion workflow (`promote-to-stage.yml` or similar) via `quality_gate_profile` input
3. Document the new profile in this guide
