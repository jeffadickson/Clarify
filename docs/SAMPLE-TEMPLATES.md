# Clarify sample template gallery

Twenty versioned sample packages live at `templates/samples/*.json`. They are displayed at `/studio/templates.html`, with bindable entities, generated DSL preview, DSL download and Edit visually links.

| Category | Samples | Level of implementation |
|---|---|---|
| Customer | Contact card, address editor, directory, search, intake, profile | Editable ClarifyDSL layouts |
| Product | Editor, pricing, catalog, search | Editable ClarifyDSL layouts |
| Order | Entry, summary, browse, filter | Editable ClarifyDSL layouts |
| Order line | Editor, pricing, browse, fulfillment | Editable ClarifyDSL layouts |
| Relationships | Customer orders, order items | Two generated form layouts with relationship validation |

**Honest capability boundary:** browse templates currently generate form layouts, not functional sortable/paged browse tables; the two master-detail templates do not generate nested grids; the search/filter templates do not generate query engines or server handlers. These are 20 real parameterized, schema-bound **sample template manifests** and working editable DSL outputs, not 20 end-to-end completed applications.

Every package has an ID, semantic version, category, entity bindings, selected fields, grid configuration and source provenance. Custom C# remains author-owned, outside regenerated sources. Tests verify all 20 manifests instantiate and parse and match their Studio-served counterparts.

## Run
Open `/studio/templates.html`, choose a template, select its bound entity (and detail entity when applicable), click Generate preview, then Edit visually. Download customized DSL from the designer.

Next: richer typed recipes for list/browse, nested master-detail grids, query/filter widgets, and safe persistence of template instances/overrides.
