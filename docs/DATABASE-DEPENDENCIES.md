# Relationship and database dependency analysis

## Scope of this increment
`designer/dependencies.mjs` builds a deterministic dependency graph from the canonical dictionary. It validates relationship endpoints, explicit foreign-key types, nullable `set-null` rules, cascade-delete cycles, database placement and cross-database reference semantics. `impactOfChange(before, after)` identifies added/removed entities and fields, type/nullability changes, moved entities and relationship policy changes.

The same module is served by Clarify Studio as `/studio/dependencies.mjs`. The graphical dictionary at `/studio/schema` displays database dependencies, warnings and a change-impact summary, and exports `clarify-dependency-impact.json`. Changes are still local to the browser until the author explicitly exports the dictionary; no migrations are executed.

## Two different relationship classes

**Native relationship:** Parent and child reside in the same database connection. Clarify can eventually generate an EF Core foreign key and provider-backed referential constraints. Cascade and set-null behavior must be validated against schema nullability and the provider.

**External reference:** Parent and child reside in different named database connections. The relationship must be explicitly `kind: "external"`, with `onDelete: "restrict"`. The analyzer warns that no native foreign key or cross-database atomic transaction is guaranteed. Runtime validation, idempotent updates, durable outbox and reconciliation remain to be implemented.

## Sample model
```json
{
  "defaultDatabase": "sales",
  "databases": [
    {"id":"sales","provider":"postgresql"},
    {"id":"inventory","provider":"sqlserver"}
  ],
  "entities": [
    {"name":"Order","database":"sales","fields":[
      {"name":"Id","type":"long","primaryKey":true},
      {"name":"ProductId","type":"long"}
    ]},
    {"name":"Product","database":"inventory","fields":[
      {"name":"Id","type":"long","primaryKey":true}
    ]}
  ],
  "relationships": [
    {"from":"Order","to":"Product","foreignKey":"ProductId",
     "principalKey":"Id","cardinality":"one-to-many",
     "kind":"external","onDelete":"restrict"}
  ]
}
```
The runnable test fixture is `examples/multi-database/dictionary.json`. Database IDs are *logical identifiers*. Credentials and connection strings belong in secure deployment configuration, never in a dictionary or template.

## Important limitations
- The legacy order-entry dictionary omits explicit primary-key fields; the analyzer treats an unspecified parent `Id` as a compatibility convention. It cannot verify the true database PK type in that case. Production schema generation must require explicit keys and stable field IDs.
- This is **static analysis**, not live SQL schema introspection, EF Core migration generation, referential integrity enforcement, or a distributed transaction coordinator.
- The graph currently models entity and field relationships. Downstream form/report/workflow/C# symbol references, derived field expressions, lookup invalidation, and change-safe rename tracking still require semantic dependency metadata.
- A `requiresReview` result is advisory. Database migrations must remain blocked until explicit review, backup/rollback planning and a real database migration test are available.
- A cross-database reference is not a replacement for a durable distributed consistency pattern. Consequential workflows require an outbox, retry/reconciliation and audit.

## Acceptance tests
`designer/dependencies.test.mjs` covers existing order relationships, external reference classification, cross-database cascades, nullable FK rules, type mismatch, cascade cycles, destructive impact, additive impact and the multi-provider fixture. Tests are wired into the existing local verification scripts and GitHub Actions. Actual green CI results have not yet been observed.

## 2026-10-15 — EF Core model generation increment

The first **real relational source generator** now exists in `designer/ef-models.mjs`. It generates C# entity classes, one `DbContext` per logical database, primary keys, required fields, string length limits, decimal precision, typed FK relationships, FK indexes, one-to-one uniqueness and EF Core delete behaviors.

The CLI `node tools/generate-ef-models.mjs` produces `examples/relational-generation/generated/SalesDbContext.g.cs` from `examples/relational-generation/dictionary.json`. Studio's dictionary designer can **Preview generated EF Core** and download the generated C# for supported models. The pure generator and its browser copy must remain identical.

`tests/ef-generation.test.mjs` verifies native relationships, delete rules, indexes, explicit key enforcement, cross-database isolation and exact generated-source reproducibility. A separate `RelationalGeneration.Smoke.csproj` executes an in-memory SQLite acceptance scenario: insert Customer/Product/Order/OrderLine, reject deletion of a referenced Customer, and cascade-delete OrderLine when Order is deleted. Local verification and CI are configured to run this smoke.

**Intentional restrictions:** this first generator requires exactly one explicit typed primary key per entity, and supports scalar `string`, `int`, `long`, `decimal`, `bool` and `date`. The original sample dictionary omits explicit keys and includes rich money/unit fields; it therefore cannot yet be generated through this strict path. Rich NodaMoney/UnitsNet mappings need the next generator increment. We reject unsupported types rather than silently produce incorrect SQL storage.

This is EF Core **model generation**, not EF Core migrations. No production schema is changed and no migration is applied. Generated C# lives in a separate generated directory; user-authored C# is not overwritten. The generated SQLite smoke uses `EnsureCreated` only on an isolated ephemeral database, never an existing deployment.

Actual green .NET compilation and SQLite execution have not been observed in this environment; the test has been committed to CI.
