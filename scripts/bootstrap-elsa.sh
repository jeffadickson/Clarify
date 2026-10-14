#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
command -v dotnet >/dev/null || { echo ".NET 10 SDK required" >&2; exit 2; }
dotnet --version
# Official Elsa 3.8 templates supply compatible workflow runtime, Studio and SQLite persistence.
dotnet new install Elsa.Templates@3.8.0
mkdir -p artifacts
if [ -e artifacts/ClarifyElsaHost ]; then
  echo "artifacts/ClarifyElsaHost exists; refusing to overwrite" >&2
  exit 3
fi
dotnet new elsa-combined -n ClarifyElsaHost --feature-model static --studio-hosting server --persistence sqlite -o artifacts/ClarifyElsaHost
dotnet restore artifacts/ClarifyElsaHost
dotnet build artifacts/ClarifyElsaHost --no-restore -c Release
echo "Official Elsa runtime+Studio compiled. Configure Development secrets using Elsa template instructions; then dotnet run --project artifacts/ClarifyElsaHost"
