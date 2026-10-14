# ClarifyDSL implementation checkpoint

GitHub contains `designer/clarifydsl.mjs` (nested group/grid parser and source-preserving position edits), `designer/index.html` (browser designer), `designer/clarifydsl.test.mjs` (four Node tests), and `tools/generate-blazor.mjs` (dictionary + ClarifyDSL → generated .razor forms).

## Implemented subset
- Dictionary-driven default forms for available entities.
- Nested group/grid source syntax in the parser (not complete arbitrary XAML-style nesting).
- Visual ↔ DSL coordinate editing, responsive visual canvas.
- Single-line source replacement preserves comments and unrelated lines.
- Generated Razor form markup that infers input types from schema.
- GitHub Actions job runs Node tests, generator and .NET build.

## Not yet complete
- Full lossless CST: parser currently preserves source text and line identities, but has no general token/trivia tree or arbitrary subtree edits.
- Generated Blazor forms have no EF Core persistence binding; HTML post URLs are placeholders.
- Nested group visual grouping not rendered distinctly.
- No runtime-generated typed EditForm, validation summaries or command registration.
- Drag between containers, resizing handles, undo/redo, incremental parse recovery, portable AST schema, multi-user revisions.
- The .NET compilation and browser tests have not been verified in the current execution environment.

Do not close GitHub M0 issue until compilation and end-to-end persistence tests pass.
