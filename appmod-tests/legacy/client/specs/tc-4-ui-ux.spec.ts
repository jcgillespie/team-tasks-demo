import { expect, test } from '@playwright/test'
import { tasksApiUrlPredicate } from './_tasksApiRoute'

/**
 * TC-4.01 — Loading then list (or empty state).
 * Legacy UI uses three ASCII dots in "Loading tasks..." (spec uses ellipsis character; both match /Loading tasks/).
 */
test('TC-4.01 — shows loading then settles', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    if (route.request().method() !== 'GET') {
      await route.continue()
      return
    }
    await new Promise((r) => setTimeout(r, 250))
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: '[]',
    })
  })

  await page.goto('/')
  await expect(page.getByText(/Loading tasks/)).toBeVisible()
  await expect(page.getByText('No tasks yet. Create your first one.')).toBeVisible()
})

/**
 * TC-4.02 — Create flow — new task appears after POST success.
 */
test('TC-4.02 — creates a task from the form', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    if (route.request().method() === 'GET') {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: '[]',
      })
      return
    }
    if (route.request().method() === 'POST') {
      const body = route.request().postDataJSON() as { title: string; description?: string }
      await route.fulfill({
        status: 201,
        contentType: 'application/json',
        body: JSON.stringify({
          id: 9900402,
          title: body.title,
          description: body.description ?? null,
          isCompleted: false,
          createdAt: new Date().toISOString(),
        }),
      })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await page.getByLabel('Title').fill('Playwright TC-4.02')
  await page.getByLabel('Description').fill('desc')
  await page.getByRole('button', { name: 'Add Task' }).click()

  await expect(page.getByText('Playwright TC-4.02')).toBeVisible()
})

/**
 * TC-4.03 — Toggle labels — complete then incomplete.
 */
test('TC-4.03 — toggles completion labels', async ({ page }) => {
  const payload = [
    {
      id: 990403,
      title: 'Toggle me',
      description: null,
      isCompleted: false,
      createdAt: '2026-04-17T12:00:00.000Z',
    },
  ]

  await page.route(tasksApiUrlPredicate, async (route) => {
    const m = route.request().method()
    const u = route.request().url()
    if (m === 'GET' && u.includes('/api/tasks') && !u.includes('/toggle')) {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(payload),
      })
      return
    }
    if (m === 'PATCH' && u.includes('/toggle')) {
      payload[0].isCompleted = !payload[0].isCompleted
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(payload[0]),
      })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  const completeBtn = page.getByRole('button', { name: 'Mark as Complete' })
  await completeBtn.click()
  await expect(page.getByRole('button', { name: 'Mark as Incomplete' })).toBeVisible()
  await page.getByRole('button', { name: 'Mark as Incomplete' }).click()
  await expect(page.getByRole('button', { name: 'Mark as Complete' })).toBeVisible()
})

/**
 * TC-4.04 — Load error message.
 */
test('TC-4.04 — load error shows fixed message', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    if (route.request().method() === 'GET') {
      await route.fulfill({ status: 500, body: 'server error' })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await expect(page.getByText('Could not load tasks.')).toBeVisible()
})

/**
 * TC-4.05 — Empty list copy.
 */
test('TC-4.05 — empty API list shows empty state copy', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    if (route.request().method() === 'GET') {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: '[]',
      })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await expect(page.getByText('No tasks yet. Create your first one.')).toBeVisible()
})

/**
 * TC-4.06 — Toggle error hides list.
 */
test('TC-4.06 — toggle error shows message and hides task list', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    const m = route.request().method()
    const u = route.request().url()
    if (m === 'GET' && !u.includes('/toggle')) {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          {
            id: 990406,
            title: 'Err toggle',
            description: null,
            isCompleted: false,
            createdAt: '2026-04-17T12:00:00.000Z',
          },
        ]),
      })
      return
    }
    if (m === 'PATCH' && u.includes('/toggle')) {
      await route.fulfill({ status: 500, body: 'no' })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await page.getByRole('button', { name: 'Mark as Complete' }).click()

  await expect(page.getByText('Could not update task status.')).toBeVisible()
  await expect(page.getByRole('button', { name: 'Mark as Complete' })).not.toBeVisible()
})
