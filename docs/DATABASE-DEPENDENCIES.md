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
