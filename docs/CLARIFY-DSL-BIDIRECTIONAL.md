# ClarifyDSL bidirectional visual design — v0.2

Status: accepted design direction; implementation pending.

## Contract
The textual `.clarify` document is the developer-facing source of truth. The visual form editor parses it into a lossless concrete syntax tree (CST), resolves a typed design model against the data dictionary, and renders an interactive WYSIWYG form. Dragging, resizing, aligning, nesting or changing a property in the designer emits a **minimal syntax-aware edit to the original DSL**, retaining comments, unmodified whitespace, field order and unrelated custom declarations. Typing in the DSL updates the preview after successful incremental parsing. Invalid intermediate text does not destroy the last valid preview; diagnostics are shown inline.

## Positioning — separate intent from pixels
Favor constraints, containers and responsive layout over absolute coordinates. Offer absolute positions only for print-oriented or fixed-layout views. Positions can be edited numerically and by drag.

```clarify
form CustomerEditor for Customer {
  title "Customer"
  size min 640x420

  grid columns 12 gap 12 {
    Name  at (1,1) span (6,1)
    Email at (7,1) span (6,1)
    Phone at (1,2) span (4,1)
    State at (5,2) span (3,1)
    City  at (8,2) span (5,1)
  }

  breakpoint narrow max 720 {
    grid columns 1 {
      Name
      Email
      Phone
      State
      City
    }
  }

  actions { Save Cancel }
}

report Invoice for Order {
  page A4
  canvas units mm {
    text "INVOICE" at (15,18) size (65,10)
    Customer.CompanyName at (15,42) size (90,8)
    Total at (155,265) size (40,8)
  }
}
```

## Recommended layout constructs
- `stack`: vertical/horizontal flexible layout, gap and padding
- `grid`: 1-based row and column locations, spans, auto placement
- `row`: horizontal grouping with alignment and flexible sizing
- `group`, `tabs`, `split`: semantic containers
- `at (column,row) span (columns,rows)`: portable grid positioning for forms
- `width`, `height`, `min`, `max`, `align`, `margin`, `padding`: dimensional constraints
- `breakpoint`: named adaptive layout overrides with target-specific diagnostics
- `canvas units px|mm|pt`: fixed coordinates for reports, labels and special instrument interfaces; never default for responsive data-entry forms

## Designer interaction semantics
- Selecting a visual element selects the corresponding DSL source span (and reverse).
- Drag within grid updates `at` and `span`; dragging between containers changes parent block while preserving control ID and attached comments.
- Property edits change the smallest AST/CST-covered source region. Undo/redo groups mouse gestures and source edits into one transaction.
- Text editing with parse errors keeps the last compiled UI in preview and highlights errors; designer mutations are suspended or explicitly rebased.
- File watching supports external VS Code edits; conflicts handled by revision IDs and three-way merge, not last-writer-wins.
- IDs are stable across renames; explicit `id` where needed, or companion identity map, to retain properties and avoid accidental recreation.
- Responsive breakpoints and accessibility constraints have preview modes; designer warns about overlapping fixed-layout controls and unreachable keyboard traversal.
- Atomic writes, source control diffs and deterministic formatting are required.

## Architectural split
`ClarifyDSL parser/CST` -> `typed UI AST + symbols` -> `designer model` and `generator target(s)`. Source edits travel through a patch service that validates revision, determines minimal CST edits, reparses, and publishes updated source + semantic model in one atomic transaction. No reverse HTML/DOM scraping.

## Required tests
- xUnit parse/edit/print: editing position changes only intended statement; preserve comments, formatting and IDs.
- Property-based roundtrip: parse -> visual action -> text update -> parse yields equivalent structure.
- NSubstitute: verify ordered edit transaction callbacks and rejection of stale revisions.
- Playwright: drag and resize form field, assert precise DSL position update; edit DSL and assert UI moves.
- Testcontainers: generated application stores real submitted form data in SQLite/PostgreSQL/SQL Server (as appropriate); UI layout edits do not alter persistence schema.

## Open choices
Define precise units for form constraints and target-independent adaptive behaviors. Avoid arbitrary coordinates where forms must remain responsive.
