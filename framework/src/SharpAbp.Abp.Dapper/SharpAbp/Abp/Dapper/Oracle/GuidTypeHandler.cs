using System;
using System.Data;
using Dapper;

namespace SharpAbp.Abp.Dapper.Oracle
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            if (value == null || value is DBNull)
            {
                return Guid.Empty;
            }

            return new Guid((byte[])value);
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {

            if (value == Guid.Empty)
            {
                parameter.Value = DBNull.Value;
                parameter.DbType = DbType.Binary;
                parameter.Size = 16;
            }
            else
            {
                parameter.Value = value.ToByteArray();
                parameter.DbType = DbType.Binary;
                parameter.Size = 16;
            }
        }
    }

    public class NullableGuidTypeHandler : SqlMapper.TypeHandler<Guid?>
    {
        public override Guid? Parse(object value)
        {
            if (value is byte[] bytes && bytes.Length == 16)
            {
                return new Guid(bytes);
            }
            return null;
        }

        public override void SetValue(IDbDataParameter parameter, Guid? value)
        {

            if (value.HasValue)
            {
                parameter.Value = value.Value.ToByteArray();
                parameter.DbType = DbType.Binary;
                parameter.Size = 16;
            }
            else
            {
                parameter.Value = DBNull.Value;
                parameter.DbType = DbType.Binary;
                parameter.Size = 16;
            }
        }
    }
}
