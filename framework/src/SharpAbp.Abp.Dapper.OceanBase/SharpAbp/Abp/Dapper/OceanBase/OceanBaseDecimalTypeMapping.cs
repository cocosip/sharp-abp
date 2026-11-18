using System;
using System.Data;
using OceanBaseConnector;

namespace SharpAbp.Abp.Dapper.OceanBase
{
    /// <summary>
    /// Reads <see cref="OceanBaseDecimal"/> values into <see cref="decimal"/>. The driver returns that type for
    /// Oracle <c>NUMBER</c> columns without a declared precision, for precision above 28, for Oracle <c>FLOAT</c>
    /// and for MySQL <c>DECIMAL</c> above precision 28. It is a plain struct with no conversion operator and no
    /// <see cref="IConvertible"/>, so Dapper's default conversion cannot turn it into a <see cref="decimal"/>.
    /// </summary>
    public sealed class OceanBaseDecimalTypeMapping : DapperTypeMapping<decimal>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => false;

        public override bool CanParse(object? value) => value is OceanBaseDecimal;

        public override void SetValue(IDbDataParameter parameter, decimal value)
        {
            throw new NotSupportedException(
                $"'{nameof(OceanBaseDecimalTypeMapping)}' only parses values; the driver binds " +
                $"{nameof(DbType)}.{nameof(DbType.Decimal)} parameters by itself.");
        }

        public override decimal Parse(object value) => ((OceanBaseDecimal)value).ToDecimal();
    }
}
