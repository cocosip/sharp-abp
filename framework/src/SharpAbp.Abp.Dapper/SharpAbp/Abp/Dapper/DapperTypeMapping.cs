using System;
using System.Data;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Base class for a Dapper type mapping.
    /// </summary>
    public abstract class DapperTypeMapping<T> : IDapperTypeMapping
    {
        public Type TargetType => typeof(T);

        public abstract bool CanSetValue(IDbDataParameter parameter);

        public abstract bool CanParse(object? value);

        public abstract void SetValue(IDbDataParameter parameter, T value);

        public virtual void SetNullValue(IDbDataParameter parameter)
        {
            parameter.Value = DBNull.Value;
        }

        public abstract T Parse(object value);

        public virtual object? ParseNull(Type destinationType)
        {
            return Nullable.GetUnderlyingType(destinationType) != null || !destinationType.IsValueType
                ? null
                : default(T);
        }

        void IDapperTypeMapping.SetValue(IDbDataParameter parameter, object? value)
        {
            if (value == null || value is DBNull)
            {
                SetNullValue(parameter);
                return;
            }

            if (value is not T typedValue)
            {
                throw new InvalidOperationException(
                    $"Dapper type mapping '{GetType().FullName}' expected a value of type '{typeof(T).FullName}', " +
                    $"but received '{value.GetType().FullName}'.");
            }

            SetValue(parameter, typedValue);
        }

        object? IDapperTypeMapping.Parse(Type destinationType, object? value)
        {
            if (value == null || value is DBNull)
            {
                return ParseNull(destinationType);
            }

            return Parse(value);
        }
    }
}
