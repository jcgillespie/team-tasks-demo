import App from '@client/App'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { afterEach, describe, expect, it, vi } from 'vitest'

const sampleTask = {
  id: 42,
  title: 'Alpha Task',
  description: null as string | null,
  isCompleted: false,
  createdAt: new Date().toISOString(),
}

/** TC-4 — UI/UX (team-tasks-test-suite.md). */
describe('TC-4', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
    vi.restoreAllMocks()
  })

  /** TC-4.01 — Loading then list. */
  it('TC-4.01 shows loading then task titles', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn(() =>
        Promise.resolve(
          new Response(JSON.stringify([sampleTask]), {
            status: 200,
            headers: { 'Content-Type': 'application/json' },
          }),
        ),
      ),
    )

    render(<App />)

    expect(screen.getByText(/loading tasks/i)).toBeInTheDocument()
    await waitFor(() => {
      expect(screen.getByText('Alpha Task')).toBeInTheDocument()
    })
  })

  /** TC-4.02 — Create flow: new task appears in list. */
  it('TC-4.02 creates a task from the form', async () => {
    const user = userEvent.setup()
    let serverTasks: typeof sampleTask[] = []

    vi.stubGlobal(
      'fetch',
      vi.fn((input: RequestInfo | URL, init?: RequestInit) => {
        const url = typeof input === 'string' ? input : input.toString()
        if (init?.method === 'POST' && url.includes('/api/tasks')) {
          const body = JSON.parse(init.body as string) as { title: string; description?: string }
          const created = {
            id: 99,
            title: body.title,
            description: body.description ?? null,
            isCompleted: false,
            createdAt: new Date().toISOString(),
          }
          serverTasks = [created]
          return Promise.resolve(
            new Response(JSON.stringify(created), {
              status: 201,
              headers: { 'Content-Type': 'application/json' },
            }),
          )
        }
        return Promise.resolve(
          new Response(JSON.stringify(serverTasks), {
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

    await user.type(screen.getByLabelText(/^Title/i), 'New from test')
    await user.click(screen.getByRole('button', { name: /add task/i }))

    await waitFor(() => {
      expect(screen.getByText('New from test')).toBeInTheDocument()
    })
  })

  /** TC-4.03 — Toggle labels. */
  it('TC-4.03 toggles completion button label', async () => {
    const user = userEvent.setup()
    let task = { ...sampleTask }

    vi.stubGlobal(
      'fetch',
      vi.fn((input: RequestInfo | URL, init?: RequestInit) => {
        const url = typeof input === 'string' ? input : input.toString()
        if (url.includes('/toggle')) {
          task = { ...task, isCompleted: !task.isCompleted }
          return Promise.resolve(
            new Response(JSON.stringify(task), {
              status: 200,
              headers: { 'Content-Type': 'application/json' },
            }),
          )
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
      expect(screen.getByRole('button', { name: /mark as complete/i })).toBeInTheDocument()
    })
    await user.click(screen.getByRole('button', { name: /mark as complete/i }))
    await waitFor(() => {
      expect(screen.getByRole('button', { name: /mark as incomplete/i })).toBeInTheDocument()
    })
  })

  /** TC-4.04 — Load error. */
  it('TC-4.04 shows Could not load tasks when list fails', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn(() => Promise.resolve(new Response('', { status: 500 }))),
    )

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('Could not load tasks.')).toBeInTheDocument()
    })
  })

  /** TC-4.05 — Empty list copy. */
  it('TC-4.05 shows empty list message', async () => {
    vi.stubGlobal(
      'fetch',
      vi.fn(() =>
        Promise.resolve(
          new Response(JSON.stringify([]), {
            status: 200,
            headers: { 'Content-Type': 'application/json' },
          }),
        ),
      ),
    )

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('No tasks yet. Create your first one.')).toBeInTheDocument()
    })
  })

  /** TC-4.06 — Toggle error hides list. */
  it('TC-4.06 shows toggle error and hides task list', async () => {
    const user = userEvent.setup()

    vi.stubGlobal(
      'fetch',
      vi.fn((input: RequestInfo | URL) => {
        const url = typeof input === 'string' ? input : input.toString()
        if (url.includes('/toggle')) {
          return Promise.resolve(new Response('', { status: 404 }))
        }
        return Promise.resolve(
          new Response(JSON.stringify([sampleTask]), {
            status: 200,
            headers: { 'Content-Type': 'application/json' },
          }),
        )
      }),
    )

    render(<App />)

    await waitFor(() => {
      expect(screen.getByText('Alpha Task')).toBeInTheDocument()
    })
    await user.click(screen.getByRole('button', { name: /mark as complete/i }))

    await waitFor(() => {
      expect(screen.getByText('Could not update task status.')).toBeInTheDocument()
    })
    expect(screen.queryByText('Alpha Task')).not.toBeInTheDocument()
  })
})
