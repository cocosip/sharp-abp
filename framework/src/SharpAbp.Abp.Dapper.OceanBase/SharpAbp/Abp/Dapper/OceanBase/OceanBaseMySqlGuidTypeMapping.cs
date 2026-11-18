using System;
using System.Data;
using System.Globalization;
using OceanBaseConnector;

namespace SharpAbp.Abp.Dapper.OceanBase
{
    /// <summary>
    /// Writes a <see cref="Guid"/> as <c>CHAR(36)</c> for MySQL-compatible OceanBase tenants, matching the
    /// default GUID format of MySQL and Pomelo. Reading is handled by <see cref="StringGuidTypeMapping"/>.
    /// </summary>
    public sealed class OceanBaseMySqlGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public const int ColumnSize = 36;

        public override bool CanSetValue(IDbDataParameter parameter) =>
            parameter is OceanBaseParameter { CompatibilityMode: OceanBaseCompatibilityMode.MySql };

        public override bool CanParse(object? value) => false;

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.DbType = DbType.String;
            parameter.Size = ColumnSize;
            parameter.Value = value.ToString("D", CultureInfo.InvariantCulture);
        }

        public override void SetNullValue(IDbDataParameter parameter)
        {
            parameter.DbType = DbType.String;
            parameter.Size = ColumnSize;
            parameter.Value = DBNull.Value;
        }

        public override Guid Parse(object value)
        {
            throw new NotSupportedException(
                $"'{nameof(OceanBaseMySqlGuidTypeMapping)}' only writes parameters; " +
                $"'{nameof(StringGuidTypeMapping)}' parses them.");
        }
    }
}
