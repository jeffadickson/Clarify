using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;

namespace Clarify.Demo.Web;

/// <summary>
/// Host-selectable authentication. Application roles and permissions are deliberately
/// independent of which provider authenticated a person.
/// </summary>
public static class ClarifyAuthentication
{
    public static IServiceCollection AddClarifyAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Clarify:Authentication:Provider"] ?? "Identity";
        if (provider.Equals("Identity", StringComparison.OrdinalIgnoreCase))
        {
            services.AddIdentityApiEndpoints<ClarifyUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ClarifyIdentityDb>();
            return services;
        }

        if (!provider.Equals("Oidc", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Choose Clarify:Authentication:Provider = Identity or Oidc.");

        var authority = configuration["Clarify:Authentication:Authority"];
        var clientId = configuration["Clarify:Authentication:ClientId"];
        if (!Uri.TryCreate(authority, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps || string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("OIDC requires an HTTPS authority and a client ID.");

        // Never hard-code the client secret. Set it through user-secrets,
        // environment variables, or the deployment secret manager.
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
        })
        .AddOpenIdConnect(options =>
        {
            options.Authority = authority;
            options.ClientId = clientId;
            options.ClientSecret = configuration["Clarify:Authentication:ClientSecret"];
            options.ResponseType = "code";
            options.UsePkce = true;
            options.SaveTokens = false;
            options.MapInboundClaims = false;
            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "roles";
        });
        return services;
    }
}
