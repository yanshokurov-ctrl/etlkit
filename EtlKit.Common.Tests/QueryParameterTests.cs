#nullable enable
using System.Data;
using EtlKit.Primitives;
using Xunit;

namespace EtlKit.Common.Tests;

/// <summary>
/// Characterization tests for <see cref="QueryParameter"/>, which derives its <see
/// cref="QueryParameter.DBType"/> from a SQL type name via <see
/// cref="EtlKit.ConnectionManager.DataTypeConverter.GetDBType"/>. They pin the behavior the type
/// must keep when it moves to another assembly (see
/// docs/tech-debt/TECH-DEBT-DataTypeConverter-Driver-Split.md).
/// </summary>
public sealed class QueryParameterTests
{
    // The type keeps its name and namespace when it moves, so existing callers still compile.
    [Fact]
    public void FullName_IsStable() =>
        Assert.Equal("EtlKit.QueryParameter", typeof(QueryParameter).FullName);

    [Fact]
    public void ImplementsIQueryParameter() =>
        Assert.IsAssignableFrom<IQueryParameter>(new QueryParameter("p", "int", 1));

    // ── constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_StoresNameTypeAndValue()
    {
        var parameter = new QueryParameter("id", "bigint", 42L);

        Assert.Equal("id", parameter.Name);
        Assert.Equal("bigint", parameter.Type);
        Assert.Equal(42L, parameter.Value);
    }

    [Fact]
    public void Constructor_NullValue_IsStoredAsDBNull() =>
        Assert.Same(DBNull.Value, new QueryParameter("p", "int", null).Value);

    [Fact]
    public void Constructor_NonNullValue_IsStoredUnchanged()
    {
        var value = new object();

        Assert.Same(value, new QueryParameter("p", "varchar(10)", value).Value);
    }

    // ── DBType ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("int", DbType.Int32)]
    [InlineData("bigint", DbType.Int64)]
    [InlineData("bit", DbType.Boolean)]
    [InlineData("decimal(18,2)", DbType.Decimal)]
    [InlineData("datetime2", DbType.DateTime)]
    [InlineData("uniqueidentifier", DbType.Guid)]
    [InlineData("varchar(10)", DbType.String)]
    [InlineData("something-unknown", DbType.String)]
    public void DBType_IsDerivedFromTheSqlTypeName(string type, DbType expected) =>
        Assert.Equal(expected, new QueryParameter("p", type, null).DBType);

    [Fact]
    public void DBType_NullTypeName_FallsBackToString() =>
        Assert.Equal(DbType.String, new QueryParameter("p", null, 1).DBType);

    // The type is a SQL type name, not a .NET type name: "Int64" is not a SQL alias, so the
    // parameter silently becomes a string. A caller that knows the DbType up front cannot say so
    // through this constructor.
    [Theory]
    [InlineData("Int16")]
    [InlineData("Int32")]
    [InlineData("Int64")]
    [InlineData("Guid")]
    [InlineData("System.Int64")]
    public void DBType_DotNetTypeName_FallsBackToString(string type) =>
        Assert.Equal(DbType.String, new QueryParameter("p", type, 1).DBType);
}
