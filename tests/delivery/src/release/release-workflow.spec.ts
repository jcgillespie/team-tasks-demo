import { describe, expect, it } from 'vitest'
import { fixtureCatalog } from '../shared/fixtureCatalog'
import { getWorkflowTriggers, readYamlFile } from '../shared/workflowTestUtils'

describe('Release promotion workflow', () => {
  it('supports push and manual triggers with staged promotion and rollback', () => {
    const workflow = readYamlFile<Record<string, unknown>>(fixtureCatalog.workflows.release)
    const triggers = getWorkflowTriggers(workflow)
    const serialized = JSON.stringify(workflow)

    expect(triggers).toContain('push')
    expect(triggers).toContain('workflow_dispatch')
    expect(serialized).toContain('create-manifest.sh')
    expect(serialized).toContain('deploy-webapp.sh')
    expect(serialized).toContain('rollback-webapp.sh')
    expect(serialized).toContain('environment":"staging')
    expect(serialized).toContain('environment":"prod')
  })
})
