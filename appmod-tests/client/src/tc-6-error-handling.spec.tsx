import { getTasks } from '@client/api/tasksApi'
import App from '@client/App'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { afterEach, describe, expect, it, vi } from 'vitest'

/** TC-6 — Error handling (team-tasks-test-suite.md). */
describe('TC-6', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  /** TC-6.01 — Non-OK HTTP: error message includes Request failed: and status. */
  it('TC-6.01 maps HTTP errors to thrown Error with status in message', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn(() => Promise.resolve(new Response('', { status: 500 }))),
    )

    try {
      await getTasks()
      expect.fail('getTasks should have thrown')
    } catch (e) {
      const message = e instanceof Error ? e.message : String(e)
      expect(message).toMatch(/Request failed:/)
      expect(message).toMatch(/500/)
    }
  })

  /** TC-6.02 — Create failure message. */
  it('TC-6.02 shows generic create error on API failure', async () => {
    const user = userEvent.setup()

    vi.stubGlobal(
      'fetch',
      vi.fn((input: RequestInfo | URL, init?: RequestInit) => {
        const url = typeof input === 'string' ? input : input.toString()
        if (init?.method === 'POST') {
          return Promise.resolve(new Response('', { status: 400 }))
        }
        return Promise.resolve(
          new Response(JSON.stringify([]), {
            status: 200,
            headers: { 'Content-Type': 'application/json' },
          }),
        )
      }),
    )

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('No tasks yet. Create your first one.')).toBeInTheDocument()
    })

    await user.type(screen.getByLabelText(/^Title/i), 'Valid title')
    await user.click(screen.getByRole('button', { name: /add task/i }))

    await waitFor(() => {
      expect(screen.getByText('Could not create task. Please try again.')).toBeInTheDocument()
    })
  })

  /** TC-6.03 — Toggle 404 → page error. */
  it('TC-6.03 shows Could not update task status on toggle failure', async () => {
    const user = userEvent.setup()
    const task = {
      id: 7,
      title: 'Stale',
      description: null as string | null,
      isCompleted: false,
      createdAt: new Date().toISOString(),
    }

    vi.stubGlobal(
      'fetch',
      vi.fn((input: RequestInfo | URL) => {
        const url = typeof input === 'string' ? input : input.toString()
        if (url.includes('/toggle')) {
          return Promise.resolve(new Response('', { status: 404 }))
        }
        return Promise.resolve(
          new Response(JSON.stringify([task]), {
            status: 200,
            headers: { 'Content-Type': 'application/json' },
          }),
        )
      }),
    )

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('Stale')).toBeInTheDocument()
    })
    await user.click(screen.getByRole('button', { name: /mark as complete/i }))

    await waitFor(() => {
      expect(screen.getByText('Could not update task status.')).toBeInTheDocument()
    })
  })

  /** TC-6.05 — Server fault surfaces generic load error path. */
  it('TC-6.05 surfaces load failure for 5xx on initial load', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn(() => Promise.resolve(new Response('', { status: 503 }))),
    )

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('Could not load tasks.')).toBeInTheDocument()
    })
  })
})
