using System;
using System.Data;
using OceanBaseConnector;

namespace SharpAbp.Abp.Dapper.OceanBase
{
    /// <summary>
    /// Writes a <see cref="Guid"/> as <c>RAW(16)</c> for Oracle-compatible OceanBase tenants. The driver has no
    /// native GUID binding and binds <see cref="DbType.Binary"/> as Oracle <c>RAW</c>. Reading is handled by
    /// <see cref="BinaryGuidTypeMapping"/>, which every binary-16 provider shares.
    /// </summary>
    public sealed class OceanBaseOracleGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) =>
            parameter is OceanBaseParameter { CompatibilityMode: OceanBaseCompatibilityMode.Oracle };

        public override bool CanParse(object? value) => false;

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.DbType = DbType.Binary;
            parameter.Size = 16;
            parameter.Value = value.ToByteArray();
        }

        public override void SetNullValue(IDbDataParameter parameter)
        {
            parameter.DbType = DbType.Binary;
            parameter.Size = 16;
            parameter.Value = DBNull.Value;
        }

        public override Guid Parse(object value)
        {
            throw new NotSupportedException(
                $"'{nameof(OceanBaseOracleGuidTypeMapping)}' only writes parameters; " +
                $"'{nameof(BinaryGuidTypeMapping)}' parses them.");
        }
    }
}
