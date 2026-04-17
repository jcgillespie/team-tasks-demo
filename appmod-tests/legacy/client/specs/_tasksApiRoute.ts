/** Match browser calls to the Team Tasks API (any path under /api/tasks on local dev hosts). */
export function tasksApiUrlPredicate(url: URL): boolean {
  const hostOk = url.hostname === 'localhost' || url.hostname === '127.0.0.1'
  return hostOk && url.pathname.startsWith('/api/tasks')
}
