$ErrorActionPreference='Stop'
Set-Location (Join-Path $PSScriptRoot '..')
node --test tests/sample-template-catalog.test.mjs designer/templates.test.mjs designer/schema.test.mjs tests/studio-integration.test.mjs designer/clarifydsl.test.mjs workflow/workflowdsl.test.mjs workflow/compile.test.mjs workflow/elsa-manifest.test.mjs workflow/elsa-catalog.test.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
node workflow/compile.mjs
node --test tests/automatic-observability.test.mjs
node tools/generate-observability.mjs
node --test tests/standard-value-types.test.mjs tests/async-generator.test.mjs
node tools/generate-async-contracts.mjs
node tools/generate-shared-blazor.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore src/Clarify.Demo.Web/Clarify.Demo.Web.csproj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build src/Clarify.Demo.Web/Clarify.Demo.Web.csproj --no-restore -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet test tests/Clarify.M0.Tests/Clarify.M0.Tests.csproj -c Release
exit $LASTEXITCODE
