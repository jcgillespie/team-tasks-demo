import type { TaskItem } from '../types/task'

type TaskListProps = {
  tasks: TaskItem[]
  onToggle: (id: number) => Promise<void>
}

export function TaskList({ tasks, onToggle }: TaskListProps) {
  if (tasks.length === 0) {
    return <p className="empty-state">No tasks yet. Create your first one.</p>
  }

  return (
    <ul className="task-list">
      {tasks.map((task) => (
        <li key={task.id} className={`task-item ${task.isCompleted ? 'completed' : ''}`}>
          <div>
            <h3>{task.title}</h3>
            {task.description ? <p>{task.description}</p> : null}
            <div className="meta">Created {new Date(task.createdAt).toLocaleString()}</div>
          </div>
          <button type="button" onClick={() => void onToggle(task.id)}>
            Mark as {task.isCompleted ? 'Incomplete' : 'Complete'}
          </button>
        </li>
      ))}
    </ul>
  )
}
