import fs from 'node:fs'
import path from 'node:path'
import { describe, expect, it } from 'vitest'

const repoRoot = path.resolve(process.cwd(), '..', '..')
const envRoot = path.join(repoRoot, 'infra/opentofu/environments')

describe('OpenTofu environment roots', () => {
  const environments = ['dev', 'staging', 'prod']

  for (const env of environments) {
    it(`${env} root contains required files`, () => {
      const files = ['main.tf', 'variables.tf', 'outputs.tf', 'backend.hcl', `${env}.tfvars`]
      for (const file of files) {
        expect(fs.existsSync(path.join(envRoot, env, file))).toBe(true)
      }
    })
  }

  it('environment roots reference shared modules', () => {
    for (const env of environments) {
      const mainTf = fs.readFileSync(path.join(envRoot, env, 'main.tf'), 'utf8')
      expect(mainTf).toContain('module "platform_baseline"')
      expect(mainTf).toContain('module "app_service_stack"')
      expect(mainTf).toContain('module "data_protection"')
    }
  })
})
