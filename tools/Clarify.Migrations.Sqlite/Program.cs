using System.Text.Json;
using Microsoft.Data.Sqlite;

// Deliberately narrow, explicit SQLite migration executor. No automatic
// database connection, no destructive DDL, and no unattended deployment.
// Usage:
// dotnet run --project tools/Clarify.Migrations.Sqlite --
//   --plan product-notes.migration.json --db sales.db
//   --approve clarify-sqlite-... --backup sales.backup.db
static string Required(string[] args, string name)
{
    var i = Array.IndexOf(args, name);
    if (i < 0 || i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
        throw new ArgumentException("Missing required argument " + name);
    return args[i + 1];
}
static string SafeName(string name)
{
    if (string.IsNullOrEmpty(name) ||
        !(char.IsAsciiLetter(name[0]) || name[0] == '_') ||
        name.Skip(1).Any(c => !(char.IsAsciiLetterOrDigit(c) || c == '_')))
        throw new InvalidOperationException("Unsafe SQL identifier");
    return "\"" + name + "\"";
}
static string RequiredProperty(JsonElement value, string property) =>
    value.GetProperty(property).GetString()
    ?? throw new InvalidOperationException("Missing " + property);
// Recompute the planner's deterministic fingerprint to detect modified plans.
// FNV-1a is an integrity/drift check, NOT a cryptographic signature.
static string ComputePlanId(JsonElement plan)
{
    var canonical = JsonSerializer.Serialize(new {
        precondition = plan.GetProperty("precondition"),
        operations = plan.GetProperty("operations")
    });
    uint hash = 2166136261;
    foreach (var ch in canonical)
        hash = unchecked((hash ^ ch) * 16777619);
    return "clarify-sqlite-" + hash.ToString("x8");
}
static async Task ExecuteAsync(string[] args)
{
    var planPath = Path.GetFullPath(Required(args, "--plan"));
    var dbPath = Path.GetFullPath(Required(args, "--db"));
    var backupPath = Path.GetFullPath(Required(args, "--backup"));
    var approved = Required(args, "--approve");
    if (dbPath == backupPath || File.Exists(backupPath))
        throw new InvalidOperationException("Backup must be a new, distinct file");
    if (!File.Exists(dbPath)) throw new FileNotFoundException("Database does not exist", dbPath);
    using var document = JsonDocument.Parse(await File.ReadAllTextAsync(planPath));
    var plan = document.RootElement;
    if (RequiredProperty(plan, "format") != "clarify-sqlite-migration/v1" ||
        RequiredProperty(plan, "provider") != "sqlite" ||
        !plan.GetProperty("executable").GetBoolean() ||
        plan.GetProperty("blocked").GetArrayLength() != 0)
        throw new InvalidOperationException("Migration plan is not executable");
    var id = RequiredProperty(plan, "planId");
    if (id != ComputePlanId(plan))
        throw new InvalidOperationException("Plan content does not match its identifier");
    if (RequiredProperty(plan.GetProperty("precondition"), "database") != RequiredProperty(plan, "database"))
        throw new InvalidOperationException("Plan database and precondition disagree");
    if (approved != id)
        throw new InvalidOperationException("Approval must match the exact plan ID");
    var operations = plan.GetProperty("operations").EnumerateArray().ToArray();
    if (operations.Length == 0) throw new InvalidOperationException("Empty migration");
    var connectionString = new SqliteConnectionStringBuilder {
        DataSource = dbPath, Mode = SqliteOpenMode.ReadWrite, ForeignKeys = true
    }.ToString();
    await using var connection = new SqliteConnection(connectionString);
    await connection.OpenAsync();
    // Exact schema preflight. Refuse drift before touching the database.
    foreach (var table in plan.GetProperty("precondition").GetProperty("expectedTables").EnumerateArray())
    {
        var tableName = RequiredProperty(table, "name");
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "PRAGMA table_info(" + SafeName(tableName) + ");";
        var actual = new Dictionary<string, string>(StringComparer.Ordinal);
        await using (var reader = await cmd.ExecuteReaderAsync())
            while (await reader.ReadAsync())
                actual.Add(reader.GetString(1), reader.GetString(2).ToUpperInvariant());
        var expected = table.GetProperty("columns").EnumerateArray().ToArray();
        if (actual.Count != expected.Length)
            throw new InvalidOperationException("Schema drift in " + tableName);
        foreach (var column in expected)
        {
            var name = RequiredProperty(column, "name");
            var type = RequiredProperty(column, "type");
            if (!actual.TryGetValue(name, out var found) ||
                !string.Equals(found, type, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Schema drift in " + tableName + "." + name);
        }
    }
    // Validate all operations before backup or DDL.
    foreach (var operation in operations)
    {
        if (RequiredProperty(operation, "kind") != "add-nullable-column")
            throw new InvalidOperationException("Unsupported operation");
        var type = RequiredProperty(operation, "type");
        if (type is not ("TEXT" or "INTEGER"))
            throw new InvalidOperationException("Unsupported SQLite type");
        var expected = "ALTER TABLE " + SafeName(RequiredProperty(operation, "table")) +
                       " ADD COLUMN " + SafeName(RequiredProperty(operation, "column")) +
                       " " + type + " NULL;";
        if (RequiredProperty(operation, "sql") != expected)
            throw new InvalidOperationException("Operation SQL does not match safe generated DDL");
    }
    // SQLite online backup API is used instead of copying an open database file.
    await using (var backup = new SqliteConnection(new SqliteConnectionStringBuilder {
        DataSource = backupPath, Mode = SqliteOpenMode.ReadWriteCreate
    }.ToString()))
    {
        await backup.OpenAsync();
        connection.BackupDatabase(backup);
    }
    try
    {
        await using var tx = await connection.BeginTransactionAsync();
        await using (var history = connection.CreateCommand())
        {
            history.Transaction = (SqliteTransaction)tx;
            history.CommandText = "CREATE TABLE IF NOT EXISTS \"__ClarifyMigrationHistory\" (PlanId TEXT PRIMARY KEY NOT NULL, AppliedAt TEXT NOT NULL);";
            await history.ExecuteNonQueryAsync();
        }
        await using (var check = connection.CreateCommand())
        {
            check.Transaction = (SqliteTransaction)tx;
            check.CommandText = "SELECT COUNT(*) FROM \"__ClarifyMigrationHistory\" WHERE PlanId = $id;";
            check.Parameters.AddWithValue("$id", id);
            if (Convert.ToInt64(await check.ExecuteScalarAsync()) != 0)
                throw new InvalidOperationException("Plan already applied");
        }
        foreach (var operation in operations)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = (SqliteTransaction)tx;
            command.CommandText = RequiredProperty(operation, "sql");
            await command.ExecuteNonQueryAsync();
        }
        await using (var history = connection.CreateCommand())
        {
            history.Transaction = (SqliteTransaction)tx;
            history.CommandText = "INSERT INTO \"__ClarifyMigrationHistory\" (PlanId, AppliedAt) VALUES ($id, $date);";
            history.Parameters.AddWithValue("$id", id);
            history.Parameters.AddWithValue("$date", DateTimeOffset.UtcNow.ToString("O"));
            await history.ExecuteNonQueryAsync();
        }
        await tx.CommitAsync();
    }
    catch
    {
        // Transaction rollback is handled by transaction disposal. Backup remains.
        throw;
    }
    Console.WriteLine("Applied " + id + "; backup: " + backupPath);
}
try { await ExecuteAsync(args); }
catch (Exception ex)
{
    Console.Error.WriteLine("Clarify migration refused or failed: " + ex.Message);
    Environment.ExitCode = 1;
}
