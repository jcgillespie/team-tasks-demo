import { describe, expect, it } from 'vitest'
import { fixtureCatalog } from '../shared/fixtureCatalog'
import { getWorkflowTriggers, readYamlFile } from '../shared/workflowTestUtils'

describe('PR CI workflow', () => {
  it('runs on pull requests and executes quality gates', () => {
    const workflow = readYamlFile<Record<string, unknown>>(fixtureCatalog.workflows.ci)
    const triggers = getWorkflowTriggers(workflow)

    expect(triggers).toContain('pull_request')

    const serialized = JSON.stringify(workflow)
    expect(serialized).toContain('pnpm lint')
    expect(serialized).toContain('pnpm build')
    expect(serialized).toContain('pnpm test')
    expect(serialized).toContain('dotnet build TeamTasks.slnx')
    expect(serialized).toContain('dotnet test TeamTasks.slnx')
  })
})
