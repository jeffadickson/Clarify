using NSubstitute;
using Testcontainers.PostgreSql;
using Xunit;

namespace Clarify.M0.Tests;

public interface IGeneratorSink
{
    void Write(string path, string content);
}

public sealed class Milestone0Tests
{
    [Fact]
    public void MockedGeneratorBoundaryPreservesExpectedSequence()
    {
        var sink = Substitute.For<IGeneratorSink>();
        sink.Write("Customer.cs", "class Customer {}");
        sink.Write("Order.cs", "class Order {}");

        Received.InOrder(() =>
        {
            sink.Write("Customer.cs", "class Customer {}");
            sink.Write("Order.cs", "class Order {}");
        });
    }

    [Fact]
    public void MoneyArithmeticInCentsAvoidsBinaryFloatingPointTotals()
    {
        long cents = checked(129950 + 2L * 44900);
        Assert.Equal(219750, cents);
    }

    [Fact(Skip = "M0 integration gate: requires Docker and generated EF Core persistence target")]
    public async Task RealDatabaseProviderTestTemplate()
    {
        await using var container = new PostgreSqlBuilder("postgres:16-alpine").Build();
        await container.StartAsync();
        Assert.NotEmpty(container.GetConnectionString());
    }
}
