using System.Security.Claims;

namespace Clarify.Demo.Web;

/// <summary>
/// Never use email or display name as a permission key. An external subject is
/// unique only within its issuer; prepend its issuer to prevent collisions.
/// Built-in ASP.NET Identity continues to use its stable NameIdentifier claim.
/// </summary>
public static class ClarifyPrincipalIds
{
    public static string? GetStableId(ClaimsPrincipal principal)
    {
        var nameId = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (nameId is not null && !string.IsNullOrWhiteSpace(nameId.Value))
            return nameId.Value;

        var sub = principal.FindFirst("sub");
        if (sub is null || string.IsNullOrWhiteSpace(sub.Value)) return null;
        var issuer = principal.FindFirst("iss")?.Value ?? sub.Issuer;
        if (string.IsNullOrWhiteSpace(issuer) || issuer == "LOCAL AUTHORITY")
            return null;
        return $"oidc:{issuer}:{sub.Value}";
    }
}
