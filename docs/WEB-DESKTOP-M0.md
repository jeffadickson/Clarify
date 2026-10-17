# ClarifyDSL: shared Web + Desktop vertical slice

The same `CustomerPage` and `CustomerFields` Razor components are hosted by:
- `src/Clarify.Demo.Web`: ASP.NET Core Blazor interactive server app.
- `src/Clarify.Demo.Desktop`: native .NET MAUI Blazor Hybrid app (Windows and macOS desktop; Android/iOS additional targets).
- `src/Clarify.Forms.Shared`: shared Razor class library, data contracts and generated editor.
- `src/Clarify.Demo.Data`: EF Core SQLite store shared by both hosts.

## Run web
```sh
node tools/generate-shared-blazor.mjs
dotnet run --project src/Clarify.Demo.Web
```
Navigate to the local web URL and save a customer. Restart and confirm the record persists in `src/Clarify.Demo.Web/clarify-customers.sqlite` or the host's content-root database.

## Run desktop
Requires .NET MAUI workload and a supported platform SDK. Windows: Visual Studio with MAUI workload. Mac: supported macOS/Xcode with MAUI Mac Catalyst.
```sh
dotnet workload install maui
dotnet build src/Clarify.Demo.Desktop -f net10.0-windows10.0.19041.0
```
On macOS use `-f net10.0-maccatalyst`. Run in IDE or platform tooling; workload/signing/prerequisites vary. Linux desktop is **not** supported by this MAUI scaffold.

Desktop app uses its own SQLite file under `FileSystem.AppDataDirectory`. The web host uses a different SQLite file. **Shared UI and data-model behavior do not imply shared database contents or automatic sync.**

## DSL edit test
Change `Email at (7,1) span (6,1)` to `Email at (1,3) span (6,1)` in `examples/forms/Customer.clarify`. Run `node tools/generate-shared-blazor.mjs`; rebuild either host. Both hosts receive the updated layout.

## Current engineering limits
- Demonstration is Customer CRUD (create/update + browse), not the entire order-entry application.
- Demo uses `EnsureCreated` and must adopt migrations before production upgrades.
- Auth/authorization, concurrency policy, tenant isolation, and offline sync are not implemented.
- The existing designer is a subset parser and supports grid coordinates and nested blocks, not a general-purpose lossless CST.
- MAUI app IDs and packaging must be finalized before releases.
- Neither .NET build nor native packaging has yet been tested in the current environment (no dotnet SDK).
- Existing tests for the browser DSL run under Node; xUnit + NSubstitute + Testcontainers still need to verify this new vertical slice.

## Loom applicability
Loom can consume the same shared renderer as an optional library, but should supply `ICustomerStore`-like adapters connected to Loom's domain services, rather than using Clarify's EF layer. No Loom repository changes are made.
