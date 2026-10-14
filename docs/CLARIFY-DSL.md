# ClarifyDSL — language design proposal v0.1

Status: proposed, not implemented. ClarifyDSL is the primary concise declarative authoring language for dictionary-aware forms, browses, reports and (later) workflow. Standard generated target is C#/ASP.NET Core. Loom remains optional.

## Goals
1. One-line default forms: `form CustomerEditor for Customer`.
2. Concise nested layout declarations, sensible dictionary-driven defaults.
3. Types, bindings and relationships resolved from Clarify semantic dictionary.
4. Parse > typed AST > validate > stable identity assignment > generate; no arbitrary execution in DSL.
5. Human-readable Git diffs and stable regeneration of visual customizations.
6. LSP diagnostics, autocomplete, go-to-definition and rename support.
7. Format/versioned syntax with error recovery and helpful source spans.
8. Safe extension components/commands registered through typed contracts.
9. Visual designer and DSL changes round trip through lossless CST where practicable.
10. Generation outputs independently deployable applications, without runtime requirement for Clarify Studio or Loom.

## Initial syntax
```clarify
form CustomerEditor for Customer {
  title "Customer Information"
  grid 2 {
    Name
    Email
    Phone
    State
  }
  group "Address" {
    Street
    City
    State
    Zip
  }
  actions { Save Cancel }
}

form OrderEditor for Order {
  row { Customer OrderDate Status }
  group "Line Items" {
    table Lines editable {
      Product
      Quantity
      UnitPrice
      LineTotal readonly
    }
  }
  row align right { Total readonly }
  actions { Save Cancel PrintInvoice }
}
```

## Grammar slice (EBNF draft)
```ebnf
program       = { declaration } ;
declaration   = formDecl | browseDecl | reportDecl ;
formDecl      = "form", identifier, "for", identifier, [ block ] ;
block         = "{", { formStatement }, "}" ;
formStatement = title | fieldDecl | groupDecl | gridDecl | rowDecl | tableDecl | actionsDecl ;
title         = "title", string ;
fieldDecl     = identifier, { "readonly" | "required" | "hidden" } ;
groupDecl     = "group", string, block ;
gridDecl      = "grid", integer, block ;
rowDecl       = "row", [ "align", ("left" | "center" | "right") ], block ;
tableDecl     = "table", identifier, [ "editable" ], block ;
actionsDecl   = "actions", "{", { identifier }, "}" ;
```

## Design constraints
- Dictionary decides data type, editor, relationship, permissions and validation. Layout is only an override.
- Parser rejects unknown symbols, non-detail table bindings, duplicate element IDs, inappropriate readonly overrides and unresolved commands with source-position diagnostics.
- Code generator emits typed bindings and explicit server-side authorization/validation; display rules never substitute for server enforcement.
- Stable IDs should be persisted in companion metadata or explicit `id` annotations for renames and visual design round trips.
- Use source spans and a lossless concrete syntax tree for comments/trivia; AST is normalized and renderer-agnostic.
- No `eval`, arbitrary SQL, untyped property paths, inline JavaScript or unconstrained C# in DSL.
- Separate capability validation for target renderers (Blazor first, Avalonia future).

## Initial acceptance cases
- One-line form creates all fields with correct dictionary-driven controls.
- Nested grid/group/table produces canonical typed AST.
- Typo in referenced field reports file, line, column and suggestions.
- Reference to non-collection in `table` is rejected.
- Regeneration preserves hand-authored command hooks and visual edits.
- Formatting yields idempotent output; parsing + printing preserves comments in supported edit workflow.
- xUnit parser tests + NSubstitute extension service tests + Testcontainers generated-application integration tests.

## Open decisions
1. Optional semicolons/newlines; current examples use whitespace-only statements. This causes ambiguity without grammar rules; establish newline significance or separators.
2. Expression syntax for conditional visibility; prefer a small typed grammar.
3. How to declare reusable components and slots.
4. Versioned schema of AST and serialization for IDE state.
5. Which lexical rules support paths, aliases, namespaced components, unicode, and escape sequences.
6. How to stage extensions without untrusted parser execution.
