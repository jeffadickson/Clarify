# Clarify beta acceptance — source and runtime evidence

Updated: 2026-10-14

## RAD pipeline
- **Implemented in source:** local-only Studio edit of Customer.clarify; conflict-hash source saving; explicit regenerate script; MSBuild regeneration before shared Razor build; Blazor Studio, dictionary, Customers, Orders, SQLite repositories and invoice preview.
- **New executable gate:** `scripts/smoke-web.sh` boots the Release Blazor host and requires HTTP 200 from /studio, /studio/schema, /studio/designer.html, /studio/clarifydsl.mjs, /studio/dictionary.json, /customers and /orders.
- **CI:** `.github/workflows/shared-blazor.yml` now requires this running-host smoke before publishing the web artifact. A successful workflow run is required to claim the gate passed.
- **Unmet acceptance:** browser-directed visual edit → save → regenerate → rebuild → confirm moved field; verified database order persistence and rollback; true dictionary-driven OrderLine generation; production migrations; signed native delivery.

## Elsa workflow
- **Source supplied:** official Elsa 3.8 combined template bootstrap, Elsa manifest/catalog validator, commented bookmark/approval code, Elsa code-first smoke and compile fixture.
- **CI:** `.github/workflows/elsa-milestone.yml` attempts official template restore/build plus actual Elsa activity smoke and compilation of approval examples.
- **Unmet acceptance:** publish actual ClarifyDSL-generated Elsa definition, start an Elsa instance, durable bookmark across complete host restart, authenticated manager approval, resumed C# effects with outbox/idempotency, duplicate-signal and concurrent-worker tests.

## Release rule
Do not close RAD #24 or Elsa #20 until linked GitHub Actions jobs have actual green results and the acceptance tests match the claimed behavior. Commit existence is not execution evidence. This environment lacks a .NET SDK and cannot run these checks locally.
