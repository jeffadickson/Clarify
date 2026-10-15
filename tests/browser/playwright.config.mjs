import {defineConfig} from '@playwright/test';
export default defineConfig({
  testDir: '.',
  testMatch: '*.spec.mjs',
  timeout: 30000,
  use: {baseURL: 'http://127.0.0.1:5168', browserName:'chromium', headless:true},
  webServer: {
    command: 'dotnet run --no-build -c Release --project src/Clarify.Demo.Web/Clarify.Demo.Web.csproj --urls http://127.0.0.1:5168',
    url: 'http://127.0.0.1:5168/studio/er-diagram.html',
    reuseExistingServer: false,
    timeout: 90000,
    env: { ASPNETCORE_ENVIRONMENT:'Development' }
  },
  reporter: [['list'],['html',{outputFolder:'artifacts/playwright-report',open:'never'}]],
  outputDir:'artifacts/playwright-results'
});
