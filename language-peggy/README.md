# Lightweight ClarifyDSL parser experiment (Peggy)

Peggy is a build-time development dependency; its pre-generated ES module
runs in browser and Node with **no Node.js server**.

```sh
cd language-peggy
npm install
npm test
```

This grammar is a **small proof of parsing forms, groups, grids and fields**,
not a replacement for the active ClarifyDSL parser. Comments and formatting
must be retained in original source and patched by source span, not recreated
from the AST. Full feature parity with 40 templates, control markers, actions,
layout constructs and schema DSL must be demonstrated before switching Studio.

See docs/ADR-010-LIGHTWEIGHT-DSL-PARSER.md.
