# Elsa milestone gate — executable verification contract

Status on 2026-10-14: **not yet passed**. Do not close #20 until run logs demonstrate success.

## What is committed
- The workflow `.github/workflows/elsa-milestone.yml` validates ClarifyDSL parsing, compiled order-approval plan, and Elsa manifest.
- It installs pinned Elsa.Templates 3.8.0 in a clean GitHub Ubuntu runner with the pinned .NET SDK from `global.json`.
- It generates the official combined server + Studio with SQLite persistence and runs `dotnet restore` and `dotnet build`.
- Identical local entry points: `scripts/bootstrap-elsa.sh` or `scripts/bootstrap-elsa.ps1`.

## Milestone definition — DO NOT CONFUSE THESE
**Gate A — Elsa substrate:** clean generated Elsa host builds and boots and its SQLite-backed workflow instances persist after process restart.
**Gate B — Clarify integration:** supported ClarifyDSL compiles to actual Elsa workflow definitions, registers real C# activities, publishes the definition, starts an instance, waits for manager approval, restarts and resumes.
**Gate C — Production semantics:** authenticated approval, idempotent external activity effects, concurrency and leases, outbox/recovery, migration safety, PostgreSQL/SQL Server Testcontainers tests, Windows/macOS packaging.

The current CI workflow attempts only the **build portion of Gate A**. Gates B and C remain unimplemented/unverified. The existing internal `WorkflowRunner` is not Elsa and must not be counted as Elsa runtime proof.

## Next action after a CI run
1. Examine run/job logs; repair failing NuGet/template/build issues with precise logged evidence.
2. Add boot + health endpoint smoke and an actual Elsa persisted bookmark restart test.
3. Implement the manifest-to-Elsa definition adapter against the exact template APIs, not guessed JSON.
4. Make the passing GitHub build mandatory before closing milestone.
