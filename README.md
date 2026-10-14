# Clarify

Clarify is an independent, open-source, Clarion-inspired RAD development environment for designing a semantic data dictionary and generating maintainable C#/.NET applications.

**Clarify is not Loom.** Loom is a separate Frappe-inspired application framework. A Loom generator adapter is possible, but not a dependency. Default generated target: ASP.NET Core + EF Core.

## Current project status
Design and requirements baseline only. This repository does not yet contain a working application generator, Studio, or passing tests.

## Project documents
- [Design](docs/DESIGN.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Requirements and traceability](docs/REQUIREMENTS.md)
- [Security threat model](docs/STRIDE.md)
- [Roadmap](docs/ROADMAP.md)
- [Clarion tutorial parity matrix](docs/CLARION_PARITY.md)

## Testing standard
xUnit + NSubstitute + Testcontainers for .NET, with SQLite fast tests and browser-level integration coverage.

## Reference application
The initial implementation benchmark is a Clarion-inspired order-entry workflow: customers, products, orders, line items, and invoices.
