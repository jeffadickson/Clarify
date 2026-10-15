using Clarify.Demo.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Clarify.M0.Tests;

/// <summary>
/// Tests the actual SQLite implementation, not an in-memory EF provider.
/// These contracts are the baseline for future dictionary-generated browses.
/// </summary>
public sealed class AsyncBrowseTests
{
    [Fact]
    public async Task PagedBrowseIsBoundedStableAndCancellable()
    {
        var path=Path.Combine(Path.GetTempPath(),"clarify-async-"+Guid.NewGuid().ToString("N")+".db");
        try
        {
            var options=new DbContextOptionsBuilder<CustomerDb>()
                .UseSqlite("Data Source="+path).Options;
            await using(var db=new CustomerDb(options))
            {
                await db.Database.EnsureCreatedAsync();
                for(var i=0;i<53;i++)
                    db.Customers.Add(new CustomerRow{Name="Customer",City=i.ToString()});
                await db.SaveChangesAsync();
            }
            var store=new SqliteCustomerStore(new Factory(options));
            var first=await store.PageAsync(0,25);
            var second=await store.PageAsync(1,25);
            var last=await store.PageAsync(2,25);
            Assert.Equal(25,first.Items.Count);
            Assert.Equal(25,second.Items.Count);
            Assert.Equal(3,last.Items.Count);
            Assert.True(first.HasNext);
            Assert.False(last.HasNext);
            Assert.Empty(first.Items.Select(x=>x.Id).Intersect(second.Items.Select(x=>x.Id)));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                ()=>store.PageAsync(0,201));
            using var cancelled=new CancellationTokenSource();
            cancelled.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                ()=>store.PageAsync(0,25,cancelled.Token));
        }
        finally {if(File.Exists(path))File.Delete(path);}
    }
    private sealed class Factory(DbContextOptions<CustomerDb> options):IDbContextFactory<CustomerDb>
    {
        public CustomerDb CreateDbContext()=>new(options);
    }
}
