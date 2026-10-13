using EtlKit.Common;
using EtlKit.Primitives;

namespace EtlKit.ConnectionManager
{
    /// <summary>
    /// Engine-specific SQL type conventions (<see cref="TryGetDBSpecificType"/>) and, for backward
    /// compatibility, forwarding members for the driver-independent type mapping that now lives in
    /// <see cref="SqlTypeMapper"/>. See
    /// <c>docs/tech-debt/TECH-DEBT-DataTypeConverter-Driver-Split.md</c> for the planned move of the
    /// engine-specific conventions out of this class as well.
    /// </summary>
    [PublicAPI]
    public static class DataTypeConverter
    {
        /// <summary>
        /// Default display length for a tiny integer column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultTinyIntegerLength instead.")]
        public const int DefaultTinyIntegerLength = SqlTypeMapper.DefaultTinyIntegerLength;

        /// <summary>
        /// Default display length for a small integer column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultSmallIntegerLength instead.")]
        public const int DefaultSmallIntegerLength = SqlTypeMapper.DefaultSmallIntegerLength;

        /// <summary>
        /// Default display length for an integer column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultIntegerLength instead.")]
        public const int DefaultIntegerLength = SqlTypeMapper.DefaultIntegerLength;

        /// <summary>
        /// Default display length for a big integer column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultBigIntegerLength instead.")]
        public const int DefaultBigIntegerLength = SqlTypeMapper.DefaultBigIntegerLength;

        /// <summary>
        /// Default display length for a <c>datetime2</c> column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultDateTime2Length instead.")]
        public const int DefaultDateTime2Length = SqlTypeMapper.DefaultDateTime2Length;

        /// <summary>
        /// Default display length for a <c>datetime</c> column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultDateTimeLength instead.")]
        public const int DefaultDateTimeLength = SqlTypeMapper.DefaultDateTimeLength;

        /// <summary>
        /// Default display length for a decimal column.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultDecimalLength instead.")]
        public const int DefaultDecimalLength = SqlTypeMapper.DefaultDecimalLength;

        /// <summary>
        /// Default length used for character-type columns when none can be determined, and as the
        /// <see cref="GetStringLengthFromCharString"/> fallback.
        /// </summary>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.DefaultStringLength instead.")]
        public const int DefaultStringLength = SqlTypeMapper.DefaultStringLength;

        /// <summary>
        /// Whether <paramref name="value"/> is a <c>char(N)</c>/<c>varchar(N)</c>/<c>nchar(N)</c>-style
        /// type definition with an explicit length.
        /// </summary>
        /// <param name="value">A SQL type definition string.</param>
        /// <remarks>Forwards to <see cref="SqlTypeMapper.IsCharTypeDefinition"/>.</remarks>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.IsCharTypeDefinition instead.")]
        public static bool IsCharTypeDefinition(string value) =>
            SqlTypeMapper.IsCharTypeDefinition(value);

        /// <summary>
        /// Extracts the length from a <c>char(N)</c>-style type definition matched by <see
        /// cref="IsCharTypeDefinition"/>, or <see cref="DefaultStringLength"/> if none can be parsed.
        /// </summary>
        /// <param name="value">A SQL type definition string, e.g. <c>"varchar(50)"</c>.</param>
        /// <remarks>Forwards to <see cref="SqlTypeMapper.GetStringLengthFromCharString"/>.</remarks>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.GetStringLengthFromCharString instead.")]
        public static int GetStringLengthFromCharString(string value) =>
            SqlTypeMapper.GetStringLengthFromCharString(value);

        /// <summary>
        /// Maps a SQL type name (optionally with a length/precision suffix, e.g. <c>"varchar(50)"</c>)
        /// to the fully-qualified name of its .NET equivalent. Unrecognized types map to <c>"System.String"</c>.
        /// </summary>
        /// <param name="dbSpecificTypeName">A SQL type name, e.g. <c>"int"</c>, <c>"datetime2"</c>.</param>
        /// <remarks>Forwards to <see cref="SqlTypeMapper.GetNETObjectTypeString"/>.</remarks>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.GetNETObjectTypeString instead.")]
        public static string GetNETObjectTypeString(string dbSpecificTypeName) =>
            SqlTypeMapper.GetNETObjectTypeString(dbSpecificTypeName);

        /// <summary>
        /// Resolves a SQL type name to its .NET <see cref="Type"/>, via <see cref="GetNETObjectTypeString"/>.
        /// </summary>
        /// <param name="dbSpecificTypeName">A SQL type name, e.g. <c>"int"</c>, <c>"datetime2"</c>.</param>
        /// <remarks>Forwards to <see cref="SqlTypeMapper.GetTypeObject"/>.</remarks>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.GetTypeObject instead.")]
        public static Type GetTypeObject(string dbSpecificTypeName) =>
            SqlTypeMapper.GetTypeObject(dbSpecificTypeName);

        /// <summary>
        /// Resolves a SQL type name to the matching <see cref="System.Data.DbType"/>, via <see
        /// cref="GetNETObjectTypeString"/>. Falls back to <see cref="DbType.String"/> if the mapped
        /// .NET type name has no matching <see cref="System.Data.DbType"/> value.
        /// </summary>
        /// <param name="dbSpecificTypeName">A SQL type name, e.g. <c>"int"</c>, <c>"datetime2"</c>.</param>
        /// <remarks>Forwards to <see cref="SqlTypeMapper.GetDBType"/>.</remarks>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.GetDBType instead.")]
        public static DbType GetDBType(string dbSpecificTypeName) =>
            SqlTypeMapper.GetDBType(dbSpecificTypeName);

        /// <summary>
        /// Adjusts <paramref name="col"/>'s SQL data type for engine-specific quirks (e.g. SQL Server's
        /// <c>TEXT</c> mapping to <c>VARCHAR(MAX)</c>, Access's lack of an <c>INT</c> alias, ClickHouse's
        /// <c>Nullable(...)</c> wrapper). Returns <paramref name="col"/>'s original <see
        /// cref="ITableColumn.DataType"/> unchanged for engines with no special-casing.
        /// </summary>
        /// <param name="col">The column whose type to adjust.</param>
        /// <param name="connectionType">The target database engine.</param>
        public static string TryGetDBSpecificType(
            ITableColumn col,
            ConnectionManagerType connectionType
        )
        {
            var typeName = col.DataType.Trim().ToUpper();
            switch (connectionType)
            {
                case ConnectionManagerType.SqlServer when typeName.Replace(" ", "") == "TEXT":
                    return "VARCHAR(MAX)";
                case ConnectionManagerType.Access when typeName == "INT":
                    return "INTEGER";
                case ConnectionManagerType.Access when SqlTypeMapper.IsCharTypeDefinition(typeName):
                {
                    if (typeName.StartsWith("N"))
                        typeName = typeName.Substring(1);
                    return SqlTypeMapper.GetStringLengthFromCharString(typeName) > 255
                        ? "LONGTEXT"
                        : typeName;
                }
                case ConnectionManagerType.Access:
                    return col.DataType;
                case ConnectionManagerType.SQLite when typeName is "INT" or "BIGINT":
                    return "INTEGER";
                case ConnectionManagerType.SQLite:
                    return col.DataType;
                case ConnectionManagerType.Postgres:
                {
                    return GetPostgreSqlType(typeName, col);
                }
                case ConnectionManagerType.ClickHouse:
                {
                    return GetClickHouseType(typeName, col);
                }
                case ConnectionManagerType.Unknown:
                case ConnectionManagerType.Adomd:
                case ConnectionManagerType.MySql:
                default:
                    return col.DataType;
            }
        }

        /// <summary>
        /// Returns the <see cref="System.DateTimeKind"/> implied by a SQL date/time type name — <see
        /// cref="DateTimeKind.Utc"/> for timezone-aware types, <see cref="DateTimeKind.Unspecified"/>
        /// for the rest, or <see langword="null"/> for non-date/time types.
        /// </summary>
        /// <param name="dbSpecificTypeName">A SQL type name, e.g. <c>"datetime2"</c>, <c>"timestamptz"</c>.</param>
        /// <remarks>Forwards to <see cref="SqlTypeMapper.GetNETDateTimeKind"/>.</remarks>
        [Obsolete("Use EtlKit.Common.SqlTypeMapper.GetNETDateTimeKind instead.")]
        public static DateTimeKind? GetNETDateTimeKind(string dbSpecificTypeName) =>
            SqlTypeMapper.GetNETDateTimeKind(dbSpecificTypeName);

        private static string GetPostgreSqlType(string typeName, ITableColumn col)
        {
            if (SqlTypeMapper.IsCharTypeDefinition(typeName))
            {
                if (typeName.StartsWith("N"))
                    return typeName.Substring(1);
            }
            else if (typeName == "DATETIME")
                return "TIMESTAMP";
            return col.DataType;
        }

        private static string GetClickHouseType(string typeName, ITableColumn col)
        {
            var type = col.DataType;
            if (SqlTypeMapper.IsCharTypeDefinition(typeName))
            {
                type = "String";
            }
            if (col.AllowNulls && !type.StartsWith("Nullable"))
            {
                return $"Nullable({type})";
            }
            return type;
        }
    }
}
