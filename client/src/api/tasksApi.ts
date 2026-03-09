import type { CreateTaskInput, TaskItem } from '../types/task'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5276'

async function parseJsonOrThrow<T>(response: Response): Promise<T> {
  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`)
  }

  return (await response.json()) as T
}

export async function getTasks(): Promise<TaskItem[]> {
  const response = await fetch(`${API_BASE_URL}/api/tasks`)
  return parseJsonOrThrow<TaskItem[]>(response)
}

export async function createTask(input: CreateTaskInput): Promise<TaskItem> {
  const response = await fetch(`${API_BASE_URL}/api/tasks`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(input),
  })

  return parseJsonOrThrow<TaskItem>(response)
}

export async function toggleTask(id: number): Promise<TaskItem> {
  const response = await fetch(`${API_BASE_URL}/api/tasks/${id}/toggle`, {
    method: 'PATCH',
  })

  return parseJsonOrThrow<TaskItem>(response)
}
