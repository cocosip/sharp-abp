using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace SharpAbp.Abp.Dapper.PostgreSql
{
    /// <summary>
    /// Writes a <see cref="Guid"/> as a PostgreSQL <c>uuid</c>, mapping <see cref="Guid.Empty"/> to <c>NULL</c>.
    /// Reading needs no mapping: Npgsql returns a <see cref="Guid"/>, which Dapper converts by itself.
    /// </summary>
    public sealed class PostgreSqlGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => parameter is NpgsqlParameter;

        public override bool CanParse(object? value) => false;

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

        public override Guid Parse(object value)
        {
            throw new NotSupportedException(
                $"'{nameof(PostgreSqlGuidTypeMapping)}' only writes parameters; Dapper reads " +
                $"'{nameof(Guid)}' values without a mapping.");
        }
    }
}
