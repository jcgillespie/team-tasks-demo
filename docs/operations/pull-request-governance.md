# Pull Request Governance

## Required Checks

The following checks are required before merge:

- `ci / quality-gate`
- `infra-plan / validate-and-plan` when infrastructure files are changed

## Required Evidence

Every pull request must include:

- Command evidence for lint, build, and tests relevant to changed areas
- Constitution compliance acknowledgement
- UX verification notes for UI-facing changes

## Ownership and Approval

- CODEOWNERS reviewers are required for protected paths.
- Infrastructure and workflow changes require platform owner review.
- Production-impacting workflow changes require maintainer approval.

## Failure Handling

- Merges are blocked until required checks are green.
- Security findings must be triaged before merge.
- If exceptions are needed, document the reason and approver in the PR description.
