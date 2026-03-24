## Summary

- Describe what changed and why.

## Validation Evidence

- [ ] `dotnet build TeamTasks.slnx`
- [ ] `dotnet test TeamTasks.slnx`
- [ ] `cd client && pnpm lint && pnpm build && pnpm test`
- [ ] Include links or snippets of relevant workflow/test output.

## Constitution Compliance

- [ ] Code Quality gate considered
- [ ] TDD expectations followed for new behavior
- [ ] Documentation updated for changed behavior
- [ ] UX consistency reviewed for UI-facing changes
- [ ] Simplicity constraints respected

## UX Verification (UI-Facing Changes Only)

- [ ] Not applicable (no UI changes)
- [ ] Manual verification steps listed
- [ ] Screenshots or recording attached

## Operational Impact

- [ ] No infrastructure or workflow impact
- [ ] Infrastructure/workflow impact documented in ops docs
- [ ] Rollback plan documented (if release-impacting)
