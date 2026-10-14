using System.Security.Cryptography;
using System.Text;

namespace Clarify.Demo.Web;

/// <summary>
/// Local development-only source editor. Never expose it on a public host.
/// Uses revision hashes to prevent two editor tabs silently overwriting work.
/// </summary>
public static class StudioSourceEndpoints
{
    public sealed record SaveForm(string Source, string Revision);
    public static void MapStudioSource(this WebApplication app)
    {
        var repo = FindProjectRoot(app.Environment.ContentRootPath);
        var dictionary = Path.Combine(repo, "examples", "order-entry", "dictionary.json");
        var forms = Path.Combine(repo, "examples", "forms");
        // Only known entity names can address a project file.
        app.MapGet("/api/studio/forms/{entity}", (string entity) =>
        {
            if (!Allowed(entity, dictionary)) return Results.NotFound();
            var path = Path.Combine(forms, entity + ".clarify");
            if (!File.Exists(path)) return Results.NotFound();
            var source = File.ReadAllText(path);
            return Results.Ok(new { source, revision = Hash(source) });
        });
        app.MapPut("/api/studio/forms/{entity}", async (string entity, SaveForm request) =>
        {
            if (!Allowed(entity, dictionary)) return Results.NotFound();
            if (request.Source is null || request.Source.Length > 200_000) return Results.BadRequest("Invalid source length.");
            var path = Path.Combine(forms, entity + ".clarify");
            if (!File.Exists(path)) return Results.NotFound();
            var current = await File.ReadAllTextAsync(path);
            if (!string.Equals(Hash(current), request.Revision, StringComparison.OrdinalIgnoreCase))
                return Results.Conflict(new { error = "Source changed since loaded. Reload before saving.", revision = Hash(current) });
            // Conservative structural checks; the full DSL parser remains the authoritative build validator.
            if (!request.Source.Contains("form ", StringComparison.Ordinal) ||
                !request.Source.Contains(" for " + entity + " {", StringComparison.Ordinal))
                return Results.BadRequest("Form declaration does not match entity.");
            var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try {
                await File.WriteAllTextAsync(temp, request.Source, new UTF8Encoding(false));
                File.Move(temp, path, overwrite: true);
            }
            finally { if(File.Exists(temp)) File.Delete(temp); }
            return Results.Ok(new { revision = Hash(request.Source) });
        });
    }
    private static string FindProjectRoot(string start)
    {
        // dotnet run --project can be invoked from different working directories.
        // Find the repository by a known source file, never by a fragile ../.. assumption.
        for (var dir = new DirectoryInfo(Path.GetFullPath(start)); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName,"examples","order-entry","dictionary.json")) &&
                Directory.Exists(Path.Combine(dir.FullName,"examples","forms")))
                return dir.FullName;
        }
        throw new DirectoryNotFoundException("Clarify source tree is unavailable; disable local source editing.");
    }
    private static bool Allowed(string entity, string dictionary) =>
        entity is "Customer" && File.Exists(dictionary); // M0 whitelist; expand through typed dictionary parser later.
    private static string Hash(string source) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source))).ToLowerInvariant();
}
