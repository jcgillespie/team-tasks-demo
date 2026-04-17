import { expect, test } from '@playwright/test'
import { tasksApiUrlPredicate } from './_tasksApiRoute'

/**
 * TC-6.02 — Create failure message — generic string on API failure.
 */
test('TC-6.02 — POST failure shows generic create error', async ({ page }) => {
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
      await route.fulfill({
        status: 400,
        contentType: 'application/json',
        body: JSON.stringify({ errors: { Title: ['too long'] } }),
      })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await page.getByLabel('Title').fill('valid title')
  await page.getByRole('button', { name: 'Add Task' }).click()

  await expect(page.getByText('Could not create task. Please try again.')).toBeVisible()
})

/**
 * TC-6.03 — Toggle 404 → page error string.
 */
test('TC-6.03 — toggle 404 shows update error', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    const m = route.request().method()
    const u = route.request().url()
    if (m === 'GET' && !u.includes('/toggle')) {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify([
          {
            id: 2147483000,
            title: 'Stale',
            description: null,
            isCompleted: false,
            createdAt: '2026-04-17T12:00:00.000Z',
          },
        ]),
      })
      return
    }
    if (m === 'PATCH' && u.includes('/toggle')) {
      await route.fulfill({ status: 404 })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await page.getByRole('button', { name: 'Mark as Complete' }).click()

  await expect(page.getByText('Could not update task status.')).toBeVisible()
})

/**
 * TC-6.04 — No auto-retry on initial load failure (single GET attempt for load).
 */
test('TC-6.04 — load failure does not trigger extra GET retries', async ({ page }) => {
  let getCount = 0
  await page.route(tasksApiUrlPredicate, async (route) => {
    if (route.request().method() !== 'GET') {
      await route.continue()
      return
    }
    getCount++
    await route.fulfill({ status: 500, body: 'fail' })
  })

  await page.goto('/')
  await expect(page.getByText('Could not load tasks.')).toBeVisible()
  const countAfterError = getCount

  await page.waitForTimeout(1500)
  expect(getCount).toBe(countAfterError)
})

/**
 * TC-6.05 — Server fault (5xx) — load path shows generic load error (same as TC-4.04 pattern).
 */
test('TC-6.05 — 5xx on load shows generic load failure copy', async ({ page }) => {
  await page.route(tasksApiUrlPredicate, async (route) => {
    if (route.request().method() === 'GET') {
      await route.fulfill({ status: 503, body: 'unavailable' })
      return
    }
    await route.continue()
  })

  await page.goto('/')
  await expect(page.getByText('Could not load tasks.')).toBeVisible()
})
