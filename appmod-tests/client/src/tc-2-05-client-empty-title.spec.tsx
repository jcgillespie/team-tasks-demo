import { TaskForm } from '@client/components/TaskForm'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, expect, it, vi } from 'vitest'

/** TC-2.05 — Client blocks empty title (team-tasks-test-suite.md). */
describe('TC-2.05', () => {
  it('shows Title is required and does not call onCreate', async () => {
    const user = userEvent.setup()
    const onCreate = vi.fn()

    render(<TaskForm onCreate={onCreate} />)

    await user.type(screen.getByLabelText(/^Title/i), '   ')
    await user.click(screen.getByRole('button', { name: /add task/i }))

    expect(screen.getByText('Title is required.')).toBeInTheDocument()
    expect(onCreate).not.toHaveBeenCalled()
  })
})
