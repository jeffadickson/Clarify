# Clarion language and runtime helper review — 2026-10-14

## Decision
Do **not** recreate the Clarion compiler or its legacy file-driver runtime. Keep C# first-class. Add a small typed, safe Clarify expression library and metadata-driven generator helpers that preserve the productivity of Clarion's concise business operations.

Clarion's compiled programming language, template generation language, and ABC runtime are **different layers**. Our analogous layers are: ClarifyDSL for declarations; template manifests and generator API for codegen; .NET/EF Core/Blazor/Elsa for execution; ordinary author-owned C# for arbitrary logic.

## Candidate helper inventory (ordered)
| Priority | Clarion precedent | Proposed Clarify capability | Execution strategy |
|---|---|---|---|
| P0 | FORMAT, DEFORMAT | culture-aware typed format/parse, money/date/percent/mask | .NET Format/Parse wrappers, explicit locale |
| P0 | CHOOSE, INLIST, INRANGE | choose, contains, between, coalesce, conditional expressions | Typed AST interpreter; optional compiled expression |
| P0 | TODAY, DATE, AGE | current date, date parts, date arithmetic, age | DateOnly/TimeOnly/TimeProvider |
| P0 | CLIP, LEFT, RIGHT, SUB, INSTRING | trim/pad/substr/find | .NET string API with explicit semantics |
| P0 | VIEW, JOIN, FILTER, ORDER, PROJECT | visual query, relational navigation, filter/sort/projection | EF Core IQueryable expression tree; server-side SQL |
| P0 | GET, ADD, PUT, DELETE | generated async CRUD, transactions and concurrency | EF Core async APIs, cancellation, authorization |
| P0 | ERRORCODE, ERROR | structured validation and operation results | typed Result/ProblemDetails; no ambient global error |
| P1 | MATCH | safe pattern matching | bounded pattern engine, no arbitrary regex from untrusted users |
| P1 | ACCEPT, EVENT, POST | declarative UI events and commands | Blazor event binding, typed command bus |
| P1 | PROP:... | metadata-driven UI property binding | visual designer + typed component properties |
| P1 | template CALL/INVOKE, EXTRACT, EXISTS | versioned template functions and metadata access | declarative generator API; no eval |
| P2 | Clarion fixed-width strings, numeric date epoch | legacy import compatibility only | explicit adapters, not default semantics |
| Avoid | START, THREAD, raw file positioning | emulate old thread/file driver semantics | use Task, cancellation, async repositories |

## Design requirements
1. A typed AST for expressions, with source positions and diagnostics. Never run arbitrary C# or JavaScript from untrusted DSL.
2. Exact, documented null, type conversion, comparison, rounding and locale behavior; do not silently mimic Clarion fixed-width string padding or numeric dates.
3. Only deterministic, pure helpers allowed in form/computed-field expressions. Impure functions (current time, random, network) require an injected capability and context.
4. Expressions used in queries must translate to provider-supported SQL; fail rather than silently fetch entire tables to evaluate locally.
5. The same expression should yield equivalent results in Studio preview, generated Blazor and server validation. Use shared fixtures and golden tests.
6. Application-authored C# functions register as typed extension points; privileged/server-only helpers cannot be called by an anonymous browser expression.
7. Generate compiled C# where static types and hot paths justify it; use an interpreter for previews, dynamic metadata and lightweight glue. Benchmark both under realistic loads.
8. Preserve existing C# source across regeneration, with standard .NET debugging and NuGet support.

## Clarion source documentation
- https://www.clarion.help/doku.php?id=procedures_listed_by_function.htm
- https://www.clarion.help/doku.php?id=built_in_template_procedures.htm
- https://www.clarion.help/doku.php?id=view_declare_a_virtual_file_.htm
- https://www.clarion.help/doku.php?id=making_the_transition_to_the_abc_templates.htm
- https://www.clarion.help/doku.php?id=additional_language_support.htm

## Initial acceptance
Create a Customer browse from dictionary metadata with computed `DisplayName`, conditional `Status`, formatted amount, and a relational Order count. Test Studio preview, generated C#, SQL translation, authorization, culture and null behavior. Add benchmarks for 1k/100k records and concurrent users. No claim of implementation until runtime tests pass.
