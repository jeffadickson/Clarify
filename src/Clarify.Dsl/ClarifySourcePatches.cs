using System.Text.RegularExpressions;

namespace Clarify.Dsl;

/// <summary>
/// A source-aware visual edit changes exactly one field placement. It does not
/// reserialize the AST, so whitespace, comments and unrelated DSL survive.
/// </summary>
public static class ClarifySourcePatches
{
    private static readonly Regex Coordinates = new(
        @"\bat\s*\(\s*\d+\s*,\s*\d+\s*\)\s*span\s*\(\s*\d+\s*,\s*\d+\s*\)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string MoveField(
        string source, string fieldName, int x, int y, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (x < 1 || y < 1 || width < 1 || height < 1)
            throw new ArgumentOutOfRangeException(nameof(x), "Grid positions and spans must be positive.");
        var before = ClarifyFormParser.Parse(source);
        if (!before.Success) throw new InvalidOperationException("Source is invalid; fix diagnostics first.");
        var matches = before.Form!.Fields.Where(f => f.Name == fieldName).ToArray();
        if (matches.Length != 1)
            throw new InvalidOperationException("Field must occur exactly once: " + fieldName);
        // Mirror visual editor collision checks for fields in this form.
        if (before.Form.Fields.Any(other =>
            other.Name != fieldName &&
            x < other.X + other.Width && other.X < x + width &&
            y < other.Y + other.Height && other.Y < y + height))
            throw new InvalidOperationException("Field would overlap an existing control.");
        var span = matches[0].Range;
        var text = source.Substring(span.Start, span.Length);
        var match = Coordinates.Match(text);
        if (!match.Success || Coordinates.Matches(text).Count != 1)
            throw new InvalidOperationException("Cannot safely locate field coordinates.");
        var patch = $"at ({x},{y}) span ({width},{height})";
        var at = span.Start + match.Index;
        var updated = source[..at] + patch + source[(at + match.Length)..];
        var after = ClarifyFormParser.Parse(updated);
        if (!after.Success) throw new InvalidOperationException("Edited source failed native validation.");
        return updated;
    }
}
