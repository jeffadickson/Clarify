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
        Assert.Equal(4, result.Form.Fields[0].Range.Line);
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
}
