# Clarify native Elsa activity catalog — initial implementation

**Status:** build-time activity/type/property validation implemented in `workflow/elsa-catalog.mjs`. Full runtime adapter remains open.

## Why this exists
`elsa "Elsa.Http.SendHttpRequest" as Notify config "activities/send.json"` must not turn into an arbitrary reflection-based execution instruction. The Clarify compiler should resolve references against activities **actually registered** in a pinned Elsa host and validate supported properties before generation.

The current `validateElsaCatalog(manifest,catalog,projectRoot)` function:
1. Requires the expected manifest format.
2. Looks up a native activity in the supplied registry.
3. Rejects unknown and disabled activities.
4. Reads JSON configuration only from within the project directory, rejecting traversal and symbolic-link escapes.
5. Rejects unrecognized property names, returning structured node diagnostics.
6. Returns validated configuration records for subsequent adapter compilation.

Example catalog: [elsa-catalog.example.json](../workflow/elsa-catalog.example.json). **This example is illustrative and is not verified against Elsa's real installed descriptors.**

## Verification
Run `node --test workflow/elsa-catalog.test.mjs` to exercise known activity, missing activity, invalid properties and path rejection. Tests are included in CI but do not prove the Elsa host actually built or ran.

## Next milestone
Implement a `.NET 10` registry exporter inside the Elsa 3.8 host that enumerates Elsa-registered activity descriptors and writes a versioned catalog. Replace static/mock catalog data in integration tests with that export. Then build and execute a mixed ClarifyDSL/C# and native Elsa workflow using the Elsa SDK. Do not load untrusted plugin assemblies or source code while merely inspecting the DSL.

## Remaining constraints
- Property *types*, value bindings, variables/expressions, ports and activity-specific constraints are not validated yet.
- Workflow Studio import/export and full bidirectional roundtrip remain open.
- Native activity configuration is not yet hydrated into Elsa runtime objects.
- Permission policy, plugin signatures and execution sandbox requirements remain open.
