using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Clarify.Demo.Web;

/// <summary>
/// Authorization belongs to a generated application, not just to the host user.
/// Role names are only meaningful within one application.
/// </summary>
public sealed class ClarifyApplication
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}
public sealed class ClarifyApplicationRole
{
    public string ApplicationId { get; set; } = "";
    public string Name { get; set; } = "";
}
public sealed class ClarifyRolePermission
{
    public string ApplicationId { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string Permission { get; set; } = "";
}
public sealed class ClarifyApplicationMembership
{
    public string ApplicationId { get; set; } = "";
    public string UserId { get; set; } = "";
    public bool Enabled { get; set; } = true;
}
public sealed class ClarifyMembershipRole
{
    public string ApplicationId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string RoleName { get; set; } = "";
}
public sealed record ApplicationPermissionResource(
    string ApplicationId, string Permission, string? RecordOwnerId = null);
public sealed class ApplicationPermissionRequirement : IAuthorizationRequirement { }

/// <summary>
/// Deny by default: users need an enabled membership, an assigned application role,
/// and an explicit permission grant. Optional owner restriction applies after that.
/// </summary>
public sealed class ApplicationPermissionHandler(ClarifyIdentityDb db)
    : AuthorizationHandler<ApplicationPermissionRequirement, ApplicationPermissionResource>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApplicationPermissionRequirement requirement,
        ApplicationPermissionResource resource)
    {
        var userId=context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || string.IsNullOrWhiteSpace(resource.ApplicationId) ||
            string.IsNullOrWhiteSpace(resource.Permission)) return;

        if (resource.RecordOwnerId is not null && resource.RecordOwnerId != userId)
            return; // Ownership constraints never bypass app membership/role checks.

        var allowed=await (
            from membership in db.ApplicationMemberships.AsNoTracking()
            join assigned in db.MembershipRoles.AsNoTracking()
                on new {membership.ApplicationId,membership.UserId}
                equals new {assigned.ApplicationId,assigned.UserId}
            join permission in db.RolePermissions.AsNoTracking()
                on new {assigned.ApplicationId,RoleName=assigned.RoleName}
                equals new {permission.ApplicationId,permission.RoleName}
            where membership.UserId==userId && membership.Enabled &&
                  membership.ApplicationId==resource.ApplicationId &&
                  permission.Permission==resource.Permission
            select membership.UserId).AnyAsync();

        if (allowed) context.Succeed(requirement);
    }
}
