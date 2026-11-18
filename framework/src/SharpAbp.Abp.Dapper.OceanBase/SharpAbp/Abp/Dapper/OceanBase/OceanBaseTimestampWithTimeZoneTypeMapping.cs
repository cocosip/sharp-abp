using System;
using System.Data;
using OceanBaseConnector;

namespace SharpAbp.Abp.Dapper.OceanBase
{
    /// <summary>
    /// Reads Oracle-mode <c>TIMESTAMP WITH TIME ZONE</c> columns into <see cref="DateTimeOffset"/>. The driver
    /// returns <see cref="OceanBaseTimestampWithTimeZone"/>, a struct that exposes only <c>ToDateTimeOffset()</c>,
    /// so Dapper's default conversion cannot turn it into a <see cref="DateTimeOffset"/>.
    /// </summary>
    public sealed class OceanBaseTimestampWithTimeZoneTypeMapping : DapperTypeMapping<DateTimeOffset>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => false;

        public override bool CanParse(object? value) => value is OceanBaseTimestampWithTimeZone;

        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            throw new NotSupportedException(
                $"'{nameof(OceanBaseTimestampWithTimeZoneTypeMapping)}' only parses values; the driver binds " +
                $"{nameof(DbType)}.{nameof(DbType.DateTimeOffset)} parameters by itself.");
        }

        public override DateTimeOffset Parse(object value) =>
            ((OceanBaseTimestampWithTimeZone)value).ToDateTimeOffset();
    }
}
