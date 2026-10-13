#nullable enable
using EtlKit.ClickHouse.ConnectionManager;
using EtlKit.ConnectionManager;
using EtlKit.Primitives;

namespace EtlKit.Common.Tests;

/// <summary>
/// Real connection managers for the tests. They are created but never opened, so no database is
/// involved; they exist to exercise what a manager reports about its engine.
/// </summary>
internal static class TestConnectionManagers
{
    /// <summary>
    /// Names of every concrete connection manager in the solution, for <see cref="Get"/>. SQL
    /// Server appears twice: the native manager and the ODBC one report the same engine.
    /// </summary>
    public static readonly string[] Names =
    [
        "SqlServer",
        "SqlServerOdbc",
        "Postgres",
        "MySql",
        "SQLite",
        "Access",
        "ClickHouse",
        "Adomd",
    ];

    /// <summary>
    /// Creates the connection manager that represents <paramref name="engine"/>. There is none for
    /// <see cref="ConnectionManagerType.Unknown"/>: no manager reports it.
    /// </summary>
    public static IConnectionManager Create(ConnectionManagerType engine) =>
        engine switch
        {
            ConnectionManagerType.SqlServer => new SqlConnectionManager(
                "Data Source=localhost;Initial Catalog=sample"
            ),
            ConnectionManagerType.Postgres => new PostgresConnectionManager(
                "Host=localhost;Database=sample"
            ),
            ConnectionManagerType.MySql => new MySqlConnectionManager(
                "Server=localhost;Database=sample"
            ),
            ConnectionManagerType.SQLite => new SQLiteConnectionManager("Data Source=:memory:"),
            ConnectionManagerType.Access => new AccessOdbcConnectionManager("Dbq=sample.accdb"),
            ConnectionManagerType.ClickHouse => new ClickHouseConnectionManager(
                "Host=localhost;Database=sample"
            ),
            ConnectionManagerType.Adomd => new AdomdConnectionManager("Data Source=localhost"),
            _ => throw new ArgumentOutOfRangeException(
                nameof(engine),
                engine,
                "No connection manager reports this engine."
            ),
        };

    // Ownership of each manager passes to the returned ManagerCase, which disposes it.
#pragma warning disable CA2000
    /// <summary>
    /// Creates the connection manager called <paramref name="name"/> (one of <see cref="Names"/>)
    /// together with the type conventions it supplies.
    /// </summary>
    public static ManagerCase Get(string name) =>
        name switch
        {
            "SqlServer" => Case(
                name,
                new SqlConnectionManager("Data Source=localhost"),
                m => m.TypeConventions
            ),
            "SqlServerOdbc" => Case(
                name,
                new SqlOdbcConnectionManager("Server=localhost"),
                m => m.TypeConventions
            ),
            "Postgres" => Case(
                name,
                new PostgresConnectionManager("Host=localhost"),
                m => m.TypeConventions
            ),
            "MySql" => Case(
                name,
                new MySqlConnectionManager("Server=localhost"),
                m => m.TypeConventions
            ),
            "SQLite" => Case(
                name,
                new SQLiteConnectionManager("Data Source=:memory:"),
                m => m.TypeConventions
            ),
            "Access" => Case(
                name,
                new AccessOdbcConnectionManager("Dbq=sample.accdb"),
                m => m.TypeConventions
            ),
            "ClickHouse" => Case(
                name,
                new ClickHouseConnectionManager("Host=localhost"),
                m => m.TypeConventions
            ),
            "Adomd" => Case(
                name,
                new AdomdConnectionManager("Data Source=localhost"),
                m => m.TypeConventions
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
        };
#pragma warning restore CA2000

    private static ManagerCase Case<T>(
        string name,
        T manager,
        Func<T, IDbTypeConventions> conventionsOf
    )
        where T : IConnectionManager => new(name, manager, conventionsOf(manager));
}

/// <summary>A connection manager together with the type conventions it supplies.</summary>
internal sealed class ManagerCase(
    string name,
    IConnectionManager manager,
    IDbTypeConventions conventions
) : IDisposable
{
    public string Name { get; } = name;

    public IConnectionManager Manager { get; } = manager;

    public IDbTypeConventions Conventions { get; } = conventions;

    public void Dispose() => Manager.Dispose();
}
