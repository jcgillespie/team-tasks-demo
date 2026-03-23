import path from 'node:path'

const repoRoot = path.resolve(process.cwd(), '..', '..')

export const fixtureCatalog = {
  workflows: {
    ci: path.join(repoRoot, '.github/workflows/ci.yml'),
    infraPlan: path.join(repoRoot, '.github/workflows/infra-plan.yml'),
    infraApply: path.join(repoRoot, '.github/workflows/infra-apply.yml'),
    driftDetection: path.join(repoRoot, '.github/workflows/drift-detection.yml'),
    release: path.join(repoRoot, '.github/workflows/release.yml'),
  },
  infra: {
    root: path.join(repoRoot, 'infra/opentofu'),
    environments: {
      dev: path.join(repoRoot, 'infra/opentofu/environments/dev'),
      staging: path.join(repoRoot, 'infra/opentofu/environments/staging'),
      prod: path.join(repoRoot, 'infra/opentofu/environments/prod'),
    },
  },
  scripts: {
    releaseRoot: path.join(repoRoot, 'scripts/release'),
    common: path.join(repoRoot, 'scripts/release/common.sh'),
    smokeTest: path.join(repoRoot, 'scripts/release/smoke-test.sh'),
  },
} as const

export type FixtureCatalog = typeof fixtureCatalog
