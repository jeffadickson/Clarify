# ClarifyDSL Blazor rendering target (early prototype)

From repository root:
```bash
node tools/generate-blazor.mjs
dotnet run --project blazor/Clarify.Forms
```
The generator parses dictionary-derived ClarifyDSL, validates fields, and emits Razor components for each entity.

**Limitations:** This is the first visual rendering target, not yet integrated with the existing generated ASP.NET Core + EF Core APIs. Forms emit HTTP POST to placeholder routes and do not persist data. The generated pages require real `/api/<entity>` endpoints to accept POST and enforce validation. No verified .NET build has been performed here; `dotnet` was unavailable. An actual browser/EF integration test and a native Blazor EditForm binding implementation are follow-up tasks.

Clarify does not depend on Loom. Loom can consume exported form AST and a standalone Blazor component library through a versioned NuGet package once the AST and bindings stabilize.
