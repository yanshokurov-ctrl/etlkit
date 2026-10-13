#nullable enable
using System.Data;
using EtlKit.ConnectionManager;
using Xunit;

namespace EtlKit.Common.Tests;

/// <summary>
/// Runs the <see cref="SqlTypeMappingContractTests"/> expectations against the obsolete
/// forwarding members of <see cref="DataTypeConverter"/>, which keep existing callers working
/// while the mapping lives in <see cref="SqlTypeMapper"/>. The obsolete members are the subject
/// here on purpose. Delete this class together with the members once they are removed.
/// </summary>
#pragma warning disable CS0618
public sealed class DataTypeConverterForwardingTests : SqlTypeMappingContractTests
{
    protected override string GetNETObjectTypeString(string? sqlType) =>
        DataTypeConverter.GetNETObjectTypeString(sqlType!);

    protected override Type? GetTypeObject(string sqlType) =>
        DataTypeConverter.GetTypeObject(sqlType);

    protected override DbType GetDBType(string? sqlType) => DataTypeConverter.GetDBType(sqlType!);

    protected override bool IsCharTypeDefinition(string definition) =>
        DataTypeConverter.IsCharTypeDefinition(definition);

    protected override int GetStringLengthFromCharString(string definition) =>
        DataTypeConverter.GetStringLengthFromCharString(definition);

    protected override DateTimeKind? GetNETDateTimeKind(string? sqlType) =>
        DataTypeConverter.GetNETDateTimeKind(sqlType!);

    protected override int[] DefaultLengths =>
        [
            DataTypeConverter.DefaultTinyIntegerLength,
            DataTypeConverter.DefaultSmallIntegerLength,
            DataTypeConverter.DefaultIntegerLength,
            DataTypeConverter.DefaultBigIntegerLength,
            DataTypeConverter.DefaultDateTime2Length,
            DataTypeConverter.DefaultDateTimeLength,
            DataTypeConverter.DefaultDecimalLength,
            DataTypeConverter.DefaultStringLength,
        ];

    protected override int DefaultStringLength => DataTypeConverter.DefaultStringLength;

    [Fact]
    public void FullName_IsStable() =>
        Assert.Equal(
            "EtlKit.ConnectionManager.DataTypeConverter",
            typeof(DataTypeConverter).FullName
        );
}
#pragma warning restore CS0618
