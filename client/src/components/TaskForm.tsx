import { useState } from 'react'
import type { FormEvent } from 'react'
import type { CreateTaskInput } from '../types/task'

type TaskFormProps = {
  onCreate: (input: CreateTaskInput) => Promise<void>
}

export function TaskForm({ onCreate }: TaskFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!title.trim()) {
      setError('Title is required.')
      return
    }

    setError(null)
    setIsSubmitting(true)

    try {
      await onCreate({
        title: title.trim(),
        description: description.trim() || undefined,
      })
      setTitle('')
      setDescription('')
    } catch {
      setError('Could not create task. Please try again.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <form className="task-form" onSubmit={handleSubmit}>
      <h2>Create Task</h2>
      <label htmlFor="title">
        Title
        <input
          id="title"
          name="title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          disabled={isSubmitting}
          required
        />
      </label>

      <label htmlFor="description">
        Description
        <textarea
          id="description"
          name="description"
          rows={4}
          value={description}
          onChange={(event) => setDescription(event.target.value)}
          disabled={isSubmitting}
        />
      </label>

      {error ? <p className="error-message">{error}</p> : null}

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? 'Creating...' : 'Add Task'}
      </button>
    </form>
  )
}
