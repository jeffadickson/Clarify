namespace Clarify.Extensibility;

/// <summary>User-owned command handlers are registered by dependency injection.</summary>
public interface ICommandHandler<in TCommand> where TCommand : notnull {
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
}
/// <summary>Typed, asynchronous business validation extension.</summary>
public interface IModelValidator<in TModel> {
    Task<IReadOnlyList<string>> ValidateAsync(TModel value, CancellationToken cancellationToken);
}
/// <summary>Allows a workflow runtime provider to be replaced without modifying application command handlers.</summary>
public interface IWorkflowGateway {
    Task<string> StartAsync(string definitionId, int version, object input, string correlationKey, CancellationToken cancellationToken);
    Task SignalAsync(string instanceId, string eventName, object eventData, string idempotencyKey, CancellationToken cancellationToken);
    Task<WorkflowSnapshot?> GetAsync(string instanceId, CancellationToken cancellationToken);
}
public sealed record WorkflowSnapshot(string InstanceId, string DefinitionId, int Version, string Status, string? WaitingFor);
/// <summary>Step implementations must tolerate repeated invocation after crashes.</summary>
public interface IWorkflowStep<in TInput> {
    Task ExecuteAsync(TInput input, string idempotencyKey, CancellationToken cancellationToken);
}
