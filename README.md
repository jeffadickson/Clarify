# Clarify

Clarify is an independent, Clarion-inspired rapid application development environment for generating understandable C#/.NET business applications from a semantic data dictionary.

**Focus:** Dictionary → generated Blazor web UI → ClarifyDSL ⇄ visual editing → EF Core persistence → master-detail forms and reports → safe regeneration.

**Loom is separate.** ClarifyDSL may become a reusable renderer/library for Loom through an optional adapter; Clarify does not require Loom.

## Current repository state
Source prototypes exist for a browser designer, dictionary-to-Razor generation, shared Web/MAUI components, SQLite persistence, ClarifyDSL workflow parsing and Elsa integration examples. They are **not a verified production-ready app**. .NET builds, native packages and persistent Elsa integration still need green CI evidence.

## Start here
- [RAD-first roadmap](docs/ROADMAP.md)
- [Technical design](docs/DESIGN.md)
- [Requirements](docs/REQUIREMENTS.md)
- [Build and release instructions](docs/BUILD-AND-RELEASE.md)
- [Visual DSL designer](designer/index.html)
- [Workflow examples (optional)](examples/elsa/README.md)
- [Elsa integration limits](docs/ELSA-INTEGRATION.md)

## Verify
Install the SDK requested by `global.json`, Node.js 22+, and prerequisites described in the build guide. Run:
```sh
bash scripts/verify.sh
```
Windows: `./scripts/verify.ps1`.

## Testing standards
xUnit + NSubstitute + Testcontainers for .NET; Node built-in test runner for the prototype DSL; browser verification through Playwright as integration matures.

## Reference acceptance application
A Clarion-inspired order-entry system with customers, products, orders, line items, master-detail editing, lookups, transaction-safe persistence and an invoice report.

## Workflow scope
Elsa 3 is the chosen optional execution engine. We are **pausing feature expansion**, not claiming complete integration. See [the roadmap](docs/ROADMAP.md) and [Elsa milestone issue](https://github.com/jeffadickson/Clarify/issues/20).
