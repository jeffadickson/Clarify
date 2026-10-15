# Clarify Studio — Authoring Guide (working development preview)

This guide covers the existing Studio workflow. It describes committed source
and does **not** imply the entire generated application pipeline is production-ready.

## 1. Open Studio

Start the ASP.NET Core demo from the repository's verification/setup guide.
Open `/studio` for the form/DSL editor and `/studio/schema` for the graphical
dictionary. Both are preserved as independent, complementary editors.

## 2. Edit data definitions

The dictionary designer lets you add entities, typed fields, required flags
and relationships. Its local working draft persists in browser storage.
Export `dictionary.json` explicitly before committing changes. Production
database schemas are **not** modified by dictionary edits.

## 3. Plot relationships

Open `/studio/er-diagram.html` from the dictionary page.
Use **Auto layout** to place parents before dependent tables, +/− to zoom,
**Fit** to restore the viewport, and drag empty canvas to pan.
Drag a table or focus it and press arrow keys to adjust its position.
Table positions are stored only in the current browser; **Reset positions**
clears them. Native foreign keys are solid, cross-database references dashed.
Use **Export SVG** for a portable image.

## 4. Use templates and edit forms

Open `/studio/templates.html` to select from the 40 sample templates.
Bind entities, generate preview DSL and choose **Edit visually**.
Studio preserves the existing text and visual form editing functionality;
user-authored C# is not overwritten by template preview.

## 5. Generate relational model source

From the dictionary, **Preview generated EF Core** can output entity and
DbContext source for dictionaries with explicit supported keys/types.
Unsupported money/unit mappings are rejected rather than incorrectly generated.
Production migration execution is separate and requires review.

## 6. Review database changes

For the supported SQLite example, load the relational demo and add an optional
field; preview the narrow additive migration plan. The CLI separately requires
a plan ID, expected schema and a new SQLite backup. Read
[SQLite migration safety](SQLITE-MIGRATIONS.md).

## 7. Native DSL evaluation

The Parlot-based C# parser is experimental. It is evaluated against
40 template fixtures and handwritten forms. Studio's existing browser parser
remains authoritative until compatibility tests pass.
See [native parser](PARLOT-EXPERIMENT.md).

## Feature maturity and verification

| Area | Source maturity |
|---|---|
| Visual dictionary and ER diagram | Implemented prototypes |
| Relationship auto layout and navigation | Implemented; browser testing pending |
| Template gallery with 40 sample manifests | Implemented; full business behavior incomplete |
| Native C# Parlot parser | Experimental, not replacing Studio |
| Generated EF Core mappings | Supported scalar/FK subset |
| SQLite schema migration | Explicit nullable-column additions only |
| End-to-end production app generation | In progress |

Verification commands are in `scripts/verify.sh`,
`scripts/verify.ps1` and `.github/workflows/shared-blazor.yml`.
No completed green .NET or Playwright run has been confirmed in this increment.

## Related issues

- #38 Native Parlot parser compatibility and source editing
- #39 ER diagram usability and tests
- #40 Documentation quality and verified walkthrough
