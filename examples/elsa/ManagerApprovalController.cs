using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clarify.Examples.Elsa;

/// <summary>
/// Intended for a properly authenticated Elsa ASP.NET Core host.
/// DO NOT mount this controller on Clarify's anonymous development endpoints.
/// </summary>
[ApiController]
[Route("api/clarify/approvals")]
[Authorize(Policy = "ClarifyManager")] // Host must configure this policy and real identity.
public sealed class ManagerApprovalController(IWorkflowDispatcher dispatcher) : ControllerBase
{
    public sealed record DecisionRequest(string OrderCorrelation, bool Approved);

    [HttpPost("decide")]
    public async Task<IActionResult> Decide([FromBody] DecisionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderCorrelation))
            return BadRequest("OrderCorrelation is required.");

        // The bookmark payload MUST match the activity's payload precisely.
        // Including order correlation prevents cross-order resume collisions.
        var actor = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(actor))
            return Forbid();

        // Elsa resumes the suspended activity, using bookmark persistence and
        // the configured runtime lock provider. This endpoint must additionally
        // enforce per-order authorization and audit the real actor.
        // Elsa 3.8 dispatches by activity/bookmark type and matching stimulus.
        // A queued dispatch is not proof that the workflow already resumed.
        await dispatcher.DispatchAsync(new DispatchResumeWorkflowsRequest(
            activityTypeName: "Clarify.ManagerApproval",
            bookmarkPayload: new Dictionary<string, object>
            {
                ["OrderCorrelation"] = request.OrderCorrelation
            })
        {
            Input = new Dictionary<string, object>
            {
                ["Decision"] = request.Approved ? "Approved" : "Rejected",
                ["ApprovedBy"] = actor,
                ["ApprovedAt"] = DateTime.UtcNow
            }
        });
        return Accepted(new { Queued = true });
    }
}
