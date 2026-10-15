# Clarify requirements v0.2 — traceable baseline

**Status:** Proposed, not implemented. These requirements describe planned work, not completed software. Clarify is independent from Loom; Loom support is optional through a versioned adapter. P0 = foundation; P1 = later milestone. Each requirement must receive a test case ID, implementation link, owner, and state before it can be marked verified.

## Tracking protocol (requirements-tracker compatible)

Canonical fields: `ID`, `Title`, `Priority`, `Status`, `Verification`, `Test IDs`, `Implementation`, `Rationale`. Allowed statuses: `Proposed`, `Accepted`, `In Progress`, `Implemented`, `Verified`, `Deferred`, `Rejected`. Changes require reviewable Git commits; requirements are never silently renumbered. Security threat modeling is maintained separately in `docs/STRIDE.md` (the STRIDE threat methodology is not the requirements tracker).

| ID | Priority | Status | Requirement | Acceptance evidence |
|---|---|---|---|---|
| DCT-001 | P0 | Proposed | Versioned canonical semantic dictionary | Automated test or documented review proving versioned canonical semantic dictionary |
| DCT-002 | P0 | Proposed | Stable IDs for models and fields | Automated test or documented review proving stable ids for models and fields |
| DCT-003 | P0 | Proposed | Entities and scalar types | Automated test or documented review proving entities and scalar types |
| DCT-004 | P0 | Proposed | Nullable/default values | Automated test or documented review proving nullable/default values |
| DCT-005 | P0 | Proposed | Composite primary keys | Automated test or documented review proving composite primary keys |
| DCT-006 | P0 | Proposed | Unique constraints and alternate keys | Automated test or documented review proving unique constraints and alternate keys |
| DCT-007 | P0 | Proposed | Indexes and sort definitions | Automated test or documented review proving indexes and sort definitions |
| DCT-008 | P0 | Proposed | Foreign keys and cardinality | Automated test or documented review proving foreign keys and cardinality |
| DCT-009 | P0 | Proposed | Referential actions and cascades | Automated test or documented review proving referential actions and cascades |
| DCT-010 | P0 | Proposed | Reusable column pool definitions | Automated test or documented review proving reusable column pool definitions |
| DCT-011 | P1 | Proposed | Inherited domain-property overrides | Automated test or documented review proving inherited domain-property overrides |
| DCT-012 | P1 | Proposed | Field captions and help text | Automated test or documented review proving field captions and help text |
| DCT-013 | P1 | Proposed | Masks and display formatting | Automated test or documented review proving masks and display formatting |
| DCT-014 | P1 | Proposed | Input validation rules | Automated test or documented review proving input validation rules |
| DCT-015 | P1 | Proposed | Cross-field validation | Automated test or documented review proving cross-field validation |
| DCT-016 | P1 | Proposed | Calculated values and dependencies | Automated test or documented review proving calculated values and dependencies |
| DCT-017 | P1 | Proposed | Cycle detection in expressions | Automated test or documented review proving cycle detection in expressions |
| DCT-018 | P1 | Proposed | Typed currency and rounding rules | Automated test or documented review proving typed currency and rounding rules |
| DCT-019 | P1 | Proposed | Date/time/timezone policy | Automated test or documented review proving date/time/timezone policy |
| DCT-020 | P1 | Proposed | Binary data and attachment metadata | Automated test or documented review proving binary data and attachment metadata |
| DCT-021 | P1 | Proposed | Enumerations and domain types | Automated test or documented review proving enumerations and domain types |
| DCT-022 | P1 | Proposed | Schema import from SQLite | Automated test or documented review proving schema import from sqlite |
| DCT-023 | P1 | Proposed | PostgreSQL schema introspection | Automated test or documented review proving postgresql schema introspection |
| DCT-024 | P1 | Proposed | SQL Server schema introspection | Automated test or documented review proving sql server schema introspection |
| DCT-025 | P1 | Proposed | Dictionary schema version migration | Automated test or documented review proving dictionary schema version migration |
| DCT-026 | P1 | Proposed | Roundtrip deterministic serialization | Automated test or documented review proving roundtrip deterministic serialization |
| DCT-027 | P1 | Proposed | User-defined annotations | Automated test or documented review proving user-defined annotations |
| DCT-028 | P1 | Proposed | Schema migration compatibility diagnostics | Automated test or documented review proving schema migration compatibility diagnostics |
| DCT-029 | P1 | Proposed | Model diff and rename detection | Automated test or documented review proving model diff and rename detection |
| DCT-030 | P1 | Proposed | Dictionary documentation generation | Automated test or documented review proving dictionary documentation generation |
| GEN-001 | P0 | Proposed | Headless generation CLI | Automated test or documented review proving headless generation cli |
| GEN-002 | P0 | Proposed | Deterministic builds and hashes | Automated test or documented review proving deterministic builds and hashes |
| GEN-003 | P0 | Proposed | Generation manifest with provenance | Automated test or documented review proving generation manifest with provenance |
| GEN-004 | P0 | Proposed | Readable generated C# models | Automated test or documented review proving readable generated c# models |
| GEN-005 | P0 | Proposed | EF Core persistence target | Automated test or documented review proving ef core persistence target |
| GEN-006 | P0 | Proposed | ASP.NET Core API target | Automated test or documented review proving asp.net core api target |
| GEN-007 | P0 | Proposed | Provider capability diagnostics | Automated test or documented review proving provider capability diagnostics |
| GEN-008 | P0 | Proposed | Regeneration preserves user code | Automated test or documented review proving regeneration preserves user code |
| GEN-009 | P0 | Proposed | Generated versus user-owned directories | Automated test or documented review proving generated versus user-owned directories |
| GEN-010 | P0 | Proposed | Typed customization points | Automated test or documented review proving typed customization points |
| GEN-011 | P1 | Proposed | Stable generated API signatures | Automated test or documented review proving stable generated api signatures |
| GEN-012 | P1 | Proposed | Custom template packaging | Automated test or documented review proving custom template packaging |
| GEN-013 | P1 | Proposed | Incremental generation | Automated test or documented review proving incremental generation |
| GEN-014 | P1 | Proposed | Dependency-ordered generator passes | Automated test or documented review proving dependency-ordered generator passes |
| GEN-015 | P1 | Proposed | Compiler diagnostics linked to dictionary | Automated test or documented review proving compiler diagnostics linked to dictionary |
| GEN-016 | P1 | Proposed | Build after generation | Automated test or documented review proving build after generation |
| GEN-017 | P1 | Proposed | Golden output regression tests | Automated test or documented review proving golden output regression tests |
| GEN-018 | P1 | Proposed | Generated migration planning | Automated test or documented review proving generated migration planning |
| GEN-019 | P1 | Proposed | Destructive migration approvals | Automated test or documented review proving destructive migration approvals |
| GEN-020 | P1 | Proposed | Rollback and recovery policy | Automated test or documented review proving rollback and recovery policy |
| GEN-021 | P1 | Proposed | OpenAPI generation | Automated test or documented review proving openapi generation |
| GEN-022 | P1 | Proposed | Generator version compatibility | Automated test or documented review proving generator version compatibility |
| GEN-023 | P1 | Proposed | Application manifest and launch profile | Automated test or documented review proving application manifest and launch profile |
| GEN-024 | P1 | Proposed | Plugin target architecture | Automated test or documented review proving plugin target architecture |
| UI-001 | P0 | Proposed | Local browser-based Studio | Automated test or documented review proving local browser-based studio |
| UI-002 | P0 | Proposed | Project creation/import wizard | Automated test or documented review proving project creation/import wizard |
| UI-003 | P0 | Proposed | Dictionary property grid | Automated test or documented review proving dictionary property grid |
| UI-004 | P0 | Proposed | Entity relationship designer | Automated test or documented review proving entity relationship designer |
| UI-005 | P0 | Proposed | Application procedure tree | Automated test or documented review proving application procedure tree |
| UI-006 | P0 | Proposed | Generated list/browse pages | Automated test or documented review proving generated list/browse pages |
| UI-007 | P0 | Proposed | Filtering and multi-sort | Automated test or documented review proving filtering and multi-sort |
| UI-008 | P0 | Proposed | Incremental locator search | Automated test or documented review proving incremental locator search |
| UI-009 | P0 | Proposed | Paged and virtualized lists | Automated test or documented review proving paged and virtualized lists |
| UI-010 | P0 | Proposed | Create/edit/delete forms | Automated test or documented review proving create/edit/delete forms |
| UI-011 | P1 | Proposed | Field-based editor selection | Automated test or documented review proving field-based editor selection |
| UI-012 | P1 | Proposed | Data-bound lookup controls | Automated test or documented review proving data-bound lookup controls |
| UI-013 | P1 | Proposed | Master-detail editing | Automated test or documented review proving master-detail editing |
| UI-014 | P1 | Proposed | Inline grid editing | Automated test or documented review proving inline grid editing |
| UI-015 | P1 | Proposed | Declarative responsive layouts | Automated test or documented review proving declarative responsive layouts |
| UI-016 | P1 | Proposed | Keyboard-only interaction | Automated test or documented review proving keyboard-only interaction |
| UI-017 | P1 | Proposed | Accessibility audit suite | Automated test or documented review proving accessibility audit suite |
| UI-018 | P1 | Proposed | Designer undo/redo | Automated test or documented review proving designer undo/redo |
| UI-019 | P1 | Proposed | Live form preview | Automated test or documented review proving live form preview |
| UI-020 | P1 | Proposed | Preview seeded test data | Automated test or documented review proving preview seeded test data |
| UI-021 | P1 | Proposed | Selection and navigation state | Automated test or documented review proving selection and navigation state |
| UI-022 | P1 | Proposed | Visual formula editor | Automated test or documented review proving visual formula editor |
| UI-023 | P1 | Proposed | Visual validation editor | Automated test or documented review proving visual validation editor |
| UI-024 | P1 | Proposed | User-owned component extensions | Automated test or documented review proving user-owned component extensions |
| UI-025 | P1 | Proposed | Theme and localization preview | Automated test or documented review proving theme and localization preview |
| UI-026 | P1 | Proposed | Schema errors shown in IDE | Automated test or documented review proving schema errors shown in ide |
| RPT-001 | P0 | Proposed | Declarative report semantic model | Automated test or documented review proving declarative report semantic model |
| RPT-002 | P0 | Proposed | Visual layout designer | Automated test or documented review proving visual layout designer |
| RPT-003 | P0 | Proposed | Reusable report data sources | Automated test or documented review proving reusable report data sources |
| RPT-004 | P0 | Proposed | Filter and parameter prompts | Automated test or documented review proving filter and parameter prompts |
| RPT-005 | P0 | Proposed | Relational master-detail datasets | Automated test or documented review proving relational master-detail datasets |
| RPT-006 | P0 | Proposed | Sort and grouped sections | Automated test or documented review proving sort and grouped sections |
| RPT-007 | P0 | Proposed | Multiple nested group breaks | Automated test or documented review proving multiple nested group breaks |
| RPT-008 | P0 | Proposed | Aggregates and running totals | Automated test or documented review proving aggregates and running totals |
| RPT-009 | P0 | Proposed | Invoice output | Automated test or documented review proving invoice output |
| RPT-010 | P0 | Proposed | HTML preview | Automated test or documented review proving html preview |
| RPT-011 | P1 | Proposed | Paginated PDF output | Automated test or documented review proving paginated pdf output |
| RPT-012 | P1 | Proposed | CSV exports | Automated test or documented review proving csv exports |
| RPT-013 | P1 | Proposed | Print styles and paper size | Automated test or documented review proving print styles and paper size |
| RPT-014 | P1 | Proposed | Report expression evaluation | Automated test or documented review proving report expression evaluation |
| RPT-015 | P1 | Proposed | Report permission enforcement | Automated test or documented review proving report permission enforcement |
| RPT-016 | P1 | Proposed | Report snapshot and reproducibility | Automated test or documented review proving report snapshot and reproducibility |
| RPT-017 | P1 | Proposed | Report designer extensibility | Automated test or documented review proving report designer extensibility |
| RPT-018 | P1 | Proposed | Data-independent golden tests | Automated test or documented review proving data-independent golden tests |
| EXT-001 | P0 | Proposed | Typed extension manifest | Automated test or documented review proving typed extension manifest |
| EXT-002 | P0 | Proposed | Global extension scope | Automated test or documented review proving global extension scope |
| EXT-003 | P0 | Proposed | Procedure/page extension scope | Automated test or documented review proving procedure/page extension scope |
| EXT-004 | P0 | Proposed | Control extension scope | Automated test or documented review proving control extension scope |
| EXT-005 | P0 | Proposed | Report extension scope | Automated test or documented review proving report extension scope |
| EXT-006 | P0 | Proposed | Ordered model transformation pipeline | Automated test or documented review proving ordered model transformation pipeline |
| EXT-007 | P0 | Proposed | Versioned extension API | Automated test or documented review proving versioned extension api |
| EXT-008 | P0 | Proposed | Extension dependency graph | Automated test or documented review proving extension dependency graph |
| EXT-009 | P0 | Proposed | Conflict and ordering checks | Automated test or documented review proving conflict and ordering checks |
| EXT-010 | P0 | Proposed | Visual extension options | Automated test or documented review proving visual extension options |
| EXT-011 | P1 | Proposed | Enable/disable without source edits | Automated test or documented review proving enable/disable without source edits |
| EXT-012 | P1 | Proposed | Package lockfile and restore | Automated test or documented review proving package lockfile and restore |
| EXT-013 | P1 | Proposed | Extension trust policy | Automated test or documented review proving extension trust policy |
| EXT-014 | P1 | Proposed | Isolation for untrusted plugins | Automated test or documented review proving isolation for untrusted plugins |
| EXT-015 | P1 | Proposed | Observability extension | Automated test or documented review proving observability extension |
| EXT-016 | P1 | Proposed | Authorization policy extension | Automated test or documented review proving authorization policy extension |
| EXT-017 | P1 | Proposed | Extension testing harness | Automated test or documented review proving extension testing harness |
| EXT-018 | P1 | Proposed | Extension migration compatibility | Automated test or documented review proving extension migration compatibility |
| DAT-001 | P0 | Proposed | SQLite first-class provider | Automated test or documented review proving sqlite first-class provider |
| DAT-002 | P0 | Proposed | PostgreSQL provider | Automated test or documented review proving postgresql provider |
| DAT-003 | P0 | Proposed | SQL Server provider | Automated test or documented review proving sql server provider |
| DAT-004 | P0 | Proposed | EF Core adapter abstraction | Automated test or documented review proving ef core adapter abstraction |
| DAT-005 | P0 | Proposed | Optional Loom adapter contract | Automated test or documented review proving optional loom adapter contract |
| DAT-006 | P0 | Proposed | Transaction boundary definition | Automated test or documented review proving transaction boundary definition |
| DAT-007 | P0 | Proposed | Optimistic concurrency tokens | Automated test or documented review proving optimistic concurrency tokens |
| DAT-008 | P0 | Proposed | Migration diff and preview | Automated test or documented review proving migration diff and preview |
| DAT-009 | P0 | Proposed | Migration backup guidance | Automated test or documented review proving migration backup guidance |
| DAT-010 | P0 | Proposed | Foreign key enforcement | Automated test or documented review proving foreign key enforcement |
| DAT-011 | P1 | Proposed | Provider parity tests | Automated test or documented review proving provider parity tests |
| DAT-012 | P1 | Proposed | Historical snapshot pricing | Automated test or documented review proving historical snapshot pricing |
| DAT-013 | P1 | Proposed | Money precision and rounding | Automated test or documented review proving money precision and rounding |
| DAT-014 | P1 | Proposed | Bulk import/export | Automated test or documented review proving bulk import/export |
| DAT-015 | P1 | Proposed | Large blob streaming | Automated test or documented review proving large blob streaming |
| DAT-016 | P1 | Proposed | Database connection resiliency | Automated test or documented review proving database connection resiliency |
| DAT-017 | P1 | Proposed | Atomic order and order line saves | Automated test or documented review proving atomic order and order line saves |
| DAT-018 | P1 | Proposed | Seed/test fixtures | Automated test or documented review proving seed/test fixtures |
| SEC-001 | P0 | Proposed | Generated authorization policy | Automated test or documented review proving generated authorization policy |
| SEC-002 | P0 | Proposed | Roles and permissions | Automated test or documented review proving roles and permissions |
| SEC-003 | P0 | Proposed | Field-level security | Automated test or documented review proving field-level security |
| SEC-004 | P0 | Proposed | Tenant isolation policy | Automated test or documented review proving tenant isolation policy |
| SEC-005 | P0 | Proposed | Secret management | Automated test or documented review proving secret management |
| SEC-006 | P0 | Proposed | Security defaults and secure headers | Automated test or documented review proving security defaults and secure headers |
| SEC-007 | P0 | Proposed | Input encoding and XSS prevention | Automated test or documented review proving input encoding and xss prevention |
| SEC-008 | P0 | Proposed | CSRF protection where required | Automated test or documented review proving csrf protection where required |
| SEC-009 | P0 | Proposed | Audit log with actor and time | Automated test or documented review proving audit log with actor and time |
| SEC-010 | P0 | Proposed | Protected sensitive fields | Automated test or documented review proving protected sensitive fields |
| SEC-011 | P1 | Proposed | Authentication integration points | Automated test or documented review proving authentication integration points |
| SEC-012 | P1 | Proposed | Extension capability restrictions | Automated test or documented review proving extension capability restrictions |
| SEC-013 | P1 | Proposed | Dependency audit and SBOM | Automated test or documented review proving dependency audit and sbom |
| SEC-014 | P1 | Proposed | STRIDE threat model | Automated test or documented review proving stride threat model |
| TST-001 | P0 | Proposed | xUnit primary test framework | Automated test or documented review proving xunit primary test framework |
| TST-002 | P0 | Proposed | NSubstitute for mock contracts | Automated test or documented review proving nsubstitute for mock contracts |
| TST-003 | P0 | Proposed | Testcontainers for real databases | Automated test or documented review proving testcontainers for real databases |
| TST-004 | P0 | Proposed | SQLite fast repository tests | Automated test or documented review proving sqlite fast repository tests |
| TST-005 | P0 | Proposed | PostgreSQL integration tests | Automated test or documented review proving postgresql integration tests |
| TST-006 | P0 | Proposed | SQL Server integration tests | Automated test or documented review proving sql server integration tests |
| TST-007 | P0 | Proposed | Golden code generation tests | Automated test or documented review proving golden code generation tests |
| TST-008 | P0 | Proposed | Generated application compile tests | Automated test or documented review proving generated application compile tests |
| TST-009 | P0 | Proposed | Generated API contract tests | Automated test or documented review proving generated api contract tests |
| TST-010 | P0 | Proposed | Playwright browser interaction tests | Automated test or documented review proving playwright browser interaction tests |
| TST-011 | P1 | Proposed | Property-based model validation tests | Automated test or documented review proving property-based model validation tests |
| TST-012 | P1 | Proposed | Migration apply and rollback tests | Automated test or documented review proving migration apply and rollback tests |
| TST-013 | P1 | Proposed | Test data and fixtures isolation | Automated test or documented review proving test data and fixtures isolation |
| TST-014 | P1 | Proposed | Deterministic CI test matrix | Automated test or documented review proving deterministic ci test matrix |
| TST-015 | P1 | Proposed | Mutation/error injection tests | Automated test or documented review proving mutation/error injection tests |
| TST-016 | P1 | Proposed | Order-entry reference acceptance suite | Automated test or documented review proving order-entry reference acceptance suite |
| TST-017 | P1 | Proposed | Observability test exporter assertions | Automated test or documented review proving observability test exporter assertions |
| TST-018 | P1 | Proposed | Coverage and traceability report | Automated test or documented review proving coverage and traceability report |
| OPS-001 | P0 | Proposed | OpenTelemetry tracing metrics logs | Automated test or documented review proving opentelemetry tracing metrics logs |
| OPS-002 | P0 | Proposed | Local Studio without cloud requirement | Automated test or documented review proving local studio without cloud requirement |
| OPS-003 | P0 | Proposed | Offline application policy and queue | Automated test or documented review proving offline application policy and queue |
| OPS-004 | P0 | Proposed | Offline sync idempotency | Automated test or documented review proving offline sync idempotency |
| OPS-005 | P0 | Proposed | Conflict detection and resolution | Automated test or documented review proving conflict detection and resolution |
| OPS-006 | P0 | Proposed | Command-line project lifecycle | Automated test or documented review proving command-line project lifecycle |
| OPS-007 | P0 | Proposed | Versioned app deployment packaging | Automated test or documented review proving versioned app deployment packaging |
| OPS-008 | P0 | Proposed | VS Code debugging | Automated test or documented review proving vs code debugging |
| OPS-009 | P0 | Proposed | CI pipeline samples | Automated test or documented review proving ci pipeline samples |
| OPS-010 | P0 | Proposed | Documentation site and tutorial | Automated test or documented review proving documentation site and tutorial |
| OPS-011 | P1 | Proposed | Reproducible toolchain lock | Automated test or documented review proving reproducible toolchain lock |
| OPS-012 | P1 | Proposed | Cross-platform support policy | Automated test or documented review proving cross-platform support policy |
| OPS-013 | P1 | Proposed | Crash recovery and backup | Automated test or documented review proving crash recovery and backup |
| OPS-014 | P1 | Proposed | Performance benchmarks | Automated test or documented review proving performance benchmarks |
| OPS-015 | P1 | Proposed | Open-source licensing and contribution guidance | Automated test or documented review proving open-source licensing and contribution guidance |

## Traceability policy

Each accepted requirement must map to one or more test IDs (e.g. `TST-DCT-001`), an implementation PR and evidence. `Verified` requires passing CI proof; generated-app scenarios must pass both SQLite fast tests and relevant Testcontainers suites. For external services or browser interactions, record the precise environment and version.

## Architectural invariants

1. No mandatory Loom dependencies anywhere in core, Studio, generator or the default ASP.NET target.
2. All generated applications compile and run without Clarify Studio.
3. No emitted source edit is a durable customization mechanism.
4. EF Core is an adapter, not the Clarify dictionary format.
5. xUnit + NSubstitute + Testcontainers are standard testing technologies.
6. Generation is deterministic and testable offline.

## Reference acceptance scenario

The Clarion-inspired order-entry tutorial is the cross-layer acceptance fixture: customers, products, orders, order lines, lookups, validation, calculations with fixed-point money, transactionally consistent save, master-detail browse, invoice report, and regeneration-preserved customization.

## Expert review: five iterative passes (2026-10-14)

All additions below are **Proposed**; no code has been implemented or verified. Each entry inherits the traceability policy above. Passes are cumulative, and each pass addresses weaknesses identified in the previous state.

### Pass 1: RAD usability and complete developer journey
**Gap identified:** The initial plan assumes that generating CRUD is sufficient. Real users need an end-to-end no-code path, discoverability, defaults, debugging, and recoverable editing.

| ID | Priority | Status | Requirement | Acceptance approach |
|---|---|---|---|---|
| REV1-001 | P0 | Proposed | Guided zero-to-working-app experience measured in minutes | Traceable automated test or documented practical review |
| REV1-002 | P0 | Proposed | Sample application import and tutorial walkthrough | Traceable automated test or documented practical review |
| REV1-003 | P0 | Proposed | Application navigation/menu designer | Traceable automated test or documented practical review |
| REV1-004 | P0 | Proposed | Role-aware navigation generation | Traceable automated test or documented practical review |
| REV1-005 | P0 | Proposed | Bulk edit and spreadsheet-style entry | Traceable automated test or documented practical review |
| REV1-006 | P0 | Proposed | Saved searches and named filters | Traceable automated test or documented practical review |
| REV1-007 | P1 | Proposed | Keyboard shortcuts and data-entry tab order | Traceable automated test or documented practical review |
| REV1-008 | P1 | Proposed | Interactive lookup with dependent filtering | Traceable automated test or documented practical review |
| REV1-009 | P1 | Proposed | Form state preservation on validation failure | Traceable automated test or documented practical review |
| REV1-010 | P1 | Proposed | Cross-page undo/redo and project snapshots | Traceable automated test or documented practical review |
| REV1-011 | P1 | Proposed | User-customizable list columns and saved views | Traceable automated test or documented practical review |
| REV1-012 | P1 | Proposed | Context-sensitive IDE diagnostics and documentation | Traceable automated test or documented practical review |
| REV1-013 | P1 | Proposed | Import wizard with field mapping and preview | Traceable automated test or documented practical review |
| REV1-014 | P1 | Proposed | Export wizard with filtered or selected rows | Traceable automated test or documented practical review |
| REV1-015 | P1 | Proposed | Runtime localization and accessibility verification | Traceable automated test or documented practical review |
| REV1-016 | P1 | Proposed | Legacy Clarion/DOS data dictionary import feasibility study | Traceable automated test or documented practical review |

### Pass 2: Relational modeling and production DBA review
**Gap identified:** The model needs full relational semantics and deliberate treatment of physical database behavior, rather than assuming EF Core abstracts it all.

| ID | Priority | Status | Requirement | Acceptance approach |
|---|---|---|---|---|
| REV2-001 | P0 | Proposed | Natural/surrogate/composite key strategy and key change policy | Traceable automated test or documented practical review |
| REV2-002 | P0 | Proposed | Nullable unique constraints with provider parity diagnostics | Traceable automated test or documented practical review |
| REV2-003 | P0 | Proposed | Check constraints and generated column support | Traceable automated test or documented practical review |
| REV2-004 | P0 | Proposed | Provider-specific collation and case-insensitive comparison | Traceable automated test or documented practical review |
| REV2-005 | P0 | Proposed | Unicode normalization and string length semantics | Traceable automated test or documented practical review |
| REV2-006 | P0 | Proposed | Precision scale overflow and money rounding tests | Traceable automated test or documented practical review |
| REV2-007 | P1 | Proposed | Identity sequences and safe numbering under concurrency | Traceable automated test or documented practical review |
| REV2-008 | P1 | Proposed | Date-only datetime-offset UTC and timezone rules | Traceable automated test or documented practical review |
| REV2-009 | P1 | Proposed | Join entities many-to-many and relationship payload | Traceable automated test or documented practical review |
| REV2-010 | P1 | Proposed | Index include columns partial indexes and query plans | Traceable automated test or documented practical review |
| REV2-011 | P1 | Proposed | Database views and read-only entities | Traceable automated test or documented practical review |
| REV2-012 | P1 | Proposed | Computed columns and stored server expressions | Traceable automated test or documented practical review |
| REV2-013 | P1 | Proposed | Triggers stored procedures and hand-owned database objects | Traceable automated test or documented practical review |
| REV2-014 | P1 | Proposed | Transaction isolation and deadlock retry policy | Traceable automated test or documented practical review |
| REV2-015 | P1 | Proposed | Delete/archive/soft-delete and legal retention semantics | Traceable automated test or documented practical review |
| REV2-016 | P1 | Proposed | Concurrency conflict visual resolution UI | Traceable automated test or documented practical review |
| REV2-017 | P1 | Proposed | Query execution plan and missing-index inspection | Traceable automated test or documented practical review |
| REV2-018 | P1 | Proposed | Backup restore point-in-time recovery runbooks | Traceable automated test or documented practical review |
| REV2-019 | P1 | Proposed | Data migration preview backfill validation and expand-contract | Traceable automated test or documented practical review |
| REV2-020 | P1 | Proposed | Schema drift detection and out-of-band change policy | Traceable automated test or documented practical review |
| REV2-021 | P1 | Proposed | Database credential and least-privilege deployment identities | Traceable automated test or documented practical review |
| REV2-022 | P1 | Proposed | Provider-specific migration artifacts and review gates | Traceable automated test or documented practical review |

### Pass 3: Business logic, reports and operational application needs
**Gap identified:** A working business system needs more than CRUD and invoices: rules, workflow, selection, audit, scheduling, and reporting.

| ID | Priority | Status | Requirement | Acceptance approach |
|---|---|---|---|---|
| REV3-001 | P0 | Proposed | Business rules declarative expressions and typed C# escape hatch | Traceable automated test or documented practical review |
| REV3-002 | P0 | Proposed | Cross-record validation and transactional invariant enforcement | Traceable automated test or documented practical review |
| REV3-003 | P0 | Proposed | State-machine and approval workflow generation | Traceable automated test or documented practical review |
| REV3-004 | P0 | Proposed | Background jobs and retryable scheduled operations | Traceable automated test or documented practical review |
| REV3-005 | P0 | Proposed | Attachments and blob lifecycle management | Traceable automated test or documented practical review |
| REV3-006 | P0 | Proposed | Declarative search and full-text indexing | Traceable automated test or documented practical review |
| REV3-007 | P1 | Proposed | Parameterized queries and safe custom SQL escape hatch | Traceable automated test or documented practical review |
| REV3-008 | P1 | Proposed | Report datasets with SQL/expression designer | Traceable automated test or documented practical review |
| REV3-009 | P1 | Proposed | Report grouping page breaks headers footers subtotals | Traceable automated test or documented practical review |
| REV3-010 | P1 | Proposed | Label barcode QR and print-layout support | Traceable automated test or documented practical review |
| REV3-011 | P1 | Proposed | CSV Excel PDF output fidelity and limits | Traceable automated test or documented practical review |
| REV3-012 | P1 | Proposed | Report server-side execution and streaming | Traceable automated test or documented practical review |
| REV3-013 | P1 | Proposed | Async long-report job status and cancellation | Traceable automated test or documented practical review |
| REV3-014 | P1 | Proposed | Report versioning and reproducible historical outputs | Traceable automated test or documented practical review |
| REV3-015 | P1 | Proposed | Audit-history browse diff and restore permissions | Traceable automated test or documented practical review |
| REV3-016 | P1 | Proposed | Field-level permissions and row-policy enforcement | Traceable automated test or documented practical review |
| REV3-017 | P1 | Proposed | Optimistic edit conflict with merge and override policy | Traceable automated test or documented practical review |
| REV3-018 | P1 | Proposed | Notification and email integration extension seam | Traceable automated test or documented practical review |
| REV3-019 | P1 | Proposed | Bulk business actions with approval and rollback | Traceable automated test or documented practical review |
| REV3-020 | P1 | Proposed | Performance budget for 1M-row datasets and high-cardinality browses | Traceable automated test or documented practical review |

### Pass 4: Extensibility, generation, source ownership, and Loom interoperability
**Gap identified:** The existing design lacks precise guarantees around authored code, extension ordering, package compatibility, and generated app runtime independence.

| ID | Priority | Status | Requirement | Acceptance approach |
|---|---|---|---|---|
| REV4-001 | P0 | Proposed | User-owned visual layout patches survive regeneration | Traceable automated test or documented practical review |
| REV4-002 | P0 | Proposed | Round-trip visual editing with generated custom components | Traceable automated test or documented practical review |
| REV4-003 | P0 | Proposed | Regeneration three-way diff and conflict resolution | Traceable automated test or documented practical review |
| REV4-004 | P0 | Proposed | Generation rollback with transactional output staging | Traceable automated test or documented practical review |
| REV4-005 | P0 | Proposed | Public stable intermediate representation and versioning | Traceable automated test or documented practical review |
| REV4-006 | P0 | Proposed | Generator plugin API and compatibility matrix | Traceable automated test or documented practical review |
| REV4-007 | P1 | Proposed | Build-time extension isolation and permissions | Traceable automated test or documented practical review |
| REV4-008 | P1 | Proposed | Extensions contribute UI controls validators commands and reports | Traceable automated test or documented practical review |
| REV4-009 | P1 | Proposed | Extension package template gallery and version pinning | Traceable automated test or documented practical review |
| REV4-010 | P1 | Proposed | Extension dependency-cycle diagnostics | Traceable automated test or documented practical review |
| REV4-011 | P1 | Proposed | Runtime compatibility without installed Studio | Traceable automated test or documented practical review |
| REV4-012 | P1 | Proposed | Portable generated application deployment smoke test | Traceable automated test or documented practical review |
| REV4-013 | P1 | Proposed | Generated code debug-symbol and source-map coverage | Traceable automated test or documented practical review |
| REV4-014 | P1 | Proposed | Published Loom adapter version compatibility tests | Traceable automated test or documented practical review |
| REV4-015 | P1 | Proposed | Loom optional integration feature parity capability matrix | Traceable automated test or documented practical review |
| REV4-016 | P1 | Proposed | Avoid reimplementing Loom runtime services without business need | Traceable automated test or documented practical review |
| REV4-017 | P1 | Proposed | Visual designer schema upgrade and old-project migration | Traceable automated test or documented practical review |
| REV4-018 | P1 | Proposed | Git-first designer operations and meaningful merge diffs | Traceable automated test or documented practical review |

### Pass 5: Adversarial acceptance, release readiness, and requirements governance
**Gap identified:** A large list of proposed requirements isn't a test strategy or a credible beta gate; require traceable evidence and adversarial workload tests.

| ID | Priority | Status | Requirement | Acceptance approach |
|---|---|---|---|---|
| REV5-001 | P0 | Proposed | Machine-readable requirements tracker schema and validation CLI | Traceable automated test or documented practical review |
| REV5-002 | P0 | Proposed | Requirements baseline approval impact analysis and link graph | Traceable automated test or documented practical review |
| REV5-003 | P0 | Proposed | Requirement-to-test bidirectional traceability report | Traceable automated test or documented practical review |
| REV5-004 | P0 | Proposed | Objective time-to-first-working-app benchmark | Traceable automated test or documented practical review |
| REV5-005 | P0 | Proposed | Nondeveloper guided usability tests and task success criteria | Traceable automated test or documented practical review |
| REV5-006 | P0 | Proposed | Generated code code-review maintainability rubric | Traceable automated test or documented practical review |
| REV5-007 | P1 | Proposed | Database provider contract suite shared across adapters | Traceable automated test or documented practical review |
| REV5-008 | P1 | Proposed | Fault injection across transaction save and migration phases | Traceable automated test or documented practical review |
| REV5-009 | P1 | Proposed | Testcontainers PostgreSQL and SQL Server CI parity suite | Traceable automated test or documented practical review |
| REV5-010 | P1 | Proposed | NSubstitute strict expected-call-order contract tests | Traceable automated test or documented practical review |
| REV5-011 | P1 | Proposed | Property-based data dictionary and migration fuzz tests | Traceable automated test or documented practical review |
| REV5-012 | P1 | Proposed | Security authorization and tenant escape penetration tests | Traceable automated test or documented practical review |
| REV5-013 | P1 | Proposed | Accessibility keyboard screen reader browser matrix | Traceable automated test or documented practical review |
| REV5-014 | P1 | Proposed | Generated app deploy and offline-start test | Traceable automated test or documented practical review |
| REV5-015 | P1 | Proposed | Backup restore drill and corruption recovery fixture | Traceable automated test or documented practical review |
| REV5-016 | P1 | Proposed | Crash during regeneration and atomic recovery test | Traceable automated test or documented practical review |
| REV5-017 | P1 | Proposed | 10x dataset performance soak and latency regression gates | Traceable automated test or documented practical review |
| REV5-018 | P1 | Proposed | Clarion tutorial parity acceptance matrix per lesson | Traceable automated test or documented practical review |
| REV5-019 | P1 | Proposed | Dependency lock SBOM licensing and security update policy | Traceable automated test or documented practical review |
| REV5-020 | P1 | Proposed | Structured release readiness checklist and beta exit criteria | Traceable automated test or documented practical review |

## Beta quality gates

- New user can create an application with 3 related entities, perform master-detail edits, and print a grouped report without writing C#; track task completion and elapsed time.
- Generated project compiles and runs standalone from a clean checkout and does not depend on Studio or Loom.
- No data loss on power interruption during migration, regeneration or save under defined fault-injection scenarios.
- Data integrity, schema changes, money handling, concurrency and row-level authorization are verified against each supported real DB backend.
- All accepted P0 requirements have linked executable tests, review evidence or documented waivers; coverage statistics are reported honestly.

## Bidirectional designer and positioning requirements

Specification: [CLARIFY-DSL-BIDIRECTIONAL.md](CLARIFY-DSL-BIDIRECTIONAL.md). All below Proposed and unverified.

| ID | Priority | Status | Requirement | Acceptance evidence |
|---|---|---|---|---|
| DSL-001 | P0 | Proposed | Bidirectional DSL and visual designer synchronization | Round-trip source/visual test tied to DSL-001 |
| DSL-002 | P0 | Proposed | Dictionary-bound WYSIWYG preview | Round-trip source/visual test tied to DSL-002 |
| DSL-003 | P0 | Proposed | Grid row and column position attributes | Round-trip source/visual test tied to DSL-003 |
| DSL-004 | P0 | Proposed | Grid spans and resizing handles | Round-trip source/visual test tied to DSL-004 |
| DSL-005 | P0 | Proposed | Container drag and drop with structural source edits | Round-trip source/visual test tied to DSL-005 |
| DSL-006 | P0 | Proposed | Responsive breakpoint source rules | Round-trip source/visual test tied to DSL-006 |
| DSL-007 | P0 | Proposed | Constrained width height min max properties | Round-trip source/visual test tied to DSL-007 |
| DSL-008 | P0 | Proposed | Print canvas absolute positions and physical units | Round-trip source/visual test tied to DSL-008 |
| DSL-009 | P0 | Proposed | Lossless source trivia and comment preservation | Round-trip source/visual test tied to DSL-009 |
| DSL-010 | P0 | Proposed | Minimal source patches on visual edits | Round-trip source/visual test tied to DSL-010 |
| DSL-011 | P0 | Proposed | Source selection and UI selection synchronization | Round-trip source/visual test tied to DSL-011 |
| DSL-012 | P0 | Proposed | Incremental parse preview with invalid text | Round-trip source/visual test tied to DSL-012 |
| DSL-013 | P0 | Proposed | Atomic undo redo across code and visual editing | Round-trip source/visual test tied to DSL-013 |
| DSL-014 | P0 | Proposed | Stable control IDs across movement and rename | Round-trip source/visual test tied to DSL-014 |
| DSL-015 | P1 | Proposed | External file revision conflict and merge handling | Round-trip source/visual test tied to DSL-015 |
| DSL-016 | P1 | Proposed | Source formatting and deterministic output | Round-trip source/visual test tied to DSL-016 |
| DSL-017 | P1 | Proposed | Keyboard and accessibility warnings for positioning | Round-trip source/visual test tied to DSL-017 |
| DSL-018 | P1 | Proposed | Playwright drag edit roundtrip test | Round-trip source/visual test tied to DSL-018 |
| DSL-019 | P1 | Proposed | xUnit property-based parse edit reparse tests | Round-trip source/visual test tied to DSL-019 |
| DSL-020 | P1 | Proposed | NSubstitute edit transaction order tests | Round-trip source/visual test tied to DSL-020 |

## User-authored .NET and durable workflows

[Detailed specification](CUSTOM-CSHARP-AND-WORKFLOWS.md). Proposed only; not implemented or verified end-to-end.

| ID | Priority | Status | Requirement | Acceptance evidence |
|---|---|---|---|---|
| CS-001 | P0 | Proposed | Typed user-owned C# command handlers | Traceable automated integration or review evidence |
| CS-002 | P0 | Proposed | Typed validators and formula extension points | Traceable automated integration or review evidence |
| CS-003 | P0 | Proposed | Generate handler and xUnit skeleton through Studio | Traceable automated integration or review evidence |
| CS-004 | P0 | Proposed | Detect missing and duplicate DI registrations | Traceable automated integration or review evidence |
| CS-005 | P0 | Proposed | Preserve user C# across regeneration | Traceable automated integration or review evidence |
| CS-006 | P0 | Proposed | VS Code and Visual Studio debugging launch profile | Traceable automated integration or review evidence |
| CS-007 | P0 | Proposed | Diagnostics link DSL action names to C# symbols | Traceable automated integration or review evidence |
| CS-008 | P0 | Proposed | Compile without Clarify Studio or Loom | Traceable automated integration or review evidence |
| CS-009 | P1 | Proposed | Safe local compilation permissions | Traceable automated integration or review evidence |
| CS-010 | P1 | Proposed | Custom workflow step contract | Traceable automated integration or review evidence |
| WF-001 | P0 | Proposed | Embedded durable workflow runtime adapter | Traceable automated integration or review evidence |
| WF-002 | P0 | Proposed | Persist execution state and workflow history | Traceable automated integration or review evidence |
| WF-003 | P0 | Proposed | Resume after host restart | Traceable automated integration or review evidence |
| WF-004 | P0 | Proposed | Immutable version-pinned workflow definitions | Traceable automated integration or review evidence |
| WF-005 | P0 | Proposed | Wait for correlated external events | Traceable automated integration or review evidence |
| WF-006 | P0 | Proposed | Human approval tasks and permissions | Traceable automated integration or review evidence |
| WF-007 | P0 | Proposed | Persisted timers retries and escalation | Traceable automated integration or review evidence |
| WF-008 | P0 | Proposed | Idempotent step execution and durable outbox | Traceable automated integration or review evidence |
| WF-009 | P1 | Proposed | Lease and concurrency protections | Traceable automated integration or review evidence |
| WF-010 | P1 | Proposed | Retry failure dead-letter handling | Traceable automated integration or review evidence |
| WF-011 | P1 | Proposed | Audit log of actors and actions | Traceable automated integration or review evidence |
| WF-012 | P1 | Proposed | Offline local execution and eventual synchronization | Traceable automated integration or review evidence |
| WF-013 | P1 | Proposed | OpenTelemetry workflow events | Traceable automated integration or review evidence |
| WF-014 | P1 | Proposed | Workflow step authoring in user-owned C# | Traceable automated integration or review evidence |
| WF-015 | P1 | Proposed | DSL workflow grammar and diagnostics | Traceable automated integration or review evidence |
| WF-016 | P1 | Proposed | Visual workflow design and definition roundtrip | Traceable automated integration or review evidence |
| WF-017 | P1 | Proposed | Workflow monitoring and operator controls | Traceable automated integration or review evidence |
| WF-018 | P1 | Proposed | Provider parity under Testcontainers | Traceable automated integration or review evidence |
| WF-019 | P1 | Proposed | Crash/restart acceptance test | Traceable automated integration or review evidence |
| WF-020 | P1 | Proposed | Backwards-compatible workflow version upgrades | Traceable automated integration or review evidence |

## Full Elsa interoperability (ELSA requirements)

All items are proposed until a linked passing test verifies them. Refer to DESIGN.md §13.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| ELSA-001 | P0 | Proposed | Reference any registered Elsa native activity by qualified type | Automated parser/adapter/runtime contract test |
| ELSA-002 | P0 | Proposed | Bind external versioned configuration to native activity nodes | Automated parser/adapter/runtime contract test |
| ELSA-003 | P0 | Proposed | Reject path traversal in native activity references | Automated parser/adapter/runtime contract test |
| ELSA-004 | P0 | Proposed | Preserve opaque native configuration through visual editing | Automated parser/adapter/runtime contract test |
| ELSA-005 | P0 | Proposed | Check Elsa activity metadata against installed packages | Automated parser/adapter/runtime contract test |
| ELSA-006 | P0 | Proposed | Support Elsa native inputs outputs variables and expressions | Automated parser/adapter/runtime contract test |
| ELSA-007 | P1 | Proposed | Support sequences decisions and condition branches | Automated parser/adapter/runtime contract test |
| ELSA-008 | P1 | Proposed | Support parallel forks and joins | Automated parser/adapter/runtime contract test |
| ELSA-009 | P1 | Proposed | Support timers delays schedules and events | Automated parser/adapter/runtime contract test |
| ELSA-010 | P1 | Proposed | Support exception handlers and compensation | Automated parser/adapter/runtime contract test |
| ELSA-011 | P1 | Proposed | Support workflow composition and subprocesses | Automated parser/adapter/runtime contract test |
| ELSA-012 | P1 | Proposed | Support first-party and third-party Elsa activities | Automated parser/adapter/runtime contract test |
| ELSA-013 | P1 | Proposed | Version pin plugin and workflow dependencies | Automated parser/adapter/runtime contract test |
| ELSA-014 | P1 | Proposed | Roundtrip Elsa Studio definitions without silent loss | Automated parser/adapter/runtime contract test |
| ELSA-015 | P1 | Proposed | Generate typed C# activity registration | Automated parser/adapter/runtime contract test |
| ELSA-016 | P1 | Proposed | Execute mixed native and Clarify C# activities | Automated parser/adapter/runtime contract test |
| ELSA-017 | P1 | Proposed | Verify durable bookmark restart and resume | Automated parser/adapter/runtime contract test |
| ELSA-018 | P1 | Proposed | Test parallel workflow recovery and deduplication | Automated parser/adapter/runtime contract test |
| ELSA-019 | P1 | Proposed | Native node configuration in visual properties UI | Automated parser/adapter/runtime contract test |
| ELSA-020 | P1 | Proposed | Diagnostics for unsupported Elsa nodes and versions | Automated parser/adapter/runtime contract test |

## Application-scoped identity and permissions

Status: proposed until runtime tests and enforcement across generated pages. Design: [IDENTITY-AND-APP-PERMISSIONS.md](IDENTITY-AND-APP-PERMISSIONS.md).

| ID | Priority | Status | Requirement | Acceptance evidence |
|---|---|---|---|---|
| AUTH-001 | P0 | Proposed | Stable host identity with optional OIDC | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-002 | P0 | Proposed | Application registry with immutable IDs | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-003 | P0 | Proposed | Application-scoped membership and disable | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-004 | P0 | Proposed | Application-scoped custom roles | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-005 | P0 | Proposed | Application-scoped permission grants | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-006 | P0 | Proposed | Multiple roles per application user | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-007 | P0 | Proposed | Server-side deny-by-default policy evaluation | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-008 | P0 | Proposed | Record-level access predicates | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-009 | P0 | Proposed | Tenant boundary enforcement | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-010 | P0 | Proposed | Workflow assignment and candidate-group mapping | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-011 | P1 | Proposed | Prohibit self-approval when specified | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-012 | P1 | Proposed | Role and membership administration UI | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-013 | P1 | Proposed | Permission change audit history | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-014 | P1 | Proposed | Secure first administrator onboarding | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-015 | P1 | Proposed | Authenticated application data APIs | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-016 | P1 | Proposed | Permission-aware generated forms and menus | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-017 | P1 | Proposed | Identity-aware Testcontainers acceptance tests | xUnit authorization tests, web API denial tests and audit evidence |
| AUTH-018 | P1 | Proposed | No privileged role implicit cross-application access | xUnit authorization tests, web API denial tests and audit evidence |

| AUTH-019 | P0 | Proposed | Application-configurable Public, OptionalLogin or LoginRequired mode | Route and authorization integration tests |
| AUTH-020 | P0 | Proposed | Public mode cannot bypass protected administrative or workflow actions | Unauthorized HTTP mutation tests |
| AUTH-021 | P1 | Proposed | Anonymous commands require explicit policy, abuse protection and rate limiting | Security tests |

## Observability and OpenTelemetry — implementation increment

These requirements describe the target; instrumentation beyond HTTP/Customer save remains proposed. No passing .NET build was observed in this environment.

| ID | Priority | Status | Requirement | Acceptance evidence |
|---|---|---|---|---|
| OTEL-001 | P0 | Implemented (unverified) | OpenTelemetry-based tracing with stable ActivitySource | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-002 | P0 | Implemented (unverified) | HTTP request tracing without secret/header capture | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-003 | P0 | Implemented (unverified) | Generated form-save spans and duration metrics | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-004 | P0 | Proposed | Structured telemetry for validation and save failures | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-005 | P0 | Proposed | Metrics for code-generation duration and outcomes | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-006 | P0 | Implemented (unverified) | Optional OTLP trace and metric export | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-007 | P0 | Implemented (unverified) | Offline operation with no Collector configured | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-008 | P0 | Proposed | Prohibit field values, passwords, secrets and SQL parameters in telemetry | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-009 | P0 | Proposed | Consistent per-application service and operation names | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-010 | P1 | Proposed | Trace context propagation across generated commands | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-011 | P1 | Proposed | Integration with Elsa built-in tracing without duplicate spans | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-012 | P1 | Proposed | Studio diagnostics and correlated generation/build failures | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-013 | P1 | Proposed | Audit trail distinct from telemetry | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-014 | P1 | Proposed | Unit tests of trace and metric emission | xUnit tracing and metric tests, OTLP smoke, security review |
| OTEL-015 | P1 | Proposed | Collector-backed integration tests and redaction assertions | xUnit tracing and metric tests, OTLP smoke, security review |

## Template ecosystem requirements

| ID | Priority | Status | Requirement | Acceptance evidence |
|---|---|---|---|---|
| TPL-001 | P0 | Implemented (unverified) | Versioned declarative template manifest and typed bindings | Deterministic Node tests |
| TPL-002 | P0 | Implemented (unverified) | Validate required dictionary relationships before template application | Positive/negative tests |
| TPL-003 | P0 | Implemented (unverified) | Studio template preview and downloadable ClarifyDSL | Browser interaction tests pending |
| TPL-004 | P0 | Proposed | Round-trip generated form DSL into visual editor | Playwright editing test |
| TPL-005 | P0 | Proposed | Regeneration preserves user-owned C# and visual overrides | Golden regeneration tests |
| TPL-006 | P0 | Proposed | Generate actual nested master-detail grid and CRUD | Functional SQLite acceptance |
| TPL-007 | P1 | Proposed | Dependency/version compatibility and update diff | Migration conflict tests |
| TPL-008 | P1 | Proposed | Data/application/report template categories | End-to-end templates |
| TPL-009 | P1 | Proposed | Offline package library and opt-in trusted NuGet extensions | Security tests |
| TPL-010 | P0 | Proposed | Stable generated element IDs and instance metadata | Snapshot and diff tests |

## Async-first generated application requirements

Async is the default for I/O and cancellation is propagated. This is not a promise of faster individual SQL queries. C# authors can still implement synchronous calculations and custom components.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| ASYNC-001 | P0 | Implemented (unverified) | Async-first generated database access | Generated code, xUnit and load tests |
| ASYNC-002 | P0 | Implemented (unverified) | Propagate CancellationToken through generated APIs | Generated code, xUnit and load tests |
| ASYNC-003 | P0 | Implemented (unverified) | Bounded stable asynchronous browse paging | Generated code, xUnit and load tests |
| ASYNC-004 | P0 | Implemented (unverified) | Cancel obsolete Blazor browse requests | Generated code, xUnit and load tests |
| ASYNC-005 | P0 | Proposed | Async validation and extension hooks with ordered execution | Generated code, xUnit and load tests |
| ASYNC-006 | P0 | Proposed | No blocking .Result or Wait in generated I/O paths | Generated code, xUnit and load tests |
| ASYNC-007 | P1 | Proposed | Async report streaming and bounded memory | Generated code, xUnit and load tests |
| ASYNC-008 | P1 | Proposed | Background jobs with durable queue and status | Generated code, xUnit and load tests |
| ASYNC-009 | P1 | Proposed | Load tests measuring concurrency, throughput and latency | Generated code, xUnit and load tests |
| ASYNC-010 | P1 | Implemented (unverified) | Async-safe DbContext lifetime and concurrency | Generated code, xUnit and load tests |
| ASYNC-011 | P1 | Proposed | Telemetry for cancellation and request latency | Generated code, xUnit and load tests |
| ASYNC-012 | P1 | Proposed | Graceful shutdown and cancellation for long jobs | Generated code, xUnit and load tests |

## Clarion-inspired helper and expression requirements

Design: [CLARION-LANGUAGE-HELPERS.md](CLARION-LANGUAGE-HELPERS.md). These are proposed, not implemented.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| EXPR-001 | P0 | Proposed | Typed safe expression AST with diagnostics | Typed expression, SQL translation, security and parity tests |
| EXPR-002 | P0 | Proposed | Culture-aware format and parse helpers | Typed expression, SQL translation, security and parity tests |
| EXPR-003 | P0 | Proposed | String/date/conditional helper functions | Typed expression, SQL translation, security and parity tests |
| EXPR-004 | P0 | Proposed | Query expressions translated to server-side SQL | Typed expression, SQL translation, security and parity tests |
| EXPR-005 | P0 | Proposed | Consistent expression semantics in Studio and generated apps | Typed expression, SQL translation, security and parity tests |
| EXPR-006 | P0 | Proposed | Registered typed user C# helper functions | Typed expression, SQL translation, security and parity tests |
| EXPR-007 | P0 | Proposed | Explicit null and money rounding semantics | Typed expression, SQL translation, security and parity tests |
| EXPR-008 | P0 | Proposed | No arbitrary code execution from DSL expressions | Typed expression, SQL translation, security and parity tests |
| EXPR-009 | P1 | Proposed | Benchmark interpreted versus generated compiled expressions | Typed expression, SQL translation, security and parity tests |
| EXPR-010 | P2 | Proposed | Legacy Clarion data conversion adapters isolated | Typed expression, SQL translation, security and parity tests |

## Standard domain value types

Design: [STANDARD-VALUE-TYPES.md](STANDARD-VALUE-TYPES.md). Status reflects committed source, not observed passing builds.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| TYPE-001 | P0 | Implemented (unverified) | NodaMoney and UnitsNet available in all generated .NET applications | EF Core persistence, generator and provider tests |
| TYPE-002 | P0 | Implemented (unverified) | Money persists amount and ISO currency separately | EF Core persistence, generator and provider tests |
| TYPE-003 | P0 | Implemented (unverified) | Physical quantities persist canonical units | EF Core persistence, generator and provider tests |
| TYPE-004 | P0 | Implemented (unverified) | Dictionary supports money length mass pressure temperature | EF Core persistence, generator and provider tests |
| TYPE-005 | P0 | Implemented (unverified) | Typed EF Core persistence survives database reopen | EF Core persistence, generator and provider tests |
| TYPE-006 | P0 | Proposed | Generated forms display preferred currency and measurement units | EF Core persistence, generator and provider tests |
| TYPE-007 | P1 | Proposed | SQL filtering/sorting on persisted scalar values | EF Core persistence, generator and provider tests |
| TYPE-008 | P1 | Proposed | Explicit rounding and currency-conversion policies | EF Core persistence, generator and provider tests |
| TYPE-009 | P1 | Proposed | Migrations from legacy cents storage preserve values | EF Core persistence, generator and provider tests |
| TYPE-010 | P1 | Proposed | Provider parity tests for SQLite PostgreSQL SQL Server | EF Core persistence, generator and provider tests |

## Standard form controls and validation

Design: [FIELD-CONTROLS-AND-VALIDATION.md](FIELD-CONTROLS-AND-VALIDATION.md). Current implementation is partial and has not passed observed CI.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| CTRL-001 | P0 | Implemented (unverified) | Standard text, multiline, number, date and checkbox editors | DSL, generator, xUnit, browser and security tests |
| CTRL-002 | P0 | Implemented (unverified) | Dropdown, radio and button-selection controls | DSL, generator, xUnit, browser and security tests |
| CTRL-003 | P0 | Implemented (unverified) | Dictionary and DSL-driven control type inference and overrides | DSL, generator, xUnit, browser and security tests |
| CTRL-004 | P0 | Implemented (unverified) | Two-way graphical control editor with metadata preservation | DSL, generator, xUnit, browser and security tests |
| CTRL-005 | P0 | Implemented (unverified) | Choice list validation and type compatibility | DSL, generator, xUnit, browser and security tests |
| CTRL-006 | P0 | Implemented (unverified) | Min/max numeric and maxLength string validation metadata | DSL, generator, xUnit, browser and security tests |
| CTRL-007 | P0 | Implemented (unverified) | Generated Blazor input controls and ValidationMessage wiring | DSL, generator, xUnit, browser and security tests |
| CTRL-008 | P0 | Proposed | Authoritative server-side generated validation for all configured rules | DSL, generator, xUnit, browser and security tests |
| CTRL-009 | P0 | Proposed | Relational searchable lookups with async paging and authorization | DSL, generator, xUnit, browser and security tests |
| CTRL-010 | P1 | Proposed | Cross-field and asynchronous custom C# validation hooks | DSL, generator, xUnit, browser and security tests |
| CTRL-011 | P1 | Proposed | NodaMoney and UnitsNet input widgets with conversion and precision | DSL, generator, xUnit, browser and security tests |
| CTRL-012 | P1 | Proposed | Accessibility and keyboard tests for choice controls | DSL, generator, xUnit, browser and security tests |

## Database dependency requirements

See docs/DATABASE-DEPENDENCIES.md. Implementation is source-level only until verified.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| DEP-001 | P0 | Implemented | Dependency graph for entities, fields and relationships | Dependency tests and database integration |
| DEP-002 | P0 | Implemented | Validate native and external database references | Dependency tests and database integration |
| DEP-003 | P0 | Implemented | Reject cascade cycles and invalid set-null rules | Dependency tests and database integration |
| DEP-004 | P0 | Implemented | Validate registered database providers and logical IDs | Dependency tests and database integration |
| DEP-005 | P0 | Implemented | Preview schema changes and affected dependencies | Dependency tests and database integration |
| DEP-006 | P0 | Proposed | Generate reviewed EF Core foreign keys and migrations | Dependency tests and database integration |
| DEP-007 | P0 | Proposed | Implement external reference integrity and reconciliation | Dependency tests and database integration |
| DEP-008 | P1 | Proposed | Include forms reports workflows and C# in impact analysis | Dependency tests and database integration |
| DEP-009 | P1 | Proposed | Generate dependent async lookups | Dependency tests and database integration |
| DEP-010 | P1 | Proposed | Stable IDs and safe rename migration | Dependency tests and database integration |
| DEP-011 | P1 | Proposed | Database rollback and provider integration tests | Dependency tests and database integration |

## EF Core relational source generation

Implementation: designer/ef-models.mjs; example: examples/relational-generation. Tests: tests/ef-generation.test.mjs and relational SQLite smoke. Source implementation is not equivalent to observed green CI.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| EFGEN-001 | P0 | Implemented | Generate EF Core POCO entities and per-database DbContext | Node golden tests and SQLite .NET smoke |
| EFGEN-002 | P0 | Implemented | Generate native foreign keys and delete behaviors | Node golden tests and SQLite .NET smoke |
| EFGEN-003 | P0 | Implemented | Generate FK indexes and one-to-one uniqueness | Node golden tests and SQLite .NET smoke |
| EFGEN-004 | P0 | Implemented | Reject missing keys, incompatible types and cross-database native FKs | Node golden tests and SQLite .NET smoke |
| EFGEN-005 | P0 | Implemented | Deterministic generated C# and Studio preview | Node golden tests and SQLite .NET smoke |
| EFGEN-006 | P0 | In Progress | SQLite relational integration smoke and CI build | Node golden tests and SQLite .NET smoke |
| EFGEN-007 | P0 | Proposed | Generate NodaMoney and UnitsNet relational mappings | Node golden tests and SQLite .NET smoke |
| EFGEN-008 | P0 | Proposed | Generate reviewed EF Core migrations and rollback plan | Node golden tests and SQLite .NET smoke |
| EFGEN-009 | P1 | In Progress | Preserve user-authored C# and avoid destructive regeneration | Node golden tests and SQLite .NET smoke |
| EFGEN-010 | P1 | Proposed | Support composite keys and alternate principal keys | Node golden tests and SQLite .NET smoke |

## Reviewed database migration requirements

See docs/SQLITE-MIGRATIONS.md. Implemented means committed source, not verified production operation.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| MIG-001 | P0 | Implemented | Plan nullable SQLite column additions without executing SQL | Node planner tests, SQLite integration and recovery drills |
| MIG-002 | P0 | Implemented | Refuse destructive or unsupported migration changes | Node planner tests, SQLite integration and recovery drills |
| MIG-003 | P0 | Implemented | Require explicit plan ID and existing database | Node planner tests, SQLite integration and recovery drills |
| MIG-004 | P0 | Implemented | Validate SQLite schema before applying planned changes | Node planner tests, SQLite integration and recovery drills |
| MIG-005 | P0 | Implemented | Create online backup and transactional migration journal | Node planner tests, SQLite integration and recovery drills |
| MIG-006 | P0 | In Progress | Pass real SQLite migration smoke in CI and verify restore | Node planner tests, SQLite integration and recovery drills |
| MIG-007 | P0 | Proposed | Immutable dictionary IDs and rename-safe migration snapshots | Node planner tests, SQLite integration and recovery drills |
| MIG-008 | P0 | Proposed | Generate provider-specific EF Core migrations for PostgreSQL/SQL Server | Node planner tests, SQLite integration and recovery drills |
| MIG-009 | P1 | Proposed | Secure migration approval and deployment audit UI | Node planner tests, SQLite integration and recovery drills |
| MIG-010 | P1 | Proposed | Concurrency hardening, restore automation and rollback policy | Node planner tests, SQLite integration and recovery drills |

## C# XML API documentation and Studio finish gates

See docs/CSHARP-DOCUMENTATION.md; issue #41 tracks outstanding documentation coverage. 'Implemented' is source-level only until CI passes.

| ID | Priority | Status | Requirement | Verification |
|---|---|---|---|---|
| DOC-001 | P0 | Implemented (unverified) | All .NET projects emit compiler XML documentation files | DocFX, CS1591 and generator/browser tests |
| DOC-002 | P0 | Implemented (unverified) | DocFX generates HTML API reference from C# XML documentation | DocFX, CS1591 and generator/browser tests |
| DOC-003 | P0 | Implemented (unverified) | EF Core, async browse and Razor generators emit XML comments | DocFX, CS1591 and generator/browser tests |
| DOC-004 | P0 | Implemented (unverified) | Auditor inventories undocumented handwritten and generated C# APIs | DocFX, CS1591 and generator/browser tests |
| DOC-005 | P0 | In Progress | Every public/protected handwritten API has meaningful documentation | DocFX, CS1591 and generator/browser tests |
| DOC-006 | P0 | In Progress | All generated C# and extension scaffolds have complete XML comments | DocFX, CS1591 and generator/browser tests |
| DOC-007 | P0 | Proposed | Close CS1591 warnings and progressively enforce as CI errors | DocFX, CS1591 and generator/browser tests |
| DOC-008 | P1 | In Progress | Publish versioned browsable API docs and end-to-end author manual | DocFX, CS1591 and generator/browser tests |
| DOC-009 | P0 | In Progress | Playwright validates ER diagram drag, zoom, navigation and SVG download | DocFX, CS1591 and generator/browser tests |
