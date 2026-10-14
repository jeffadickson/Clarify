# Clarify M0 browser demo

Open [index.html](index.html) in Chromium, Edge, Chrome or Firefox. No server, npm, database or internet connection is needed.

## Demonstrated
- Application navigation, generated-style customer/product browses, search-as-you-type, add/edit/delete.
- Master-detail order entry with customer lookup and line-item product lookup.
- Order-line price snapshots; sums in integer cents for demo totals.
- Referential delete protection for existing order references.
- Simple HTML invoice preview and print.
- Browser localStorage persistence, reset sample data.
- Semantic field metadata editor and illustrative C# entity source preview.
- Fields and HTML text are escaped when dynamically rendered.

## Important limitations (not production-ready)
- The field editor does **not** regenerate running forms or migrate existing data.
- C# is **preview-only**; no EF Core model, database migration or compilable solution is emitted.
- Browser localStorage is not transactional multi-user persistence, offline sync, or a production database.
- Money is stored as JavaScript Number, not production-grade fixed decimal with currency semantics; only total calculation uses integer cents.
- Browser test automation and C# integration tests are not yet executed or connected to CI.
- Orders cannot currently be deleted. Adding a line closes the order editor; reopen the order to see it.
- No authentication, authorization, audit, concurrency control, schema migration or production report rendering.

## M0 acceptance progression
- **M0-A Browser demo:** initial source delivered; manual browser validation pending.
- **M0-B Buildable .NET generator:** open; must generate, compile, run and integrate SQLite.
- **M0-C Test suite:** xUnit + NSubstitute + Testcontainers for .NET; scaffold committed but execution pending until .NET SDK and container runtime are available.
- **M0-D Usability:** complete one developer journey from dictionary to runtime form and invoice with verified automation.

The phrase "demo stage" is met as a static proof of interaction, **not** a production capable RAD tool. Do not treat this as completion of the original M0 full acceptance gates.
