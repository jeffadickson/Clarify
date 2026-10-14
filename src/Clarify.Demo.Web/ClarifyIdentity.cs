using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clarify.Demo.Web;

/// <summary>Application-owned identities: workflow assignments reference stable user IDs, not names.</summary>
public sealed class ClarifyUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
public sealed class ClarifyIdentityDb(DbContextOptions<ClarifyIdentityDb> options)
    : IdentityDbContext<ClarifyUser>(options);

public static class ClarifyPolicies
{
    public const string ManageUsers = "Clarify.ManageUsers";
    public const string ApproveOrders = "Clarify.ApproveOrders";
    public const string DesignApplications = "Clarify.DesignApplications";
    public static IServiceCollection AddClarifyAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(ManageUsers, p => p.RequireAuthenticatedUser().RequireRole("Administrator"));
            options.AddPolicy(ApproveOrders, p => p.RequireAuthenticatedUser().RequireRole("Manager"));
            options.AddPolicy(DesignApplications, p => p.RequireAuthenticatedUser().RequireRole("Developer","Administrator"));
        });
        return services;
    }
}

/// <summary>
/// Per-record approval is checked separately from the Manager role.
/// Production implementations must also enforce tenant membership and separation of duties.
/// </summary>
public sealed record OrderApprovalResource(long OrderId, string RequesterId);
public sealed class OrderApprovalRequirement : IAuthorizationRequirement;
public sealed class OrderApprovalHandler : AuthorizationHandler<OrderApprovalRequirement,OrderApprovalResource>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, OrderApprovalRequirement requirement, OrderApprovalResource resource)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is not null &&
            userId != resource.RequesterId &&
            context.User.IsInRole("Manager"))
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
