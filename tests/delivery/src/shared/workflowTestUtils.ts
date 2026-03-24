import fs from 'node:fs'
import { parse as parseYaml } from 'yaml'

export function readYamlFile<T>(filePath: string): T {
  const raw = fs.readFileSync(filePath, 'utf8')
  return parseYaml(raw) as T
}

export function getWorkflowTriggers(workflow: Record<string, unknown>): string[] {
  const trigger = workflow.on
  if (Array.isArray(trigger)) {
    return trigger.map(String)
  }

  if (typeof trigger === 'string') {
    return [trigger]
  }

  if (trigger && typeof trigger === 'object') {
    return Object.keys(trigger as Record<string, unknown>)
  }

  return []
}

export function expectJob(workflow: Record<string, unknown>, jobName: string): Record<string, unknown> {
  const jobs = workflow.jobs as Record<string, unknown> | undefined
  if (!jobs || !(jobName in jobs)) {
    throw new Error(`Expected workflow job '${jobName}' to exist`)
  }

  return jobs[jobName] as Record<string, unknown>
}

export function flattenJobStepRuns(job: Record<string, unknown>): string {
  const steps = (job.steps ?? []) as Array<Record<string, unknown>>
  return steps
    .map((step) => (typeof step.run === 'string' ? step.run : ''))
    .filter(Boolean)
    .join('\n')
}
