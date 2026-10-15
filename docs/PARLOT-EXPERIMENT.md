# Native ClarifyDSL parser experiment — Parlot

## Status (2026-10-15)
A new isolated `src/Clarify.Dsl` .NET 10 library uses **Parlot 1.5.9**
for formal parsing of field placement declarations. A conservative C# block
scanner builds a typed `FormDefinition` and `FormField` model and reports
`Diagnostic` entries including 1-based source line/column and source spans.

The current implementation parses the existing nested Customer form example,
groups and grids, comments, form headers, titles, and basic field coordinates.
The companion xUnit project validates good input, malformed placements and
unclosed blocks. It runs through the normal CI verification scripts.

In development, local clients can POST JSON
`{"source":"form Example for Customer { ... }"}` to
`/api/studio/dsl/parse-native` to receive parsed nodes and diagnostics.
The endpoint is read-only, source-size bounded and loopback restricted.
It is not exposed in production.

## Fit evaluation gates

1. **Run green .NET tests** and collect build evidence before adoption.
2. Compare AST fields, source positions and diagnostics against the existing
   `designer/clarifydsl.mjs` parser for the Customer source and template output.
3. Expand syntax to tabs, rich controls, template metadata, dictionary,
   reports, and workflow definitions.
4. Ensure source-aware edits preserve whitespace, comments and C#.
5. Make C# the canonical semantic model before replacing JS validators.
6. Keep current browser editor operational until source round-trip tests pass.

## Limitations
This is **not** an entire recursive Parlot grammar yet: Parlot recognizes
full field-placement syntax while a small native scanner tracks nested blocks.
A complete grammar and reliable visual edit patching are still required.

The existing Langium and Peggy directories are archived experiments.
There is no Node.js or JavaScript interpreter requirement for the new .NET
parser or generated applications.

Do not assert that tests passed merely because the CI commands are committed.

## Compatibility gate and native source edits (2026-10-15)

`tools/export-dsl-parity-fixtures.mjs` now instantiates **all 40 sample templates** plus the existing manually authored Customer form using the active JavaScript grammar. It serializes each resulting source and canonical form/entity/field coordinates/source lines to a transient JSON fixture.

`tests/Clarify.Dsl.Tests/CompatibilityTests.cs` parses these fixtures with the new C# parser and compares every field's name, coordinates and line. All three verification environments—Linux, Windows and GitHub Actions—run fixture generation before the native parser xUnit tests.

`ClarifySourcePatches.MoveField` provides an initial **native C# source-range patch**: it changes just the selected field position, preserving unrelated comments, template metadata, control markers and author formatting. Dedicated tests cover these protections and reject invalid patch requests.

**Parity fixtures are generated build artifacts**; they are not an independent proof of semantic correctness. We must still verify both parser implementations against a full language specification, especially malformed syntax and unusual layouts.

**Current release gate:** no observed .NET test execution yet. The compatibility suite is configured, but Parlot is not automatically promoted to Studio's canonical parser.
