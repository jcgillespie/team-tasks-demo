import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import App from './App'
import * as tasksApi from './api/tasksApi'
import type { TaskItem } from './types/task'

vi.mock('./api/tasksApi')

const mockedApi = vi.mocked(tasksApi)

const sampleTasks: TaskItem[] = [
  {
    id: 1,
    title: 'Write docs',
    description: 'README updates',
    isCompleted: false,
    createdAt: new Date('2026-03-09T08:00:00Z').toISOString(),
  },
]

describe('App', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('shows loading then renders task list', async () => {
    mockedApi.getTasks.mockResolvedValue(sampleTasks)

    render(<App />)

    expect(screen.getByText('Loading tasks...')).toBeInTheDocument()
    expect(await screen.findByText('Write docs')).toBeInTheDocument()
  })

  it('creates a task from the form', async () => {
    mockedApi.getTasks.mockResolvedValue([])
    mockedApi.createTask.mockResolvedValue({
      id: 2,
      title: 'New task',
      description: 'From test',
      isCompleted: false,
      createdAt: new Date().toISOString(),
    })

    render(<App />)

    await screen.findByText('No tasks yet. Create your first one.')

    await userEvent.type(screen.getByLabelText('Title'), 'New task')
    await userEvent.type(screen.getByLabelText('Description'), 'From test')
    await userEvent.click(screen.getByRole('button', { name: 'Add Task' }))

    await waitFor(() => {
      expect(mockedApi.createTask).toHaveBeenCalledWith({
        title: 'New task',
        description: 'From test',
      })
    })

    expect(await screen.findByText('New task')).toBeInTheDocument()
  })

  it('toggles completion status', async () => {
    mockedApi.getTasks.mockResolvedValue(sampleTasks)
    mockedApi.toggleTask.mockResolvedValue({
      ...sampleTasks[0],
      isCompleted: true,
    })

    render(<App />)
    await screen.findByText('Write docs')

    await userEvent.click(screen.getByRole('button', { name: 'Mark as Complete' }))

    await waitFor(() => {
      expect(mockedApi.toggleTask).toHaveBeenCalledWith(1)
    })

    expect(screen.getByRole('button', { name: 'Mark as Incomplete' })).toBeInTheDocument()
  })

  it('shows error state when loading fails', async () => {
    mockedApi.getTasks.mockRejectedValue(new Error('network'))

    render(<App />)

    expect(await screen.findByText('Could not load tasks.')).toBeInTheDocument()
  })
})
