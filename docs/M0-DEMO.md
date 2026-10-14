# M0 demo gate (updated)

Milestone 0 is **demo stage**, replacing the old documentation-only scaffolding interpretation.

## Current deliverables
- [Offline browser RAD demo](../demo/index.html)
- [Demo constraints and usage](../demo/README.md)
- [xUnit + NSubstitute + Testcontainers test scaffold](../tests/Clarify.M0.Tests/Clarify.M0.Tests.csproj)

## Acceptance gates
| Gate | Condition | Status |
|---|---|---|
| M0-A | Standalone interactive application prototype committed | Source delivered; browser verification pending |
| M0-B | End-to-end model-driven form generation | Open — metadata changes currently affect C# preview only |
| M0-C | Generate compilable C#/EF Core/SQLite project | Open — .NET SDK unavailable in current execution environment |
| M0-D | xUnit/NSubstitute tests run and pass in CI | Open — scaffold committed, not executed |
| M0-E | PostgreSQL/SQL Server integration through Testcontainers | Open |
| M0-F | Usability/browser automation and report output | Open |
| M0-G | Optional Loom target verified | Deferred, not prerequisite |

## Claim discipline
Do not confuse runnable standalone HTML with generated compiled .NET applications; the former is the M0 demonstration increment, the latter is a remaining milestone acceptance criterion. Keep issue #1 open until the requisite gates are green.
