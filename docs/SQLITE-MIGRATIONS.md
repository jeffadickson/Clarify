# Safe SQLite migrations — first executable slice

## What is supported
Clarify now has a **deliberately narrow, reviewed SQLite migration path** for adding a nullable simple scalar column to an existing table. It is a proof of the dictionary → migration plan → approved execution loop, not a replacement for EF Core migrations across providers.

- `designer/sqlite-migrations.mjs` compares two dictionary snapshots and produces a deterministic, read-only `clarify-sqlite-migration/v1` plan.
- `tools/Clarify.Migrations.Sqlite` is an independent .NET CLI that requires an explicit plan ID, an existing SQLite database, and a **new backup path**. It validates the schema, validates a restricted DDL allowlist, creates a SQLite online backup, then executes the approved DDL in a transaction and records migration history.
- The Studio dictionary editor can load the explicit-key relational demo, add an optional field, preview the plan, and download it. No SQL is run in the browser.
- `scripts/smoke-sqlite-migrations.sh` creates an isolated SQLite database and checks approval refusal, modified-plan refusal, successful schema change, data preservation, backup integrity and replay refusal.
- `tests/sqlite-migrations.test.mjs` verifies deterministic plans and rejects destructive changes, unsupported rich types, required columns, and relationship policy changes.

## Try the example

1. Open Studio `/studio/schema`, click **Load relational demo**.
2. Under Product, add a `Notes` field of type `string`, required = false.
3. Click **Preview safe SQLite migration** and inspect the proposed SQL and plan ID.
4. Export the plan. Do not apply it to a production database without backup verification and review.
5. For a *test database with the exact baseline schema*, use the .NET CLI:

```sh
dotnet run --project tools/Clarify.Migrations.Sqlite/Clarify.Migrations.Sqlite.csproj -- \
  --plan examples/relational-generation/product-notes.migration.json \
  --db /path/to/test-sales.db \
  --approve clarify-sqlite-82cf008e \
  --backup /path/to/new-test-sales.backup.db
```

The ID is for the checked-in sample plan only. A different schema or migration plan requires a different ID.

## Safety boundaries

- This implementation **refuses** table deletion, column deletion/rename, type conversion, changing nullability, adding non-nullable fields, relationship changes, cross-database migrations, and unsupported money/unit mappings.
- No SQL is run without an explicit CLI invocation. There is no production auto-migration on application startup.
- The migration plan ID uses a deterministic non-cryptographic fingerprint to detect accidental modifications. It is **not** a signature or access-control mechanism. The operator's OS/database permissions remain authoritative.
- The executor verifies names/types of the expected SQLite columns, but does not yet compare every index, constraint, collation or schema object. It must not be described as comprehensive schema-drift protection.
- Backup is made using SQLite's online backup API rather than copying an open database file. The backup filename is reserved with an atomic create-new operation to avoid overwriting an existing backup; restore automation and comprehensive concurrency hardening are not yet implemented.
- For other database providers, use reviewed EF Core migrations with their own snapshots and provider-specific SQL. The current planner does not pretend to generate them.
- Current dictionary change tracking uses names rather than immutable field IDs. A rename is treated as potentially destructive and **blocked**.
- Generated entity source uses explicit `ToTable("Entity")` naming to keep schema names stable for migration planning.

## Verification

The Node planner behavior and generated EF Core source were checked at source level. GitHub CI is configured to execute the real SQLite smoke; a successful .NET run has **not** yet been observed. Do not apply this to a consequential database until the full smoke and restore drill pass.

## Next steps

Implement immutable dictionary IDs and migration snapshots, real EF Core migration scaffolding per provider, migration review/approval UI, backup restore verification, concurrency/locking hardening, rollback policy, and generated money/UnitsNet mappings. These are tracked separately from the narrow SQLite prototype.
