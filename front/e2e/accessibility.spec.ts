import { expect, test } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

const WCAG_A_AA = ['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'wcag22aa'];

for (const path of ['/', '/login', '/register']) {
  test(`${path} has no WCAG A/AA violation`, async ({ page }) => {
    await page.goto(path);
    await expect(page.getByRole('heading', { level: 1 })).toBeVisible();

    const results = await new AxeBuilder({ page }).withTags(WCAG_A_AA).analyze();
    expect(results.violations).toEqual([]);
  });
}

test('the skip link is the first element reached with Tab', async ({ page }) => {
  await page.goto('/');
  await expect(page.getByRole('heading', { level: 1 })).toBeVisible();
  await page.keyboard.press('Tab');
  await expect(page.getByRole('link', { name: 'Aller au contenu' })).toBeFocused();
});
