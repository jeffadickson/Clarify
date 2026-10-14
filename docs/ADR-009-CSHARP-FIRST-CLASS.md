# ADR-009 — C# is a first-class authoring language

**Status:** Accepted architectural requirement. Implementation maturity varies; this is not a claim that every API below already exists.

Clarify is a RAD environment, **not** a closed low-code platform. Visual dictionary, form/report designers and ClarifyDSL remove boilerplate, but authors can freely implement ordinary C# and consume .NET packages.

## Non-negotiable contracts
1. Every generated application is a normal, independently buildable .NET solution, not code trapped inside Clarify Studio.
2. Generated and user-authored files have distinct ownership; regeneration must never overwrite user C#.
3. Expose typed C# extension surfaces for domain services, commands, entity validation, lifecycle events, calculated fields, query customization, authorization, reports, Blazor components and Elsa activities.
4. Authors can replace or decorate services using dependency injection and standard .NET APIs; Clarify must not force every extension into a fixed set of DSL hooks.
5. Provide source navigation, normal VS Code/Visual Studio debugging, Roslyn diagnostics, type checking and NuGet dependencies.
6. A developer can write an entire application feature in C# and still benefit from generated dictionary/model code.
7. Version extension contracts and detect breakage at build time.
8. Scaffolding a new C# event handler, service or component should generate a commented class and xUnit test.
9. Generated orchestration must honor deterministic invocation order, validation and transaction boundaries; no arbitrary C# evaluation inside data-driven DSL text.
10. A no-code path and a code-first path are equally supported. The absence of custom C# should never be a prerequisite for using Clarify.

## Example (proposed API, not implemented)
```csharp
public sealed class ValidateOrder : IBeforeSave<Order>
{
    public Task ValidateAsync(Order order, CancellationToken ct)
    {
        if (order.Lines.Count == 0)
            throw new ValidationException("Order requires a line item.");
        return Task.CompletedTask;
    }
}
```

## Engineering acceptance
Generate an order-entry app; add user-owned C# validator, service override and Blazor component; regenerate forms and database mappings; confirm custom source is unchanged, compiles and executes. Verify with xUnit, NSubstitute ordered calls, Testcontainers persistence and a browser test.
