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
        // Prefer the issuer-scoped OIDC subject even if middleware also mapped
        // a NameIdentifier claim. Different issuers can reuse the same sub.
        var sub = principal.FindFirst("sub");
        if (sub is not null && !string.IsNullOrWhiteSpace(sub.Value))
        {
            var issuer = principal.FindFirst("iss")?.Value ?? sub.Issuer;
            if (!string.IsNullOrWhiteSpace(issuer) && issuer != "LOCAL AUTHORITY")
                return $"oidc:{issuer}:{sub.Value}";
            return null;
        }
        var nameId = principal.FindFirst(ClaimTypes.NameIdentifier);
        return string.IsNullOrWhiteSpace(nameId?.Value) ? null : nameId.Value;
    }
}
