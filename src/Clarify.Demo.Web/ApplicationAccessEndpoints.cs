using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Clarify.Demo.Web;

/// <summary>
/// Example of the per-application authorization boundary generated endpoints will use.
/// Do not use a global "Manager" role as an alternative to this resource check.
/// </summary>
public static class ApplicationAccessEndpoints
{
    public static void MapApplicationAccess(this WebApplication app)
    {
        app.MapGet("/api/apps/{applicationId}/permissions/{permission}", async (
            string applicationId, string permission, ClaimsPrincipal user,
            IAuthorizationService authorization) =>
        {
            var result = await authorization.AuthorizeAsync(
                user,
                new ApplicationPermissionResource(applicationId, permission),
                new[] { new ApplicationPermissionRequirement() });
            return result.Succeeded ? Results.Ok(new { permitted = true })
                                    : Results.Forbid();
        }).RequireAuthorization();
    }
}
