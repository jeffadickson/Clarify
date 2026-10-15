using System.Text.Json;
using Clarify.Dsl;
using Xunit;

namespace Clarify.Dsl.Tests;

/// <summary>
/// Compatibility tests against active JavaScript parser fixtures, including all
/// 40 template packages and existing manually authored source.
/// This gate does not switch Studio to native parsing.
/// </summary>
public sealed class CompatibilityTests
{
    [Fact]
    public void AllGeneratedFormsMatchActiveParser()
    {
        var file=Path.Combine(AppContext.BaseDirectory,"Fixtures","parity.json");
        Assert.True(File.Exists(file),"Run: node tools/export-dsl-parity-fixtures.mjs");
        using var json=JsonDocument.Parse(File.ReadAllText(file));
        var fixtures=json.RootElement.EnumerateArray().ToList();
        Assert.True(fixtures.Count>=41,"Expected all 40 samples and existing Customer form");
        foreach(var fixture in fixtures)
        {
            var label=fixture.GetProperty("label").GetString();
            var result=ClarifyFormParser.Parse(fixture.GetProperty("source").GetString()!);
            Assert.True(result.Success,label+": "+string.Join("; ",result.Diagnostics.Select(x=>x.Message)));
            Assert.Equal(fixture.GetProperty("form").GetString(),result.Form!.Name);
            Assert.Equal(fixture.GetProperty("entity").GetString(),result.Form.Entity);
            var expected=fixture.GetProperty("fields").EnumerateArray().ToArray();
            Assert.Equal(expected.Length,result.Form.Fields.Count);
            for(var i=0;i<expected.Length;i++)
            {
                var actual=result.Form.Fields[i];
                Assert.Equal(expected[i].GetProperty("name").GetString(),actual.Name);
                Assert.Equal(expected[i].GetProperty("x").GetInt32(),actual.X);
                Assert.Equal(expected[i].GetProperty("y").GetInt32(),actual.Y);
                Assert.Equal(expected[i].GetProperty("width").GetInt32(),actual.Width);
                Assert.Equal(expected[i].GetProperty("height").GetInt32(),actual.Height);
                Assert.Equal(expected[i].GetProperty("line").GetInt32(),actual.Range.Line);
            }
        }
    }
}
