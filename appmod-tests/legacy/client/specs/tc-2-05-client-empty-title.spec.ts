import { expect, test } from '@playwright/test'
import { tasksApiUrlPredicate } from './_tasksApiRoute'

/**
 * TC-2.05 — Client empty title — "Title is required."; no create HTTP call.
 */
test('TC-2.05 — whitespace-only title shows validation and does not POST', async ({
  page,
}) => {
  let postCount = 0
  await page.route(tasksApiUrlPredicate, async (route) => {
    const m = route.request().method()
    if (m === 'GET') {
      await route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: '[]',
      })
      return
    }
    if (m === 'POST') {
      postCount++
    }
    await route.fulfill({ status: 201, body: '{}', contentType: 'application/json' })
  })

  await page.goto('/')
  await page.getByLabel('Title').fill('   ')
  await page.getByRole('button', { name: 'Add Task' }).click()

  await expect(page.getByText('Title is required.')).toBeVisible()
  expect(postCount).toBe(0)
})
