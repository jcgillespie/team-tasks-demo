import { useCallback, useEffect, useState } from 'react'
import { createTask, getTasks, toggleTask } from '../api/tasksApi'
import { TaskForm } from '../components/TaskForm'
import { TaskList } from '../components/TaskList'
import type { CreateTaskInput, TaskItem } from '../types/task'

export function TasksPage() {
  const [tasks, setTasks] = useState<TaskItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const loadTasks = useCallback(async () => {
    setIsLoading(true)
    setError(null)

    try {
      const loaded = await getTasks()
      setTasks(loaded)
    } catch {
      setError('Could not load tasks.')
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    void loadTasks()
  }, [loadTasks])

  async function handleCreate(input: CreateTaskInput) {
    const created = await createTask(input)
    setTasks((previous) => [created, ...previous])
  }

  async function handleToggle(id: number) {
    try {
      const updated = await toggleTask(id)
      setTasks((previous) => previous.map((task) => (task.id === id ? updated : task)))
    } catch {
      setError('Could not update task status.')
    }
  }

  return (
    <main>
      <header className="page-header">
        <h1>Team Tasks</h1>
        <p>Keep track of what the team is shipping next.</p>
      </header>

      <section className="layout-grid">
        <aside className="panel">
          <TaskForm onCreate={handleCreate} />
        </aside>

        <section className="panel">
          <h2>Tasks</h2>
          {isLoading ? <p className="status">Loading tasks...</p> : null}
          {error ? <p className="error-message">{error}</p> : null}
          {!isLoading && !error ? <TaskList tasks={tasks} onToggle={handleToggle} /> : null}
        </section>
      </section>
    </main>
  )
}
