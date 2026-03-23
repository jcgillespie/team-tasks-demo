import { describe, expect, it } from 'vitest'
import { fixtureCatalog } from '../shared/fixtureCatalog'
import { expectJob, getWorkflowTriggers, readYamlFile } from '../shared/workflowTestUtils'

describe('Infrastructure workflows contract', () => {
  it('infra-plan validates and saves plan artifact', () => {
    const workflow = readYamlFile<Record<string, unknown>>(fixtureCatalog.workflows.infraPlan)
    const triggers = getWorkflowTriggers(workflow)

    expect(triggers).toContain('pull_request')
    expect(triggers).toContain('workflow_dispatch')

    const job = expectJob(workflow, 'validate-and-plan')
    const serialized = JSON.stringify(job)
    expect(serialized).toContain('tofu fmt -check')
    expect(serialized).toContain('tofu validate')
    expect(serialized).toContain('tofu plan')
    expect(serialized).toContain('actions/upload-artifact')
  })

  it('infra-apply consumes saved plans and requires environment', () => {
    const workflow = readYamlFile<Record<string, unknown>>(fixtureCatalog.workflows.infraApply)
    const job = expectJob(workflow, 'apply')
    const serialized = JSON.stringify(job)

    expect(serialized).toContain('actions/download-artifact')
    expect(serialized).toContain('tofu apply')
    expect(serialized).toContain('environment')
  })

  it('drift detection is scheduled and never applies', () => {
    const workflow = readYamlFile<Record<string, unknown>>(fixtureCatalog.workflows.driftDetection)
    const triggers = getWorkflowTriggers(workflow)
    const serialized = JSON.stringify(workflow)

    expect(triggers).toContain('schedule')
    expect(serialized).toContain('tofu plan')
    expect(serialized).not.toContain('tofu apply')
  })
})
