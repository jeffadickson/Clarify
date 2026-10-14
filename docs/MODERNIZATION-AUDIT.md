# Clarify build/release modernization audit

Audit date: 2026-10-14. Scope: known repository projects, tools, build paths and workflows (source-level inspection through GitHub connector; remote git clone failed due unavailable outbound DNS from the container).

| Area | Finding | Action |
|---|---|---|
| .NET web + Razor + persistence + MAUI | Multiple independent projects pinned to .NET 8, manual workload requirements | Introduced SDK global.json, repeatable scripts, continued .NET 10 migration gate |
| EF Core/NuGet | Direct package versions, no committed lockfiles | Added NuGet.Config; **lockfiles still to be generated and validated** |
| Node DSL tools | Pure ES modules, no npm packages | Keep Node built-in test runner; no needless package manager |
| Browser designer | Static HTML; no asset pipeline | Keep zero-build static mode |
| Testing | Node test code, xUnit/NSubstitute template, skipped Testcontainers scenario | CI runs .NET tests; live Testcontainers provider parity still open |
| Native desktop | MAUI platform project not proven to compile | CI Windows experimental; macOS runner and signing still needed |
| Release artifacts | No standardized publish job | CI web publish + artifact upload |
| Dependency hygiene | No update bots or repo .gitignore | Added Dependabot weekly NuGet/Actions; ignored generated files |
| Runtime workflow | Architecture-only, no persisted implementation | Added SQLite approval flow + fresh-context restart test |

## Release verification gates not yet met
- Full clean clone on Windows and macOS with MAUI build, package and smoke tests.
- Upgrade net8.0 and related dependencies to net10.0 LTS, verify all renderers.
- Generate/commit NuGet lockfiles and enforce locked restore; produce dependency inventory/SBOM.
- Secure workflow approval with real identity and policy.
- Playwright form-to-SQLite and workflow HTTP tests, Testcontainers PostgreSQL/SQL Server.
- Signed/tagged releases, checksum provenance, scheduled dependency scans.
- Remove old experimental CI jobs and consolidate duplicate generator paths after successful green run.

Maturity: reproducible build *entry points introduced*, but not yet reproducible dependency closure or verified cross-platform release. 
