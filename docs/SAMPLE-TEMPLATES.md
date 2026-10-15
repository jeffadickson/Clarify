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

## Version 0.2 — standard rich value types

All 20 sample template manifests have been advanced to version **0.2.0** and declare NodaMoney and UnitsNet as the standard semantic type providers. This is a metadata-driven capability, not a new interpreter.

The sample dictionary explicitly defines USD for Product.Price and OrderLine.UnitPrice, and adds Product.Length, Product.Mass, Product.Pressure and Product.Temperature with preferred display units. The Product Editor, Product Pricing and Product Catalog templates demonstrate those fields; the OrderLine pricing template demonstrates money.

The template engine reads each bound field's semantic type from the dictionary and emits `// @clarify-type` metadata adjacent to its ClarifyDSL declaration. For example:

```text
// @clarify-type {"field":"Price","type":"money","dotnet":"NodaMoney.Money","storage":["amount","currency"],"currency":"USD"}
Price at (1,1) span (6,1)
```

The form designer displays currency/unit hints on the canvas, and the dictionary editor accepts currency and display-unit metadata for newly added fields. Visual movement preserves the metadata comments.

**Not yet implemented:** NodaMoney/UnitsNet input controls, conversion/rounding policy, automatic generated EF Core mappings and migrations from these template declarations. Do not confuse metadata propagation with fully operational strongly typed UI or database generation. The earlier standalone EF Core example and tests remain the persistence reference.

## Additional regression templates — gallery total 40

Twenty more versioned `clarify-template/v1` packages (version `0.2.0`) are committed in `templates/samples`, mirrored into Studio assets and registered in `sample-template-index.json`.

- **Customer (4):** quick card, city directory, contact lookup, data review
- **Product (8):** measurements, dimensions, mass, pressure, thermal specification, price tag, technical specification, unit conversion preview
- **Order (4):** customer lookup, date entry, review, batch browse
- **OrderLine (4):** quantity inspection, unit price, item review, cost summary

The samples exercise multiple grid widths, different field ordering and the money/length/mass/pressure/temperature semantic metadata. Existing catalog tests now expect **40** manifests, instantiate each template against the canonical dictionary, parse generated DSL and compare its Studio-served copy. A separate gallery count test verifies uniqueness.

These are *test data and layout-generation examples*. Names such as "unit conversion preview" and "batch browse" do not imply that conversion widgets or browse/query execution are implemented. This is deliberate coverage for future typed UI, browse, template upgrade and C# preservation tests.
