import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

/**
 * TC-6.01 — Non-OK HTTP — client adapter rejects with message that includes status.
 * Spec §6.2; behavioral suite references observable error message shape.
 */
describe('TC-6.01', () => {
  beforeEach(() => {
    vi.resetModules()
  })

  afterEach(() => {
    vi.unstubAllGlobals()
  })

  it('getTasks rejects with Request failed and 404 in message', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue(new Response(null, { status: 404, statusText: 'Not Found' })),
    )
    const { getTasks } = await import('../../../../client/src/api/tasksApi.ts')
    await expect(getTasks()).rejects.toThrow(/Request failed:\s*404/)
  })

  it('getTasks rejects with Request failed and 500 in message', async () => {
    vi.resetModules()
    vi.stubGlobal(
      'fetch',
      vi.fn().mockResolvedValue(new Response(null, { status: 500, statusText: 'Error' })),
    )
    const { getTasks } = await import('../../../../client/src/api/tasksApi.ts')
    await expect(getTasks()).rejects.toThrow(/Request failed:\s*500/)
  })
})
