#nullable enable
using EtlKit.ConnectionManager;
using EtlKit.Primitives;
using Xunit;

namespace EtlKit.Common.Tests;

/// <summary>
/// Runs the <see cref="DbTypeConventionsContractTests"/> expectations against the obsolete,
/// engine-keyed <see cref="DataTypeConverter.TryGetDBSpecificType"/>, which keeps callers that
/// only know the engine type working. It also covers what only the facade can express: an engine
/// without a connection manager, and an engine value that does not exist. The obsolete member is
/// the subject here on purpose. Delete this class together with the member once it is removed.
/// </summary>
#pragma warning disable CS0618
public sealed class DataTypeConverterConventionsForwardingTests : DbTypeConventionsContractTests
{
    protected override string MapType(ConnectionManagerType engine, ITableColumn column) =>
        DataTypeConverter.TryGetDBSpecificType(column, engine);

    [Theory]
    [InlineData("text", false)]
    [InlineData("  int  ", true)]
    [InlineData("nvarchar(300)", false)]
    [InlineData("datetime", true)]
    public void Unknown_ReturnsDataTypeVerbatim(string dataType, bool allowNulls) =>
        Assert.Equal(
            dataType,
            DataTypeConverter.TryGetDBSpecificType(
                new TableColumn("col", dataType, allowNulls),
                ConnectionManagerType.Unknown
            )
        );

    [Fact]
    public void UndefinedEngineValue_ReturnsDataTypeVerbatim() =>
        Assert.Equal(
            "  nvarchar(300) ",
            DataTypeConverter.TryGetDBSpecificType(
                new TableColumn("col", "  nvarchar(300) "),
                (ConnectionManagerType)999
            )
        );
}
#pragma warning restore CS0618
