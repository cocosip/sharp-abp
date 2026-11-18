using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SharpAbp.Abp.Dapper.Oracle
{
    /// <summary>
    /// Writes a <see cref="Guid"/> as <c>RAW(16)</c> for Oracle parameters. Reading is handled by
    /// <see cref="BinaryGuidTypeMapping"/>, which every binary-16 provider shares, because Dapper gives a
    /// type handler no connection context while parsing.
    /// </summary>
    public sealed class OracleGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => parameter is OracleParameter;

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
                $"'{nameof(OracleGuidTypeMapping)}' only writes parameters; " +
                $"'{nameof(BinaryGuidTypeMapping)}' parses them.");
        }
    }
}
