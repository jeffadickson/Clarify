$ErrorActionPreference='Stop'
Set-Location (Join-Path $PSScriptRoot '..')
node --test tests/template-gallery-count.test.mjs tests/form-controls.test.mjs tests/sample-template-catalog.test.mjs designer/templates.test.mjs designer/dependencies.test.mjs designer/schema.test.mjs tests/er-layout.test.mjs tests/studio-integration.test.mjs designer/clarifydsl.test.mjs workflow/workflowdsl.test.mjs workflow/compile.test.mjs workflow/elsa-manifest.test.mjs workflow/elsa-catalog.test.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
node --test tests/ef-generation.test.mjs tests/sqlite-migrations.test.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
node tools/generate-ef-models.mjs
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
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet run --project examples/relational-generation/RelationalGeneration.Smoke.csproj -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build tools/Clarify.Migrations.Sqlite/Clarify.Migrations.Sqlite.csproj -c Release
exit $LASTEXITCODE

node tools/export-dsl-parity-fixtures.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet test tests/Clarify.Dsl.Tests/Clarify.Dsl.Tests.csproj -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

node --test tests/documentation-generation.test.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
node tools/audit-csharp-docs.mjs --report --strict-generated
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet tool restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet docfx docs/docfx.json
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
