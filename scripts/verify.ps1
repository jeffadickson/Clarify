$ErrorActionPreference='Stop'
Set-Location (Join-Path $PSScriptRoot '..')
node --test designer/clarifydsl.test.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
node tools/generate-shared-blazor.mjs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore src/Clarify.Demo.Web/Clarify.Demo.Web.csproj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build src/Clarify.Demo.Web/Clarify.Demo.Web.csproj --no-restore -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet test tests/Clarify.M0.Tests/Clarify.M0.Tests.csproj -c Release
exit $LASTEXITCODE
