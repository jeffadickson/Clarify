using Clarify.Dsl;

namespace Clarify.Demo.Web;

/// <summary>
/// Development-only comparison endpoint for the new native parser.
/// Parsing is read-only; it never evaluates C# or writes project files.
/// </summary>
public static class NativeDslEndpoints
{
    public sealed record ParseRequest(string? Source);
    public static IEndpointRouteBuilder MapNativeDslPreview(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/studio/dsl/parse-native", (HttpContext context, ParseRequest request) =>
        {
            if (context.Connection.RemoteIpAddress is not { } remote ||
                !System.Net.IPAddress.IsLoopback(remote))
                return Results.NotFound();
            if (request.Source is null || request.Source.Length > 256_000)
                return Results.BadRequest(new { error = "Source is missing or exceeds the 256 KB preview limit." });
            return Results.Ok(ClarifyFormParser.Parse(request.Source));
        });
        return endpoints;
    }
}
