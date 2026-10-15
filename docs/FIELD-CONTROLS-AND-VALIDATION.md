# Field control registry and validation — first implementation

## Implemented
- `designer/controls.mjs` is the shared control registry: text, multiline text, numeric entry, date, checkbox, dropdown, radio, and button selection.
- `designer/clarifydsl.mjs` parses declarative `// @clarify-control` field metadata and validates type compatibility and choice lists.
- Studio's field inspector selects control type, comma-separated choices, text max length and numeric min/max; changes round-trip to ClarifyDSL without changing positions or user C#.
- `tools/control-renderer.mjs` generates typed Blazor `InputText`, `InputTextArea`, `InputNumber`, `InputDate`, `InputCheckbox`, `InputSelect`, and `InputRadioGroup` markup.
- `tools/generate-shared-blazor.mjs` uses this renderer for Customer fields. Its existing DataAnnotations and `ValidationMessage` wiring remains intact.
- Templates may specify `forms[].controls`. Gallery examples demonstrate dropdowns, button selections, date fields, and validation metadata.
- `tests/form-controls.test.mjs` tests type inference, invalid configurations, DSL round-tripping, markup generation, and real template fixtures.

## Example

```text
form CustomerEditor for Customer {
  grid columns 12 {
    // @clarify-control {"field":"City","kind":"dropdown","choices":["Pasadena","Glendale","Other"]}
    City at (1,1) span (6,1)
  }
}
```

The `// @clarify-control` marker is a first-generation compatibility representation so existing ClarifyDSL parsing and positioning continue to work. A future structured DSL syntax and stable node IDs should replace comment metadata.

## Validation contract
- **Studio/design time:** Reject mismatched control/data types, missing or duplicate dropdown choices, and invalid min/max or maxLength configuration.
- **Generated browser:** Use Blazor input components, HTML limits where appropriate, and the existing EditForm/DataAnnotationsValidator/ValidationMessage.
- **Server:** Existing CustomerModel DataAnnotations and SqliteCustomerStore validation remain authoritative for required, email and field lengths. Arbitrary template-defined choice sets and numeric ranges are **not yet compiled into server validators**; they must not be treated as a security boundary.
- **Database:** Uniqueness, referential integrity and transactional validation are separate requirements.

## Not yet complete
Searchable relational lookups, server-generated choice enforcement, arbitrary generated model validators, rich NodaMoney/UnitsNet editors, true segmented button styling with full keyboard/a11y review, cross-field/async validators, and a full browser/.NET verification run. The renderer intentionally rejects rich numeric fields until strongly typed conversion bindings exist; generating an invalid `InputNumber<Money>` would be worse.

This is the first working source-level control architecture, not production-complete field validation.
