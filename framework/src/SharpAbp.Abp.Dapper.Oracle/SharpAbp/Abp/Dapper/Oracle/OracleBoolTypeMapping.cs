using System;
using System.Data;
using System.Globalization;
using Oracle.ManagedDataAccess.Client;

namespace SharpAbp.Abp.Dapper.Oracle
{
    public sealed class OracleBoolTypeMapping : DapperTypeMapping<bool>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => parameter is OracleParameter;

        public override bool CanParse(object? value) => false;

        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.Value = value ? (byte)1 : (byte)0;
        }

        public override void SetNullValue(IDbDataParameter parameter)
        {
            parameter.Value = (byte)0;
        }

        public override bool Parse(object value)
        {
            return Convert.ToDecimal(value, CultureInfo.InvariantCulture) != decimal.Zero;
        }
    }
}
