using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace SharpAbp.Abp.Dapper.PostgreSql
{
    public sealed class PostgreSqlGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => parameter is NpgsqlParameter;

        public override bool CanParse(object? value) => value is Guid;

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            var npgsqlParameter = (NpgsqlParameter)parameter;
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Uuid;
            npgsqlParameter.Value = value == Guid.Empty ? DBNull.Value : value;
        }

        public override void SetNullValue(IDbDataParameter parameter)
        {
            var npgsqlParameter = (NpgsqlParameter)parameter;
            npgsqlParameter.NpgsqlDbType = NpgsqlDbType.Uuid;
            npgsqlParameter.Value = DBNull.Value;
        }

        public override Guid Parse(object value) => (Guid)value;
    }
}
