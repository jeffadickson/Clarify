using System.Text.Json;
using System.Text.RegularExpressions;
using Parlot.Fluent;
using static Parlot.Fluent.Parsers;

namespace Clarify.Dsl;

/// <summary>Source ranges use zero-based offsets and one-based lines/columns.</summary>
public sealed record SourceRange(int Start, int Length, int Line, int Column);
public sealed record FormField(string Name, int X, int Y, int Width, int Height, SourceRange Range);
public sealed record FormDefinition(string Name, string Entity, IReadOnlyList<FormField> Fields);
public sealed record Diagnostic(string Code, string Message, int Line, int Column);
public sealed record FormParseResult(FormDefinition? Form, IReadOnlyList<Diagnostic> Diagnostics)
{
    public bool Success => Form is not null && Diagnostics.Count == 0;
}

/// <summary>
/// Parlot recognizes complete placement declarations; a conservative source
/// scanner maintains block nesting, original source positions, and comments.
/// Unsupported constructs produce diagnostics instead of being silently dropped.
/// This is a compatibility prototype, NOT a full lossless grammar.
/// </summary>
public static class ClarifyFormParser
{
    // Use the real Parlot combinator API, with Eof to reject trailing garbage.
    private static readonly Parser<Parlot.TextSpan> Placement = Capture(
        Terms.Identifier()
            .AndSkip(Terms.Text("at"))
            .AndSkip(Terms.Char('('))
            .And(Terms.Integer())
            .AndSkip(Terms.Char(','))
            .And(Terms.Integer())
            .AndSkip(Terms.Char(')'))
            .AndSkip(Terms.Text("span"))
            .AndSkip(Terms.Char('('))
            .And(Terms.Integer())
            .AndSkip(Terms.Char(','))
            .And(Terms.Integer())
            .AndSkip(Terms.Char(')'))).Eof();

    private static readonly Regex PlacementValues = new(
        @"^\s*(?<name>[A-Za-z_]\w*)\s+at\s*\(\s*(?<x>\d+)\s*,\s*(?<y>\d+)\s*\)\s*span\s*\(\s*(?<w>\d+)\s*,\s*(?<h>\d+)\s*\)\s*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly Regex Header = new(
        @"^form\s+(?<name>\w+)\s+for\s+(?<entity>\w+)\s*\{$", RegexOptions.Compiled);
    private static readonly Regex Block = new(
        @"^(group|grid|row|stack|tabs|tab|actions)\b.*\{$", RegexOptions.Compiled);

    public static FormParseResult Parse(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var errors = new List<Diagnostic>();
        var fields = new List<FormField>();
        string? name = null, entity = null;
        var stack = new Stack<string>();
        var offset = 0;
        var rows = source.Split('\n');
        for (var index = 0; index < rows.Length; index++)
        {
            var raw = rows[index].TrimEnd('\r');
            var text = raw.Split("//", 2, StringSplitOptions.None)[0].Trim();
            var line = index + 1;
            var column = Math.Max(1, raw.IndexOf(text, StringComparison.Ordinal) + 1);
            if (text.Length == 0) { offset += rows[index].Length + 1; continue; }
            if (name is null)
            {
                var header = Header.Match(text);
                if (!header.Success)
                    errors.Add(new("DSL001", "Expected form Name for Entity {", line, column));
                else
                {
                    name = header.Groups["name"].Value;
                    entity = header.Groups["entity"].Value;
                    stack.Push("form");
                }
            }
            else if (text == "}")
            {
                if (stack.Count == 0) errors.Add(new("DSL002", "Unexpected closing brace", line, column));
                else stack.Pop();
            }
            else if (Block.IsMatch(text)) stack.Push(text.Split(' ', 2)[0]);
            else if (text.StartsWith("title ", StringComparison.Ordinal) ||
                     text.StartsWith("size ", StringComparison.Ordinal) ||
                     text.StartsWith("breakpoint ", StringComparison.Ordinal) ||
                     text.StartsWith("actions ", StringComparison.Ordinal)) { }
            else if (stack.TryPeek(out var block) && block == "actions") { }
            else if (!Placement.TryParse(text, out _))
                errors.Add(new("DSL003", "Invalid or unsupported declaration", line, column));
            else
            {
                var m = PlacementValues.Match(text);
                if (!m.Success) errors.Add(new("DSL004", "Invalid field coordinates", line, column));
                else
                {
                    var x = int.Parse(m.Groups["x"].Value);
                    var y = int.Parse(m.Groups["y"].Value);
                    var w = int.Parse(m.Groups["w"].Value);
                    var h = int.Parse(m.Groups["h"].Value);
                    if (x < 1 || y < 1 || w < 1 || h < 1)
                        errors.Add(new("DSL005", "Coordinates and spans must be positive", line, column));
                    else fields.Add(new(m.Groups["name"].Value, x, y, w, h,
                        new(offset + raw.IndexOf(text, StringComparison.Ordinal), text.Length, line, column)));
                }
            }
            offset += rows[index].Length + 1;
        }
        if (stack.Count != 0) errors.Add(new("DSL006", "Unclosed block", rows.Length, 1));
        if (name is null) errors.Add(new("DSL007", "Missing form", 1, 1));
        return new(errors.Count == 0 ? new(name!, entity!, fields) : null, errors);
    }
}
