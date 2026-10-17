using Clarify.Generated;
using Microsoft.EntityFrameworkCore;

// Author-owned composition root. The generated context below is updated from
// dictionary.json; this file survives schema regeneration unchanged.
var builder = WebApplication.CreateBuilder(args);
// The public/no-login demo is deliberately loopback-only, not production auth.
if (builder.Configuration["urls"] is null)
    builder.WebHost.UseUrls("http://127.0.0.1:5279");
builder.Services.AddDbContextFactory<CheckoutDbContext>(options =>
    options.UseSqlite("Data Source=" + Path.Combine(builder.Environment.ContentRootPath, "equipment-checkout.sqlite")));
var app = builder.Build();
app.Use(async (context, next) =>
{
    var remote = context.Connection.RemoteIpAddress;
    if (remote is null || !System.Net.IPAddress.IsLoopback(remote))
    {
        context.Response.StatusCode = 403;
        return;
    }
    if (!HttpMethods.IsGet(context.Request.Method) &&
        !HttpMethods.IsHead(context.Request.Method))
    {
        // Local demo APIs must not accept cross-origin browser writes.
        var origin = context.Request.Headers.Origin.ToString();
        if (!string.IsNullOrEmpty(origin) &&
            (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
             !string.Equals(uri.Host, context.Request.Host.Host, StringComparison.OrdinalIgnoreCase) ||
             uri.Port != context.Request.Host.Port))
        {
            context.Response.StatusCode = 403;
            return;
        }
    }
    await next();
});
app.UseDefaultFiles();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CheckoutDbContext>>();
    await using var db = await factory.CreateDbContextAsync();
    // Initial demo database only; existing deployments require reviewed migrations.
    await db.Database.EnsureCreatedAsync();
    if (!await db.PersonRows.AnyAsync())
    {
        db.PersonRows.AddRange(
            new Person { Name = "Taylor Morgan", Email = "taylor@example.invalid" },
            new Person { Name = "Alex Rivera", Email = "alex@example.invalid" });
    }
    if (!await db.EquipmentRows.AnyAsync())
    {
        db.EquipmentRows.AddRange(
            new Equipment { Name = "Digital oscilloscope", SerialNumber = "SCOPE-001" },
            new Equipment { Name = "Multimeter", SerialNumber = "METER-002" },
            new Equipment { Name = "Thermal camera", SerialNumber = "THERM-003" });
    }
    await db.SaveChangesAsync();
}

// Read models are projected from EF Core relationships rather than written by
// the frontend. All database calls are asynchronous and cancellation-aware.
app.MapGet("/api/snapshot", async (IDbContextFactory<CheckoutDbContext> factory, CancellationToken ct) =>
{
    await using var db = await factory.CreateDbContextAsync(ct);
    var people = await db.PersonRows.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    var equipment = await db.EquipmentRows.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    var loans = await (
        from l in db.LoanRows.AsNoTracking()
        join e in db.EquipmentRows on l.EquipmentId equals e.Id
        join p in db.PersonRows on l.PersonId equals p.Id
        orderby l.Id descending
        select new LoanView(l.Id, e.Id, e.Name, p.Name, l.CheckedOut, l.Returned)
    ).ToListAsync(ct);
    return Results.Ok(new { people, equipment, loans });
});

app.MapPost("/api/people", async (PersonInput input,
    IDbContextFactory<CheckoutDbContext> factory, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length > 120 ||
        (input.Email?.Length ?? 0) > 200)
        return Results.BadRequest(new { error = "Name is required (maximum 120 characters); email maximum is 200." });
    await using var db = await factory.CreateDbContextAsync(ct);
    var person = new Person { Name = input.Name.Trim(), Email = input.Email?.Trim() };
    db.PersonRows.Add(person);
    await db.SaveChangesAsync(ct);
    return Results.Ok(new { person.Id });
});

app.MapPost("/api/equipment", async (EquipmentInput input,
    IDbContextFactory<CheckoutDbContext> factory, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Length > 120 ||
        string.IsNullOrWhiteSpace(input.SerialNumber) || input.SerialNumber.Length > 80)
        return Results.BadRequest(new { error = "Name and serial number are required." });
    await using var db = await factory.CreateDbContextAsync(ct);
    var serial = input.SerialNumber.Trim();
    if (await db.EquipmentRows.AnyAsync(x => x.SerialNumber == serial, ct))
        return Results.Conflict(new { error = "An item with this serial number already exists." });
    var item = new Equipment { Name = input.Name.Trim(), SerialNumber = serial };
    db.EquipmentRows.Add(item);
    try
    {
        await db.SaveChangesAsync(ct);
    }
    catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.Sqlite.SqliteException { SqliteErrorCode: 19 })
    {
        // The unique database index also rejects concurrent duplicate requests.
        return Results.Conflict(new { error = "An item with this serial number already exists." });
    }
    return Results.Ok(new { item.Id });
});

app.MapPost("/api/loans", async (LoanInput input,
    IDbContextFactory<CheckoutDbContext> factory, CancellationToken ct) =>
{
    await using var db = await factory.CreateDbContextAsync(ct);
    await using var transaction = await db.Database.BeginTransactionAsync(ct);
    if (!await db.PersonRows.AnyAsync(x => x.Id == input.PersonId, ct) ||
        !await db.EquipmentRows.AnyAsync(x => x.Id == input.EquipmentId, ct))
        return Results.BadRequest(new { error = "Choose an existing person and equipment item." });
    if (await db.LoanRows.AnyAsync(x => x.EquipmentId == input.EquipmentId && !x.Returned, ct))
        return Results.Conflict(new { error = "Equipment is already checked out." });
    var loan = new Loan { PersonId = input.PersonId, EquipmentId = input.EquipmentId,
        CheckedOut = DateOnly.FromDateTime(DateTime.UtcNow), Returned = false };
    db.LoanRows.Add(loan);
    await db.SaveChangesAsync(ct);
    await transaction.CommitAsync(ct);
    return Results.Ok(new { loan.Id });
});

app.MapPost("/api/loans/{id:long}/return", async (long id,
    IDbContextFactory<CheckoutDbContext> factory, CancellationToken ct) =>
{
    await using var db = await factory.CreateDbContextAsync(ct);
    var loan = await db.LoanRows.SingleOrDefaultAsync(x => x.Id == id, ct);
    if (loan is null) return Results.NotFound(new { error = "Loan not found." });
    if (loan.Returned) return Results.Conflict(new { error = "Item has already been returned." });
    loan.Returned = true;
    await db.SaveChangesAsync(ct);
    return Results.Ok(new { loan.Id, loan.Returned });
});

app.Run();

/// <summary>Validatable input for a new borrower.</summary>
/// <param name="Name">Person's display name.</param>
/// <param name="Email">Optional contact email address.</param>
public sealed record PersonInput(string Name, string? Email);
/// <summary>Validatable input for inventory equipment.</summary>
/// <param name="Name">Equipment display name.</param>
/// <param name="SerialNumber">Unique physical label supplied by the user.</param>
public sealed record EquipmentInput(string Name, string SerialNumber);
/// <summary>Identifies the borrower and physical item to be checked out.</summary>
/// <param name="PersonId">Existing person ID.</param>
/// <param name="EquipmentId">Existing equipment ID.</param>
public sealed record LoanInput(long PersonId, long EquipmentId);
/// <summary>Joined presentation view of one checkout transaction.</summary>
/// <param name="Id">Loan identifier.</param>
/// <param name="EquipmentId">Stable equipment identifier.</param>
/// <param name="Equipment">Equipment name.</param>
/// <param name="Person">Borrower name.</param>
/// <param name="CheckedOut">Date of checkout.</param>
/// <param name="Returned">Whether item has been returned.</param>
public sealed record LoanView(long Id, long EquipmentId, string Equipment, string Person, DateOnly CheckedOut, bool Returned);
