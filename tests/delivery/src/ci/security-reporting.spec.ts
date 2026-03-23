import fs from 'node:fs'
import path from 'node:path'
import { describe, expect, it } from 'vitest'
import { fixtureCatalog } from '../shared/fixtureCatalog'
import { readYamlFile } from '../shared/workflowTestUtils'

const repoRoot = path.resolve(process.cwd(), '..', '..')

describe('Security and reporting configuration', () => {
  it('defines dependency and secret scanning baselines', () => {
    expect(fs.existsSync(path.join(repoRoot, '.github/dependabot.yml'))).toBe(true)
    expect(fs.existsSync(path.join(repoRoot, '.github/gitleaks.toml'))).toBe(true)
  })

  it('publishes test evidence in CI workflow', () => {
    const workflow = readYamlFile<Record<string, unknown>>(fixtureCatalog.workflows.ci)
    const serialized = JSON.stringify(workflow)
    expect(serialized).toContain('publish-test-report.sh')
    expect(serialized).toContain('publish-workflow-summary.sh')
  })
})
