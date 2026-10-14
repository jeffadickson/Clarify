# Clarify verification status — October 14, 2026

## Source changes delivered
- Missing dictionary JSON restored so `tools/generate-shared-blazor.mjs` has its schema input.
- .NET web, shared, data, extensibility and test projects retargeted to .NET 10 LTS; MAUI remains an independent net8.0 transition task and is **not** a verified cross-platform release.
- `global.json` and local EF tool manifest updated to 10.
- Repeatable shell/PowerShell verification invokes form and workflow DSL tests, Razor generation, web compilation and xUnit.
- CI web job includes generation-drift check, test and publish artifact.
- `workflow/workflowdsl.mjs` has a bounded parser supporting definition, version, event start, activities, human wait, and retry policy; the browser `workflow/index.html` renders and allows moving sequential steps in DSL.
- Approval workflow instances still persist to SQLite through existing `ApprovalWorkflowService`.

## Important limitations
1. **No actual Workflow Core engine adapter is installed.** The existing order approval state machine is persisted and resumable but cannot execute arbitrary DSL-defined activities; DSL and runtime are not yet wired together.
2. The graphical workflow preview is a source-aware sequence editor, not a general bidirectional diagram designer.
3. No .NET SDK, container runtime, or outbound git networking is available in this authoring environment; GitHub connector commits succeeded, but compilation and .NET integration tests have **not** been observed passing.
4. NuGet lockfiles, locked restores, SBOM/checksums, signed release packages and full MAUI verification remain open.
5. Generated forms are still a customer-focused demo, not full order-entry/approval UI integration.

## Follow-up gates
- Inspect the outcome of the new CI run, resolve all compilation errors and rerun.
- Generate and commit packages.lock.json, enable locked-mode restore, and ensure dependency update bot regenerates lockfiles.
- Integrate workflow execution and activity registration with persisted engine storage, outbox and concurrency leases.
- Add Testcontainers-backed database parity and Playwright browser tests.
- Build and smoke-test Windows and macOS native packages and publish signed versioned artifacts.

## Subsequent build hardening pass
- Fixed the net8/native-versus-net10/shared project framework mismatch by retargeting MAUI to net10 and requesting MAUI 10 package references. Native build is still unverified and may require additional platform package/SDK changes.
- Configured NuGet lockfile generation for test projects too. Lockfiles have not yet been generated or committed; locked-mode restore remains unavailable.
- Added local JSON mutation and Origin safeguards to development-only workflow endpoints, while noting that loopback access is **not authentication**.
- GitHub status API returned no commit statuses for the checked commit; this is not evidence that Actions ran or passed.
- Workflow Core itself, a compiler connection from workflow DSL to runtime, Testcontainers integration and signed packaging remain open.

**Release verdict:** not beta-certified. Changes in this pass are committed source/build configuration without observed native or .NET execution.
