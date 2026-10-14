$ErrorActionPreference='Stop'
Set-Location (Join-Path $PSScriptRoot '..')
dotnet --version
if ($LASTEXITCODE -ne 0) { throw ".NET 10 SDK required" }
dotnet new install Elsa.Templates@3.8.0
if ($LASTEXITCODE -ne 0) { throw "Elsa template installation failed" }
New-Item -ItemType Directory artifacts -Force | Out-Null
if (Test-Path artifacts/ClarifyElsaHost) { throw "Destination exists; refusing overwrite" }
dotnet new elsa-combined -n ClarifyElsaHost --feature-model static --studio-hosting server --persistence sqlite -o artifacts/ClarifyElsaHost
if ($LASTEXITCODE -ne 0) { throw "Elsa template creation failed" }
dotnet restore artifacts/ClarifyElsaHost
if ($LASTEXITCODE -ne 0) { throw "Elsa restore failed" }
dotnet build artifacts/ClarifyElsaHost --no-restore -c Release
if ($LASTEXITCODE -ne 0) { throw "Elsa build failed" }
Write-Output "Configure Development secrets, then run the generated Elsa host."
