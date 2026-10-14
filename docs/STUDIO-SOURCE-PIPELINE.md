# Clarify Studio source-to-Razor pipeline — M0 integration

**New:** Studio's embedded editor can load the source-controlled Customer form from `examples/forms/Customer.clarify` and save it through a guarded local-only API. This is a controlled development tool, not a remote authoring service.

## Developer walkthrough
1. Install .NET 10 SDK and Node 22; run `dotnet run --project src/Clarify.Demo.Web --urls http://127.0.0.1:5000` with `ASPNETCORE_ENVIRONMENT=Development`.
2. Open `http://127.0.0.1:5000/studio` in the same browser.
3. Select the Customer field, move it visually, or change its `at (...)` coordinates in the ClarifyDSL editor.
4. Click **Save Customer.clarify**. This uses an optimistic revision token (SHA-256 hash) to prevent silently overwriting another editor's update, and atomically replaces the project source.
5. Run `node tools/regenerate.mjs` in the repository root.
6. Rebuild/restart the .NET web host and open `/customers`; the generated layout is updated.

## Strict limitations
- This is a two-stage save/regenerate pipeline, **not hot reload**.
- Source writes are limited to the Customer form and a Development host on loopback, with same-origin Origin checking on mutations.
- The server's structural precheck does **not** replace the full ClarifyDSL parser; generation performs semantic validation.
- A failure during generation will not erase developer-owned source; the generator still writes directly to a generated Razor file and needs atomic staged output in a later step.
- Git source control is not automatically committed. Changes remain in the working tree for developer review.
- Workflow/Elsa support remains optional and independent of this RAD pipeline.
- Source changes are not guaranteed to be compiled until CI succeeds; do not imply verified builds.

## Acceptance gaps
1. Add production-grade parser/validator in .NET or controlled server-side compiler before saving.
2. Implement atomic generation and preview/diff/rollback in the Studio UI.
3. End-to-end browser test: drag field → save → regenerate → compile → visually verify.
4. Extend to Order/Product/OrderLine and report templates.
5. Audit authentication and remote usage before enabling any non-loopback source editing.
