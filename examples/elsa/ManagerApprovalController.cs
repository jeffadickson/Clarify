using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Stimuli;
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
public sealed class ManagerApprovalController(IWorkflowResumer resumer) : ControllerBase
{
    public sealed record DecisionRequest(string OrderCorrelation, bool Approved);

    [HttpPost("decide")]
    public async Task<IActionResult> Decide([FromBody] DecisionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderCorrelation))
            return BadRequest("OrderCorrelation is required.");

        // The bookmark payload MUST match the activity's payload precisely.
        // Including order correlation prevents cross-order resume collisions.
        var stimulus = new BookmarkStimulus
        {
            BookmarkName = "Clarify.ManagerApproval",
            Payload = new Dictionary<string, object>
            {
                ["OrderCorrelation"] = request.OrderCorrelation
            }
        };
        var actor = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(actor))
            return Forbid();

        // Elsa resumes the suspended activity, using bookmark persistence and
        // the configured runtime lock provider. This endpoint must additionally
        // enforce per-order authorization and audit the real actor.
        var results = await resumer.ResumeAsync(stimulus,
            new Dictionary<string, object>
            {
                ["Decision"] = request.Approved ? "Approved" : "Rejected",
                ["ApprovedBy"] = actor,
                ["ApprovedAt"] = DateTime.UtcNow
            });
        return results.Count == 0 ? NotFound() : Ok(new { Resumed = results.Count });
    }
}
