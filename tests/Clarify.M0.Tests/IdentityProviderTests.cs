using System.Security.Claims;
using Clarify.Demo.Web;
using Xunit;

namespace Clarify.M0.Tests;

/// <summary>External identities must never be keyed by changeable email addresses.</summary>
public sealed class IdentityProviderTests
{
    [Fact]
    public void ExternalSubjectIncludesIssuer()
    {
        var claims=new ClaimsPrincipal(new ClaimsIdentity(new[]{
            new Claim("sub","user-42"),
            new Claim("iss","https://login.example.invalid")
        },"oidc"));
        Assert.Equal("oidc:https://login.example.invalid:user-42",
            ClarifyPrincipalIds.GetStableId(claims));
    }

    [Fact]
    public void BuiltInIdentityRetainsExistingUserId()
    {
        var user=new ClaimsPrincipal(new ClaimsIdentity(new[]{
            new Claim(ClaimTypes.NameIdentifier,"aspnet-user-a")
        },"identity"));
        Assert.Equal("aspnet-user-a",ClarifyPrincipalIds.GetStableId(user));
    }

    [Fact]
    public void MissingIdentityIsDenied()
    {
        Assert.Null(ClarifyPrincipalIds.GetStableId(new ClaimsPrincipal()));
    }
}
