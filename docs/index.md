# Clarify API Reference

The reference is built from C# triple-slash XML documentation comments using [DocFX](https://dotnet.github.io/docfx/).

See [Studio Authoring Guide](STUDIO-AUTHOR-GUIDE.md), [Native Parser Evaluation](PARLOT-EXPERIMENT.md), [ER Diagram](ER-DIAGRAM.md), and [Documentation Standards](CSHARP-DOCUMENTATION.md).

Generated C# must include meaningful <summary>, <param>, <returns> and <exception> tags where relevant. Comment coverage is audited in CI; missing existing comments are tracked rather than silently suppressed.
