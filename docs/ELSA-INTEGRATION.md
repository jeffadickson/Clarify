# Elsa 3.8 integration bootstrap

Elsa 3.8 is now the preferred runtime. Clarify provisions the **official Elsa Server + Studio combined template** rather than extending the prototype `WorkflowRunner` into a general-purpose orchestrator. Elsa documentation specifies that templates 3.8.0 generate .NET 10 apps with matching package versions and SQLite-backed workflow persistence.

## Fresh clone
Prerequisites: .NET 10 SDK and network access to NuGet.

macOS/Linux:
```sh
bash scripts/bootstrap-elsa.sh
```
Windows PowerShell:
```powershell
./scripts/bootstrap-elsa.ps1
```

This creates `artifacts/ClarifyElsaHost` (ignored by Git), restores and builds it. The script refuses to overwrite existing output. Configure the generated app's **Development-only** credentials and signing key following Elsa's current instructions. Never commit secrets or expose the default development identity to a network. Run the host with `dotnet run --project artifacts/ClarifyElsaHost` and use its built-in Elsa Studio.

## ClarifyDSL integration
The workflow definition `examples/workflows/OrderApproval.clarify` is parsed by `workflow/workflowdsl.mjs`, compiled into a sequential plan by `workflow/compile.mjs`, and checked for Elsa compatibility by `workflow/elsa-manifest.mjs`.

The manifest ensures stable activity IDs, resolved registered C# activities, approval roles and versions. **It does not yet install/launch those definitions in Elsa.** That requires the .NET Elsa adapter which constructs actual Elsa activity objects and registers them with the host.

`node --test workflow/*.test.mjs` runs the DSL and manifest tests.

## Current verification and limits
- Source bootstrapping is committed, but there is no passing .NET template-build result available from this execution environment.
- The old SQLite runner remains a test fixture; Elsa has not been wired into the Clarify web process or native desktop UI.
- The Elsa workflow designer is available in the generated combined host, but bidirectional editing with ClarifyDSL is **not** complete.
- No secure manager approval activity, authorization checks or outbox have been implemented.
- Do not enable Elsa APIs in Clarify's unauthenticated development demo endpoints.

## Next release gates
1. Successfully generate, compile and boot the official Elsa combined host.
2. Implement and test `Clarify.Workflow.Elsa` to register actual Elsa workflows and typed activities from manifests.
3. Prove SQLite restart/bookmark recovery with a real Elsa workflow instance.
4. Authenticate manager decisions and enforce role checks server-side.
5. Verify concurrency, idempotency, outbound effects and PostgreSQL/SQL Server persistence under Testcontainers.
6. Embed or link Elsa Studio inside Clarify Studio with careful permissions and supported source mappings.

References:
- https://www.elsaworkflows.io/get-started
- https://www.elsaworkflows.io/get-started/elsa-server
- https://www.elsaworkflows.io/get-started/elsa-server-and-studio
