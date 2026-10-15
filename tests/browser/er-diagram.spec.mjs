import {test,expect} from '@playwright/test';

test('ER diagram keeps Studio links and supports fit/zoom/auto layout',async ({page})=>{
  await page.goto('/studio/er-diagram.html');
  await expect(page.getByRole('heading',{name:/Database Relationship Diagram/})).toBeVisible();
  await expect(page.locator('svg g[role="group"]')).toHaveCount(4);
  await expect(page.getByRole('status')).toContainText('3 relationships');
  const before=await page.locator('#diagram').getAttribute('viewBox');
  await page.getByRole('button',{name:'Zoom in'}).click();
  const after=await page.locator('#diagram').getAttribute('viewBox');
  expect(after).not.toBe(before);
  await page.getByRole('button',{name:'Fit'}).click();
  await expect(page.locator('#diagram')).toHaveAttribute('viewBox',before);
  await page.getByRole('button',{name:'Auto layout'}).click();
  await expect(page.locator('svg path')).toHaveCount(3);
  await page.getByRole('link',{name:/Back to Data Dictionary/}).click();
  await expect(page).toHaveURL(/\/studio\/schema$/);
});

test('ER diagram keyboard placement is retained when page reloads',async ({page})=>{
  await page.goto('/studio/er-diagram.html');
  const group=page.locator('svg g[role="group"]').first();
  const old=await group.getAttribute('transform');
  await group.focus();
  await page.keyboard.press('ArrowRight');
  await expect(group).not.toHaveAttribute('transform',old);
  const updated=await group.getAttribute('transform');
  await page.reload();
  await expect(page.locator('svg g[role="group"]').first()).toHaveAttribute('transform',updated);
});

test('ER diagram exports a real SVG without modifying dictionary',async ({page})=>{
  await page.goto('/studio/er-diagram.html');
  const [download]=await Promise.all([
    page.waitForEvent('download'),
    page.getByRole('button',{name:'Export SVG'}).click()
  ]);
  expect(download.suggestedFilename()).toBe('clarify-er-diagram.svg');
  await expect(page.getByRole('status')).toContainText('4 entities');
});
