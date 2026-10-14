# Graphical Data Dictionary — M1 prototype

The Clarify Studio route `/studio/schema` now hosts an interactive schema designer, sourced from the same project dictionary used by generation.

## Implemented in this increment
- Create entity with automatically suggested long integer primary key.
- Add typed fields with required flag.
- Connect child table to parent using a named foreign key; supply cardinality and delete behavior.
- Inline validation of duplicate entities/fields, unknown types, missing relationship endpoints/foreign keys.
- Export a complete JSON dictionary; preserve existing metadata while editing.
- Three sample relationships: Order → Customer, OrderLine → Order, OrderLine → Product.
- Browser-local work-in-progress persistence, reset, and a canonical JSON export.

## Developer example
1. Open `/studio/schema`.
2. Click **+ Entity**, enter `Supplier`.
3. Click **+ Field** in Supplier, define `Name` as required string.
4. Click **+ Relationship**, set child `Product`, parent `Supplier`, FK `SupplierId`, deletion `restrict`.
5. Export dictionary.json, review the Git diff and replace the source dictionary intentionally.
6. Use Clarify's CLI generation after schema validation. **Direct UI → SQL migrations is not yet implemented.**

## Gaps before parity with Clarion
This is an editable visual dictionary prototype, **not yet a complete ER diagram canvas**: no mouse-drawn connectors, field property inspectors, index/key editing, versioned schema migrations, live server persistence, reverse engineering or full relational code generation. The existing generated application does not automatically incorporate exported dictionary changes. Those features remain the next primary work.

## Verification
`designer/schema.test.mjs` and `tests/studio-integration.test.mjs` are wired into local scripts and GitHub CI. No actual .NET runtime or full browser interaction test has been witnessed in this environment.
