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
