using System.Data;
using Dapper;

namespace SharpAbp.Abp.Dapper.Oracle
{
    public class BoolTypeHandler : SqlMapper.TypeHandler<bool>
    {
        public override bool Parse(object value)
        {
            if (value == null)
            {
                return false;
            }
            return bool.Parse((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.Value = value ? (byte)1 : (byte)0;
        }
    }

    public class NullableBoolTypeHandler : SqlMapper.TypeHandler<bool?>
    {
        public override bool? Parse(object value)
        {
            if (value == null)
            {
                return null;
            }
            return bool.Parse((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, bool? value)
        {
            if (!value.HasValue)
            {
                parameter.Value = (byte)0;
            }
            else
            {
                parameter.Value = value.Value ? (byte)1 : (byte)0;
            }
        }
    }
}
