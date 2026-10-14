#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
node --test designer/clarifydsl.test.mjs workflow/workflowdsl.test.mjs workflow/compile.test.mjs workflow/elsa-manifest.test.mjs workflow/elsa-catalog.test.mjs
node workflow/compile.mjs
node tools/generate-shared-blazor.mjs
dotnet restore src/Clarify.Demo.Web/Clarify.Demo.Web.csproj
dotnet build src/Clarify.Demo.Web/Clarify.Demo.Web.csproj --no-restore -c Release
dotnet test tests/Clarify.M0.Tests/Clarify.M0.Tests.csproj -c Release
