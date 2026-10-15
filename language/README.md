# ClarifyDSL / Langium experimental parser

This **parallel** Langium grammar uses Node.js **only for development**. It does
not replace the current `designer/clarifydsl.mjs` parser, remove source edits,
or introduce Node.js into deployed .NET applications.

## Build

```sh
cd language
npm ci
npm test
```

The Langium CLI generates AST and language modules, then TypeScript builds
the parser and test runner. CI should compare the grammar against existing
`.clarify` files and template-generated layouts before migration.

The grammar currently covers basic forms, nested groups/grids, field positions,
simple actions, rows and stacks; it intentionally excludes dictionary editing,
full control declarations, tabs, reports and cross-file references.

**Not done:** in-browser worker bundle, Monaco language client, VS Code extension,
full lossless CST source patching and complete language parity. Those must pass
compatibility tests before changing Studio's default parser.
