using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;

namespace Clarify.Examples.Elsa;

/// <summary>
/// A genuine Elsa blocking activity. Unlike a polling loop, a bookmark releases
/// the executing thread and lets Elsa persist the workflow's suspended state.
/// </summary>
[Activity("Clarify", "Approvals", "Waits for an authenticated manager decision")]
public sealed class WaitForManagerApproval : Activity
{
    // The caller chooses the business correlation value, such as "order:42".
    // It MUST be stable across retries; never use a new random value on replay.
    public Input<string> OrderCorrelation { get; set; } = default!;

    protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        var order = context.Get(OrderCorrelation);
        if (string.IsNullOrWhiteSpace(order))
            throw new InvalidOperationException("A nonempty order correlation is required.");

        // Bookmarks are durable only when the hosting Elsa runtime is configured
        // with persistent instance/bookmark stores (e.g. EF Core SQLite).
        context.CreateBookmark(new CreateBookmarkArgs
        {
            BookmarkName = "Clarify.ManagerApproval",
            Stimulus = new Dictionary<string, object> { ["OrderCorrelation"] = order },
            IncludeActivityInstanceId = false,
            Callback = OnResumeAsync,
            AutoBurn = true // A consumed approval cannot be resumed a second time.
        });
        return ValueTask.CompletedTask;
    }

    private static async ValueTask OnResumeAsync(ActivityExecutionContext context)
    {
        // These values must be provided by an authenticated/authorized endpoint.
        // Never trust an ApprovedBy username supplied in the HTTP request body.
        var input = context.WorkflowInput;
        var decision = input.TryGetValue("Decision", out var value)
            ? value?.ToString() : null;
        var outcome = decision switch
        {
            "Approved" => "Approved",
            "Rejected" => "Rejected",
            _ => throw new InvalidOperationException("Expected Approved or Rejected.")
        };
        await context.CompleteActivityWithOutcomesAsync(outcome);
    }
}
