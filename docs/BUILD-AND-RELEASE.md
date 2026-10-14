# Build and release infrastructure

## Platform audit (2026-10-14)
- .NET 8 projects: shared Razor class library, EF Core SQLite data store, ASP.NET Core web host, extensibility contracts, xUnit/NSubstitute/Testcontainers test project and MAUI Hybrid (Windows, Mac Catalyst, iOS, Android).
- JavaScript: pure ESM, Node built-in test runner; no external npm dependencies, so no npm/pnpm lockfile is necessary yet.
- Browser-only designer: static HTML + ES modules; no bundler required.
- No C/C++, Python, Ruby, Rust or HDL build systems in this repository's known tracked sources. Do not introduce Make/CMake/uv/pnpm without need.

## Fresh clone (web)
Install git, Node 22+, the SDK selected by `global.json` and internet access for initial NuGet restore.
```sh
git clone https://github.com/jeffadickson/Clarify.git
cd Clarify
bash scripts/verify.sh
dotnet run --project src/Clarify.Demo.Web
```
Windows PowerShell: `./scripts/verify.ps1`.
`node tools/generate-shared-blazor.mjs` refreshes the shared Razor layout from `examples/forms/Customer.clarify`.

## Dependencies
- Use SDK pinning in `global.json`, NuGet source pinning in `NuGet.Config`, direct explicit package versions in .csproj. 
- Restore lock mode is transitional: `packages.lock.json` **still needs generation and commit** using a full .NET SDK environment. After it exists, CI should use `dotnet restore --locked-mode`.
- `.config/dotnet-tools.json` pins `dotnet-ef`. Run `dotnet tool restore` as needed.
- No secret values committed. Local SQLite files ignored.
- Configure Dependabot for weekly NuGet/GitHub Actions updates, review and merge only with build/test evidence.

## CI and release
- Linux job runs Node tests, generated Razor build, xUnit and web publish.
- Windows MAUI and macOS Mac Catalyst builds require platform workloads, signing settings, and separate CI capacity; not treated as passing without evidence.
- Release artifacts: self-contained/published web zip, reproducibility manifest and checksums; portable native install packages later.
- Release checklist: reproducible fresh clone, lock restore, unit/integration/browser tests, migrations, security analysis, SBOM, version/tag provenance and signed release bundles.
- Distinct databases: web uses server-side SQLite; MAUI uses platform AppData SQLite. No sync unless explicitly added.

## Modernization decision
.NET 8 and 9 support ends November 10, 2026; .NET 10 LTS is the forward target. Current targets remain on 8 until MAUI and web projects pass an actual migration build. `global.json` is intentionally a reproducible *transitional* baseline, **not** a claim that .NET 8 is a suitable long-lived production release. See upgrade issue.

## Engineering limitations
No .NET SDK or Docker engine is installed in the authoring environment. Node test results and GitHub CI should be used to verify actual builds. Existing generated Blazor forms and desktop scaffold were not previously compiled here, and a full build may expose additional defects.
