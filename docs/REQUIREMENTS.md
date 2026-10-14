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
