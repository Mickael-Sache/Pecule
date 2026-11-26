import { expect, test } from '@playwright/test';
import AxeBuilder from '@axe-core/playwright';

const WCAG_A_AA = ['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'wcag22aa'];

test('a new user registers, creates a bank account and signs out', async ({ page }) => {
  const email = `e2e-${Date.now()}@example.com`;
  const password = 'Pecule-E2e-2026!';
  const accountName = 'Compte e2e';

  // Register
  await page.goto('/register');
  await page.getByLabel('Adresse e-mail').fill(email);
  await page.getByLabel('Mot de passe', { exact: true }).fill(password);
  await page.getByLabel('Confirmation du mot de passe').fill(password);
  await page.getByRole('button', { name: 'Valider' }).click();

  // First launch Account page (empty)
  await expect(page.getByRole('heading', { level: 1, name: 'Mes comptes' })).toBeVisible();
  await expect(page.getByText("Vous n'avez pas encore de compte bancaire.")).toBeVisible();

  // Create new bank account
  await page.getByLabel('Nom du compte').fill(accountName);
  await page.getByLabel('Type de compte').selectOption({ label: 'Livret' });
  await page.getByLabel('Solde initial').fill('1250.50');
  await page.getByRole('button', { name: 'Enregistrer' }).click();

  // Account is displayed
  const table = page.getByRole('table', { name: 'Liste des comptes' });
  const row = table.getByRole('row', { name: new RegExp(accountName) });
  await expect(row).toBeVisible();
  await expect(row).toContainText('Livret');

  // Check for WCAG/RGAA violation with data in page
  const results = await new AxeBuilder({ page }).withTags(WCAG_A_AA).analyze();
  expect(results.violations).toEqual([]);

  // Logout
  await page.getByRole('button', { name: 'Se déconnecter' }).click();
  await expect(page.getByRole('link', { name: 'Se connecter' })).toBeVisible();
});
