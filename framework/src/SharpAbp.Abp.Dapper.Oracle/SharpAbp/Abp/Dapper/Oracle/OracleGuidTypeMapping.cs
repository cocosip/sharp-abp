using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SharpAbp.Abp.Dapper.Oracle
{
    public sealed class OracleGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => parameter is OracleParameter;

        public override bool CanParse(object? value) => value is byte[] { Length: 16 };

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

        public override Guid Parse(object value) => new((byte[])value);
    }
}
