#nullable enable
using System.Data;
using Xunit;

namespace EtlKit.Common.Tests;

/// <summary>
/// Runs the <see cref="SqlTypeMappingContractTests"/> expectations against <see
/// cref="SqlTypeMapper"/>, the home of the driver-independent type mapping.
/// </summary>
public sealed class SqlTypeMapperTests : SqlTypeMappingContractTests
{
    protected override string GetNETObjectTypeString(string? sqlType) =>
        SqlTypeMapper.GetNETObjectTypeString(sqlType!);

    protected override Type? GetTypeObject(string sqlType) => SqlTypeMapper.GetTypeObject(sqlType);

    protected override DbType GetDBType(string? sqlType) => SqlTypeMapper.GetDBType(sqlType!);

    protected override bool IsCharTypeDefinition(string definition) =>
        SqlTypeMapper.IsCharTypeDefinition(definition);

    protected override int GetStringLengthFromCharString(string definition) =>
        SqlTypeMapper.GetStringLengthFromCharString(definition);

    protected override DateTimeKind? GetNETDateTimeKind(string? sqlType) =>
        SqlTypeMapper.GetNETDateTimeKind(sqlType!);

    protected override int[] DefaultLengths =>
        [
            SqlTypeMapper.DefaultTinyIntegerLength,
            SqlTypeMapper.DefaultSmallIntegerLength,
            SqlTypeMapper.DefaultIntegerLength,
            SqlTypeMapper.DefaultBigIntegerLength,
            SqlTypeMapper.DefaultDateTime2Length,
            SqlTypeMapper.DefaultDateTimeLength,
            SqlTypeMapper.DefaultDecimalLength,
            SqlTypeMapper.DefaultStringLength,
        ];

    protected override int DefaultStringLength => SqlTypeMapper.DefaultStringLength;

    [Fact]
    public void FullName_IsStable() =>
        Assert.Equal("EtlKit.Common.SqlTypeMapper", typeof(SqlTypeMapper).FullName);
}
