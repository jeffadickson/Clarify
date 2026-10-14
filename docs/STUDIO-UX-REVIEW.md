# Clarify Studio — integrated RAD experience and Better Design review

Status: first Blazor-hosted workspace is committed; full end-to-end UX not browser-verified.

## Why this change
Clarify originally had separate HTML prototypes and a generated Blazor app. That split undermines the core promise: developers should stay in one product while moving between schema, visual editor, DSL and running application. The new `/studio` route embeds the two-way form designer, and `/studio/schema` displays the actual project dictionary. Runtime screens are linked directly.

## Better Design guidance consulted
Reviewed Better Design's **Forms**, **Navigation**, and **Accessibility code review** guidance. Decisions:
- Persistent project tool navigation, active destination and a direct **Open running form** action.
- Prioritize the visual form workspace; don't default users into a giant configuration dialog.
- Preserve keyboard-operable position fields alongside drag interactions.
- Keep all fields labeled, show syntax errors visibly and preserve the last valid preview.
- Designer changes must not be presented as deployed changes before safe regeneration exists.
- Responsive workspace and source editor should remain readable on narrow screens.

## UX acceptance requirements
1. First-run project path: import/create schema, see a generated form immediately, change layout, save DSL and preview.
2. Selecting a canvas field selects its DSL source and property inspector; edits are reversible and keyboard accessible.
3. Source ↔ visual synchronization preserves comments, stable IDs and manual customizations.
4. Developer always knows which artifact is edited, saved, generated and deployed. Dirty/unsaved status must be explicit.
5. Schema edits do not silently mutate live application data; migration preview and approval are separate.
6. Run mode is a full data-backed app, not static mock output.
7. Accessibility: meaningful heading hierarchy, labels, focus rings, semantic landmarks and visible errors.
8. Validate desktop/mobile layouts in actual browser captures at 390, 1440 and 1728 pixels.

## Present limitations
- The `/studio` designer remains an embedded static JS canvas running in a Blazor-hosted app, not yet a fully native Razor visual designer.
- Its save persistence is currently browser localStorage, not Git repository or project-source patching. It does **not** write `.clarify` back to disk or regenerate deployed pages.
- Dictionary page is a read-only view.
- There is no integrated build, regenerate, schema migration or full programmatic UI test.
- Browser visual review and .NET compilation are not verified in this session.

## Next engineering slice
Add an explicit secure local-only project edit API that writes user-owned `.clarify` files atomically with revision IDs, plus a project regeneration command with preview/diff and build status. Then complete the end-to-end order-entry generation pipeline and automated Playwright tests.
