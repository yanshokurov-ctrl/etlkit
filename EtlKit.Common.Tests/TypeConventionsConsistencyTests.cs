#nullable enable
using EtlKit.ConnectionManager;
using Xunit;

namespace EtlKit.Common.Tests;

/// <summary>
/// Cross-checks every concrete connection manager against the obsolete engine-keyed rules in
/// <see cref="DataTypeConverter"/> over a broad set of column types. The contract tests pin
/// literal expectations; this guards the wiring (each manager supplies the rules of the engine it
/// reports) and keeps the one rule that exists twice, ClickHouse's, from drifting apart.
/// </summary>
public sealed class TypeConventionsConsistencyTests
{
    private static readonly string[] DataTypes =
    [
        "TEXT",
        "text",
        " Text ",
        "te xt",
        "ntext",
        "INT",
        "int",
        "  int  ",
        "BIGINT",
        "bigint",
        "BigInt",
        "integer",
        "smallint",
        "tinyint",
        "bit",
        "decimal(18,2)",
        "float",
        "VARCHAR(10)",
        "varchar(50)",
        "varchar(255)",
        "varchar(256)",
        "nvarchar(50)",
        "NVARCHAR(300)",
        "nchar(10)",
        "char(1000)",
        "varchar(max)",
        "nvarchar(max)",
        "DATETIME",
        "datetime",
        "  DateTime  ",
        "datetime2",
        "timestamp",
        "uniqueidentifier",
        "Int32",
        "String",
        "Nullable(Int32)",
        "Nullable(String)",
    ];

    public static TheoryData<string> ManagerNames => new(TestConnectionManagers.Names);

    [Theory]
    [MemberData(nameof(ManagerNames))]
    public void ManagerConventions_MatchTheEngineKeyedRules(string name)
    {
        using var managerCase = TestConnectionManagers.Get(name);
        var engine = managerCase.Manager.ConnectionManagerType;

        var mismatches = new List<string>();
        foreach (var dataType in DataTypes)
        {
            foreach (var allowNulls in new[] { false, true })
            {
                var column = new TableColumn("col", dataType, allowNulls);
#pragma warning disable CS0618
                var expected = DataTypeConverter.TryGetDBSpecificType(column, engine);
#pragma warning restore CS0618
                var actual = managerCase.Conventions.GetDbSpecificType(column);
                if (actual != expected)
                    mismatches.Add(
                        $"'{dataType}' allowNulls={allowNulls}: manager '{actual}', engine-keyed '{expected}'"
                    );
            }
        }

        Assert.Empty(mismatches);
    }
}
