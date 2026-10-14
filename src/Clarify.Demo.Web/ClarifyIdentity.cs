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
    : IdentityDbContext<ClarifyUser>(options)
{
    public DbSet<ClarifyApplication> Applications => Set<ClarifyApplication>();
    public DbSet<ClarifyApplicationMembership> ApplicationMemberships => Set<ClarifyApplicationMembership>();
    public DbSet<ClarifyApplicationRole> ApplicationRoles => Set<ClarifyApplicationRole>();
    public DbSet<ClarifyMembershipRole> MembershipRoles => Set<ClarifyMembershipRole>();
    public DbSet<ClarifyRolePermission> RolePermissions => Set<ClarifyRolePermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ClarifyApplication>().HasKey(x=>x.Id);
        builder.Entity<ClarifyApplicationRole>().HasKey(x=>new {x.ApplicationId,x.Name});
        builder.Entity<ClarifyApplicationMembership>().HasKey(x=>new {x.ApplicationId,x.UserId});
        builder.Entity<ClarifyMembershipRole>().HasKey(x=>new {x.ApplicationId,x.UserId,x.RoleName});
        builder.Entity<ClarifyRolePermission>().HasKey(x=>new {x.ApplicationId,x.RoleName,x.Permission});
        builder.Entity<ClarifyApplicationMembership>()
            .HasOne<ClarifyApplication>().WithMany().HasForeignKey(x=>x.ApplicationId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ClarifyApplicationRole>()
            .HasOne<ClarifyApplication>().WithMany().HasForeignKey(x=>x.ApplicationId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ClarifyMembershipRole>()
            .HasOne<ClarifyApplicationMembership>().WithMany()
            .HasForeignKey(x=>new {x.ApplicationId,x.UserId}).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ClarifyMembershipRole>()
            .HasOne<ClarifyApplicationRole>().WithMany()
            .HasForeignKey(x=>new {x.ApplicationId,Name=x.RoleName}).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ClarifyRolePermission>()
            .HasOne<ClarifyApplicationRole>().WithMany()
            .HasForeignKey(x=>new {x.ApplicationId,Name=x.RoleName}).OnDelete(DeleteBehavior.Cascade);
    }
}

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
public sealed class OrderApprovalRequirement : IAuthorizationRequirement { }
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
