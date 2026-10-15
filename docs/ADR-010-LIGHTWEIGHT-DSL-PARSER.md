# ADR-010 — Keep ClarifyDSL parsing lightweight

**Status:** Accepted direction, subject to grammar parity tests.

ClarifyDSL is a small declarative language for forms, dictionary metadata, templates,
reports and application configuration. It is **not** a general-purpose scripting
language; user-written C# remains first-class.

## Decision

Adopt **Peggy** as the preferred grammar generator for the next parsing prototype.
Continue using the existing `designer/clarifydsl.mjs` implementation for production
Studio editing until parity and source-preservation tests pass.

Peggy is a PEG-based JavaScript grammar tool that generates an ES module parser at
build time. Its generated parser runs in browser or Node without a Node server or
the Peggy package at runtime.

Langium, initially scaffolded under `language/`, remains an unselected experiment
rather than a required application dependency. Do not bundle both parsing
frameworks in Studio or replace the current designer before testing.

## Required guarantees

1. Graphical and textual editing operate on exactly the same semantic model.
2. A visual edit must produce a minimal source-range patch; preserve comments,
   whitespace, author code and unrelated declarations.
3. One grammar validates forms, nested groups, grids, field placements and then
   control metadata; broaden to relationships, queries, reports and templates.
4. Show errors with line/column and preserve last valid visual preview.
5. Validate 40 existing template examples and all checked-in .clarify files.
6. Dictionary cross-references and semantic checks remain separate from parsing.
7. Generated apps remain normal .NET executables, with no JS or Node at runtime.
8. C# hooks, normal .NET debugging and NuGet extension points remain available.

## Alternative

Chevrotain is also relatively lightweight and gives more direct concrete-syntax-tree
and recovery control, but requires a programmatic grammar/lexer. Revisit it only
if Peggy's error recovery or source patching proves inadequate.

## Verification gate

`npm ci` and grammar generation with a pinned version, direct syntax acceptance and
rejection tests, source position assertions and golden model parity with current
ClarifyDSL. No provider switch until these pass in CI.

References: https://peggyjs.org/documentation.html ; https://chevrotain.io/
