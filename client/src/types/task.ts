export type TaskItem = {
  id: number
  title: string
  description: string | null
  isCompleted: boolean
  createdAt: string
}

export type CreateTaskInput = {
  title: string
  description?: string
}
