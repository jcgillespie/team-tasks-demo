import App from '@client/App'
import { render, screen, waitFor } from '@testing-library/react'
import { afterEach, describe, expect, it, vi } from 'vitest'

/** TC-6.04 — No automatic second fetch after initial load failure (team-tasks-test-suite.md). */
describe('TC-6.04', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  it('does not retry the initial load after failure', async () => {
    const fetchMock = vi.fn(() => Promise.resolve(new Response('', { status: 500 })))
    vi.stubGlobal('fetch', fetchMock)

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('Could not load tasks.')).toBeInTheDocument()
    })

    expect(fetchMock).toHaveBeenCalledTimes(1)
  })
})
