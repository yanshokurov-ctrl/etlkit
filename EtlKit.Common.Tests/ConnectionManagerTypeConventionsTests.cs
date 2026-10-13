#nullable enable
using EtlKit.ConnectionManager;
using EtlKit.Primitives;
using Xunit;

namespace EtlKit.Common.Tests;

/// <summary>
/// Runs the <see cref="DbTypeConventionsContractTests"/> expectations against the conventions
/// that real connection managers supply through <c>TypeConventions</c>, which is where
/// <c>CreateTableTask</c> takes the SQL type of a column from.
/// </summary>
public sealed class ConnectionManagerTypeConventionsTests : DbTypeConventionsContractTests
{
    // The manager names in TestConnectionManagers match the ConnectionManagerType names.
    protected override string MapType(ConnectionManagerType engine, ITableColumn column)
    {
        using var managerCase = TestConnectionManagers.Get(engine.ToString());

        return managerCase.Conventions.GetDbSpecificType(column);
    }

    // No manager may be left without conventions by accident: a manager that reports an engine
    // must supply that engine's rules, which the contract above pins per engine.
    [Theory]
    [InlineData("SqlServer", ConnectionManagerType.SqlServer)]
    [InlineData("SqlServerOdbc", ConnectionManagerType.SqlServer)]
    [InlineData("Postgres", ConnectionManagerType.Postgres)]
    [InlineData("MySql", ConnectionManagerType.MySql)]
    [InlineData("SQLite", ConnectionManagerType.SQLite)]
    [InlineData("Access", ConnectionManagerType.Access)]
    [InlineData("ClickHouse", ConnectionManagerType.ClickHouse)]
    [InlineData("Adomd", ConnectionManagerType.Adomd)]
    public void Manager_ReportsTheEngineItsConventionsAreFor(
        string name,
        ConnectionManagerType expectedEngine
    )
    {
        using var managerCase = TestConnectionManagers.Get(name);

        Assert.Equal(expectedEngine, managerCase.Manager.ConnectionManagerType);
    }

    [Fact]
    public void EveryConcreteManager_IsListed()
    {
        var engines = TestConnectionManagers
            .Names.Select(name =>
            {
                using var managerCase = TestConnectionManagers.Get(name);
                return managerCase.Manager.ConnectionManagerType;
            })
            .ToHashSet();

        var missing = Enum.GetValues<ConnectionManagerType>()
            .Where(engine => engine != ConnectionManagerType.Unknown && !engines.Contains(engine))
            .ToList();

        Assert.Empty(missing);
    }
}
