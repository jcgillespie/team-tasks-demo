export interface GitHubContext {
  ref: string
  sha: string
  repository: string
  actor: string
  eventName: string
}

export function createDefaultGitHubContext(overrides: Partial<GitHubContext> = {}): GitHubContext {
  return {
    ref: 'refs/heads/main',
    sha: '0000000000000000000000000000000000000000',
    repository: 'jcgillespie/team-tasks-demo',
    actor: 'octocat',
    eventName: 'workflow_dispatch',
    ...overrides,
  }
}
