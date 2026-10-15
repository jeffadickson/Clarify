# C# API Documentation Standard

Clarify adopts **C# XML documentation comments**, the .NET equivalent of JavaDoc:
`/// <summary>`, `<param>`, `<typeparam>`, `<returns>`, `<exception>`,
`<remarks>`, and `<example>`. **DocFX** renders the compiled assemblies and their XML documentation into a searchable API reference.

## Nonnegotiable authoring contract

- Every new public/protected class, interface, record, enum, method, constructor,
  property, event, and extension API is documented at the point of declaration.
- Public methods describe parameters, return values, cancellation behavior,
  side effects, security preconditions, important exceptions, and ownership.
- Private methods and complex internal logic get meaningful explanatory comments;
  comments explain intent and invariants, not restate a line of code.
- Generated C# / generated Razor members are documented by their **generators**.
  Never edit `.g.cs` or generated Razor output as a durable documentation fix.
- Source templates generating C# must produce the same documentation comments
  and remain safe across regeneration, without overwriting author-owned C#.
- Examples should be executable where practical; XML markup must remain valid.
- The standard also applies to generated and author-owned C# extension scaffolds,
  instrument drivers, test stubs, EF Core models, command handlers, and workflows.
- Avoid secrets or customer data in examples or documentation comments.

## Build and publication

`Directory.Build.props` sets `GenerateDocumentationFile=true`. Missing public
documentation raises compiler **CS1591**. The existing codebase has legacy gaps,
so missing-comment warnings are currently measured, **not globally treated as
errors**. We must not claim full coverage until the warnings and audit are closed.

Run:

```sh
node tools/audit-csharp-docs.mjs --report --strict-generated
dotnet restore src/Clarify.Demo.Web/Clarify.Demo.Web.csproj
dotnet build src/Clarify.Demo.Web/Clarify.Demo.Web.csproj -c Release
dotnet tool restore
dotnet docfx docs/docfx.json
```

The static site is produced at `docs/_site`. GitHub Actions uploads it as
a build artifact. The audit report is `artifacts/documentation-coverage.json`
and is a **heuristic backlog inventory**, not a Roslyn-semantic coverage number.
Compiler XML docs/CS1591 are the authoritative test for public C# declarations.

## Example

```csharp
/// <summary>Retrieves a bounded page of customers in stable order.</summary>
/// <param name="page">Zero-based page number.</param>
/// <param name="pageSize">Maximum records returned, subject to service limits.</param>
/// <param name="ct">Cancellation token for database I/O.</param>
/// <returns>Page records and whether a subsequent page exists.</returns>
/// <exception cref="ArgumentOutOfRangeException">
/// The page index or size exceeds the supported bounds.
/// </exception>
public Task<CustomerPageResult> PageAsync(
    int page, int pageSize, CancellationToken ct = default);
```

## Documentation debt and release gates

1. Audit *all* public C# APIs and record missing documentation in issue #40.
2. Fix handwritten source, generated code, and C# templates at the generator.
3. Verify DocFX site generation and XML links through CI.
4. Gradually enable CS1591-as-error for projects with complete coverage;
   only then enforce repository-wide as an error, with reviewed exceptions.
5. Include code examples, XML reference, Studio walkthrough and upgrade/migration
   documentation in release packaging.

Avoid automatic meaningless boilerplate comments merely to silence CS1591.
Docs should explain behavior; the code remains the source of truth.
