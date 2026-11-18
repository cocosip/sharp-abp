using System;
using System.Data;
using OceanBaseConnector;

namespace SharpAbp.Abp.Dapper.OceanBase
{
    /// <summary>
    /// Reads Oracle-mode <c>TIMESTAMP</c> columns into <see cref="DateTime"/>. The driver returns
    /// <see cref="OceanBaseTimestamp"/> above scale 7 and <see cref="OceanBaseTimestampLocalTimeZone"/> for
    /// <c>TIMESTAMP WITH LOCAL TIME ZONE</c>; both are structs that expose only <c>ToDateTime()</c>, so Dapper's
    /// default conversion cannot turn them into a <see cref="DateTime"/>.
    /// </summary>
    public sealed class OceanBaseTimestampTypeMapping : DapperTypeMapping<DateTime>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => false;

        public override bool CanParse(object? value) =>
            value is OceanBaseTimestamp or OceanBaseTimestampLocalTimeZone;

        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            throw new NotSupportedException(
                $"'{nameof(OceanBaseTimestampTypeMapping)}' only parses values; the driver binds " +
                $"{nameof(DbType)}.{nameof(DbType.DateTime)} parameters by itself.");
        }

        public override DateTime Parse(object value)
        {
            return value switch
            {
                OceanBaseTimestamp timestamp => timestamp.ToDateTime(),
                OceanBaseTimestampLocalTimeZone localTimeZone => localTimeZone.ToDateTime(),
                _ => throw new NotSupportedException(
                    $"'{nameof(OceanBaseTimestampTypeMapping)}' cannot read '{value.GetType().FullName}'.")
            };
        }
    }
}
