# ClarifyDSL and Loom — proposed integration

## Recommendation
**Yes, Loom could benefit from ClarifyDSL.** Keep the projects independent. Publish reusable `ClarifyDSL.Core` (parser, AST, schema bindings) and `ClarifyDSL.Blazor` (form renderer) packages. Loom consumes them as an optional front end, not a foundational dependency. Clarify can continue generating completely standalone ASP.NET Core apps.

## Benefits to Loom
1. Auto-generate UI from Loom DocType-style schemas.
2. Enable end-users to customize forms visually without handwritten HTML/Razor.
3. Store source-controlled `.clarify` form declarations and safe layout overrides.
4. Reuse validation/relationship metadata and responsive layouts.
5. Let Loom expose plugin-defined controls with explicit typed contracts.

## Required boundary
`Loom DocType / model` → `LoomSchemaAdapter` → `ClarifyDSL semantic schema` → `ClarifyDSL AST` → `Blazor renderer`.

The adapter translates Loom's native types, permissions, lookup semantics, calculated fields, field names and stable IDs into ClarifyDSL equivalents. Loom still owns data access, authorization, workflows, transactions and business validation. ClarifyDSL owns layout semantics, compile-time binding diagnostics and rendering.

## Nonnegotiable constraints
- No circular repository/package dependency.
- No generated form bypasses server-side Loom authorization.
- Loom adapters must report unsupported field types rather than silently dropping them.
- Forms never copy secrets or execute arbitrary C# embedded in DSL.
- Keep renderer interchangeable; don't make Loom dependent on Clarify Studio.
- Integration tests must cover Lookups, child tables, permissions, money and schema upgrades.

## Sequencing
1. Stabilize DSL grammar, AST IDs and diagnostics.
2. Convert first Blazor target to typed EditForm + a backend-agnostic save handler.
3. Publish parser and renderer NuGet packages.
4. Implement Loom model adapter and use it in a real customer/order screen.
5. Run xUnit/NSubstitute contracts, Testcontainers database tests and browser UI tests.

Current state: **architecture proposal only**. No Loom source files changed.
