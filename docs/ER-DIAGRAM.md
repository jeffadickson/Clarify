# Entity-relationship diagram in Clarify Studio

Clarify Studio now has an **additive** ER diagram at `/studio/er-diagram.html`.
The existing `/studio/schema` graphical data dictionary and `/studio` form
designer remain unchanged in purpose and remain accessible.

The diagram displays:
- Every dictionary entity as a table card, with field names, primary key flags, types and database location.
- Native same-database foreign keys as solid lines.
- Explicit application-managed cross-database references as dashed lines.
- Live dependency warnings from `designer/dependencies.mjs` (mirrored to the web static assets).
- A drag-to-rearrange view and SVG export.

It uses the canonical served dictionary or the dictionary designer's
existing `clarify-schema-prototype` local browser draft. Edits are performed
in the dictionary designer; the diagram is a **read-only projection** and
cannot modify schema or execute migrations. Reload picks up the latest draft.

This is a self-contained SVG renderer with no CDN, third-party graph server,
or Graphviz installation. It works in the existing ASP.NET Core-hosted Studio.

## Limitations

Automatic positioning is grid-based rather than optimal graph layout.
Edge routing may cross crowded tables; large schemas need zoom/pan, automatic
layout, field grouping and improved accessibility. Dragged coordinates are
currently view-local and aren't stored as project layout metadata.

The parser compatibility suite still needs a *verified passing* .NET run.
No parser replacement was made to implement this diagram.

Source integration smoke is in `tests/studio-integration.test.mjs`;
browser interaction and visual regression tests remain acceptance gates.
