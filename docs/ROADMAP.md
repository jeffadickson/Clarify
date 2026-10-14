# Clarify delivery roadmap — RAD-first reset
Updated 2026-10-14

## Product objective
Recreate and surpass Clarion's rapid application development workflow:
**Dictionary → automatically generated usable application → two-way visual/ClarifyDSL editing → database-backed browser/desktop runtime → reports → safe regeneration.**

Loom remains independent; optional Loom adapters may consume reusable ClarifyDSL libraries.

## Active milestone: RAD vertical slice (highest priority)
1. **Clean build:** .NET 10 solution, lockfiles, generated-source consistency, passing xUnit/Node tests and CI.
2. **Dictionary:** open an existing database or define Customer, Product, Order and OrderLine visually. Verify field/key/relationship metadata and generated schema.
3. **Live form designer:** schema generates ClarifyDSL, source ↔ canvas editing, preserve manual edits and stable identities.
4. **Generated app:** compile and run ASP.NET Core + Blazor + EF Core + SQLite. Real master-detail editing, lookups, validation and transactions.
5. **Reports:** grouped customer and invoice report in browser/print output.
6. **Portable deliverable:** independent generated source, one-command local start, verified publish artifact, web and desktop shell smoke where practical.

## Optional subsystem: Elsa integration (maintenance / gated)
**Decision:** Elsa 3 is the supported engine choice. Do not build a competing production engine. Keep ClarifyDSL workflow syntax, the Elsa manifest/catalog validation and the commented C# examples. Do not continue implementing arbitrary Elsa activities, graphical parity, advanced scheduling or distributed execution before RAD milestone is green.

**What exists:** Elsa bootstrap scripts, manifests/catalog preflight, sample blocking activity/controller and compile/smoke fixture source. **What is NOT verified:** actual Elsa combined-host build, ClarifyDSL-to-Elsa runtime definition registration, persistent Elsa bookmark restart, authenticated approval, full Elsa Studio round-trip, multiworker exactly-once business effects.

**Reactivation condition:** generated RAD app builds/runs and the user specifically prioritizes workflow. First Elsa task on reactivation: actual Elsa host + bookmark restart test, not more DSL syntax.

## Release discipline
- A committed test is not a passing test; record execution evidence.
- Maintain open issues for unverified parts rather than declaring general Elsa compatibility complete.
- No further increase to workflow requirements until working RAD acceptance fixture passes.
- All new functionality needs commented examples, associated acceptance test and current requirements status.
