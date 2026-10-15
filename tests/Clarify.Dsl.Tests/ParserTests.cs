using Clarify.Dsl;
using Xunit;
namespace Clarify.Dsl.Tests;
public sealed class ParserTests
{
    [Fact]
    public void ExistingNestedCustomerFormParses()
    {
        const string source = """
            form CustomerEditor for Customer {
              title "Customer Editor"
              group "Contact" {
                grid columns 12 gap 12 {
                  Name at (1,1) span (6,1) // retained comment
                  Email at (7,1) span (6,1)
                  City at (1,2) span (6,1)
                }
              }
            }
            """;
        var result = ClarifyFormParser.Parse(source);
        Assert.True(result.Success, string.Join("; ", result.Diagnostics.Select(x => x.Message)));
        Assert.Equal(3, result.Form!.Fields.Count);
        Assert.Equal("Customer", result.Form.Entity);
        Assert.Equal(5, result.Form.Fields[0].Range.Line);
    }

    [Fact]
    public void InvalidDeclarationIsReportedWithLine()
    {
        var result = ClarifyFormParser.Parse("form F for Customer {\n  Name at (1,) span (6,1)\n}");
        Assert.False(result.Success);
        Assert.Contains(result.Diagnostics, x => x.Code == "DSL003" && x.Line == 2);
    }

    [Fact]
    public void UnclosedBlockIsRejected()
    {
        var result = ClarifyFormParser.Parse("form F for Customer {\n  grid columns 12 {\n  Name at (1,1) span (6,1)\n}");
        Assert.Contains(result.Diagnostics, x => x.Code == "DSL006");
    }
    [Fact]
    public void NativeVisualEditPreservesCommentsAndOtherFields()
    {
        var original = """
            // @clarify-template {"id":"clarify.sample.example","version":"0.2.0"}
            form CustomerEditor for Customer {
              grid columns 12 {
                // @clarify-control {"field":"Name","kind":"text"}
                Name at (1,1) span (6,1) // author's comment
                Email at (7,1) span (6,1)
              }
            }
            """;
        var updated=ClarifySourcePatches.MoveField(original,"Name",2,2,5,1);
        Assert.Contains("Name at (2,2) span (5,1) // author's comment",updated);
        Assert.Contains("Email at (7,1) span (6,1)",updated);
        Assert.Contains("// @clarify-template",updated);
        Assert.Contains("// @clarify-control",updated);
        Assert.True(ClarifyFormParser.Parse(updated).Success);
    }

    [Fact]
    public void NativeVisualEditRejectsUnsafeInput()
    {
        const string source="form F for Customer {\nName at (1,1) span (6,1)\n}\n";
        Assert.Throws<ArgumentOutOfRangeException>(
            ()=>ClarifySourcePatches.MoveField(source,"Name",0,1,6,1));
        Assert.Throws<InvalidOperationException>(
            ()=>ClarifySourcePatches.MoveField(source,"Missing",1,1,6,1));
    }

    [Fact]
    public void RejectsExtraContentAfterFormAndDuplicateFields()
    {
        var trailing = ClarifyFormParser.Parse("form F for Customer {\n}\nName at (1,1) span (6,1)");
        Assert.Contains(trailing.Diagnostics, x => x.Code == "DSL008");
        var duplicate = ClarifyFormParser.Parse(
            "form F for Customer {\nName at (1,1) span (6,1)\nName at (7,1) span (6,1)\n}");
        Assert.Contains(duplicate.Diagnostics, x => x.Code == "DSL009");
    }

    [Fact]
    public void SourcePatchRejectsOverlappingFields()
    {
        const string source = "form F for Customer {\nName at (1,1) span (6,1)\nEmail at (7,1) span (6,1)\n}";
        Assert.Throws<InvalidOperationException>(() =>
            ClarifySourcePatches.MoveField(source, "Email", 1, 1, 6, 1));
    }

    [Fact]
    public void GridBoundsAndInvalidColumnsAreDiagnosed()
    {
        var outside=ClarifyFormParser.Parse(
            "form F for Customer {\n grid columns 12 {\n Name at (11,1) span (6,1)\n }\n}");
        Assert.Contains(outside.Diagnostics, d=>d.Code=="DSL011");
        var invalid=ClarifyFormParser.Parse(
            "form F for Customer {\n grid columns 0 {\n Name at (1,1) span (6,1)\n }\n}");
        Assert.Contains(invalid.Diagnostics, d=>d.Code=="DSL010");
    }

}
