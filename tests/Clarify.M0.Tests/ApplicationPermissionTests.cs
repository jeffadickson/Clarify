using System.Security.Claims;
using Clarify.Demo.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Clarify.M0.Tests;

public sealed class ApplicationPermissionTests
{
    [Fact]
    public async Task SameUserCanApproveInOneAppButNotAnother()
    {
        var options=new DbContextOptionsBuilder<ClarifyIdentityDb>()
            .UseSqlite("Data Source=:memory:").Options;
        await using var db=new ClarifyIdentityDb(options);
        await db.Database.OpenConnectionAsync();
        await db.Database.EnsureCreatedAsync();
        db.Applications.AddRange(
            new ClarifyApplication { Id="sales",Name="Sales" },
            new ClarifyApplication { Id="inventory",Name="Inventory" });
        db.ApplicationRoles.Add(new ClarifyApplicationRole {ApplicationId="sales",Name="Approver"});
        db.ApplicationMemberships.Add(new ClarifyApplicationMembership {ApplicationId="sales",UserId="user-a"});
        db.MembershipRoles.Add(new ClarifyMembershipRole {ApplicationId="sales",UserId="user-a",RoleName="Approver"});
        db.RolePermissions.Add(new ClarifyRolePermission {ApplicationId="sales",RoleName="Approver",Permission="orders.approve"});
        // SQLite FK rules require a real Identity user for the membership relation only
        // if it is explicitly configured; the host model currently keys membership independently.
        await db.SaveChangesAsync();

        var handler=new ApplicationPermissionHandler(db);
        var principal=new ClaimsPrincipal(new ClaimsIdentity(
            new[]{new Claim(ClaimTypes.NameIdentifier,"user-a")},"Test"));
        async Task<bool> Allowed(string appId)
        {
            var requirement=new ApplicationPermissionRequirement();
            var context=new AuthorizationHandlerContext(
                new[]{requirement},principal,
                new ApplicationPermissionResource(appId,"orders.approve"));
            await handler.HandleAsync(context);
            return context.HasSucceeded;
        }
        Assert.True(await Allowed("sales"));
        Assert.False(await Allowed("inventory"));
    }
}
