#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
node --test designer/clarifydsl.test.mjs
node tools/generate-shared-blazor.mjs
dotnet restore src/Clarify.Demo.Web/Clarify.Demo.Web.csproj
dotnet build src/Clarify.Demo.Web/Clarify.Demo.Web.csproj --no-restore -c Release
dotnet test tests/Clarify.M0.Tests/Clarify.M0.Tests.csproj -c Release
