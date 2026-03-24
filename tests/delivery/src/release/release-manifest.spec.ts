import fs from 'node:fs'
import path from 'node:path'
import { describe, expect, it } from 'vitest'

const repoRoot = path.resolve(process.cwd(), '..', '..')

describe('Release manifest tooling', () => {
  it('contains manifest and checksum scripts', () => {
    expect(fs.existsSync(path.join(repoRoot, 'scripts/release/create-manifest.sh'))).toBe(true)
    expect(fs.existsSync(path.join(repoRoot, 'scripts/release/verify-checksums.sh'))).toBe(true)
  })
})
