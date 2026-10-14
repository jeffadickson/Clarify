namespace Clarify.Demo.Web;

/// <summary>
/// Authentication is an application-level design decision, not a platform-wide
/// assumption. "Public" never permits anonymous administration or approvals.
/// </summary>
public enum ApplicationAccessMode { Public, OptionalLogin, LoginRequired }

public sealed record ApplicationSecurityProfile(
    string ApplicationId,
    ApplicationAccessMode Mode,
    bool AllowAnonymousReads = false);

public static class ApplicationSecurityProfiles
{
    public static ApplicationSecurityProfile Resolve(IConfiguration configuration, string applicationId)
    {
        var raw = configuration[$"Clarify:Applications:{applicationId}:AccessMode"] ?? "LoginRequired";
        if (!Enum.TryParse<ApplicationAccessMode>(raw, true, out var mode))
            throw new InvalidOperationException($"Invalid access mode for application '{applicationId}': {raw}");
        return new(applicationId, mode, mode == ApplicationAccessMode.Public);
    }

    /// <summary>
    /// Public: ordinary published pages can be visited anonymously.
    /// OptionalLogin: viewing is public, personal features require login.
    /// LoginRequired: enforce authenticated access by default.
    /// Every admin, mutation and workflow approval requires an explicit grant.
    /// </summary>
    public static bool AllowsAnonymousView(ApplicationAccessMode mode) =>
        mode is ApplicationAccessMode.Public or ApplicationAccessMode.OptionalLogin;
}
