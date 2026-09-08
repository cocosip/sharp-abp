using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Dapper;

namespace SharpAbp.Abp.Dapper
{
    internal sealed class DispatchingTypeHandler : SqlMapper.ITypeHandler
    {
        private readonly Type _targetType;
        private readonly IReadOnlyList<IDapperTypeMapping> _mappings;
        private readonly DbType? _defaultDbType;
        private readonly SqlMapper.ITypeHandler? _fallbackHandler;

        public DispatchingTypeHandler(
            Type targetType,
            IReadOnlyList<IDapperTypeMapping> mappings,
            DbType? defaultDbType,
            SqlMapper.ITypeHandler? fallbackHandler)
        {
            _targetType = targetType;
            _mappings = mappings;
            _defaultDbType = defaultDbType;
            _fallbackHandler = fallbackHandler;
        }

        public DbType? DefaultDbType => _defaultDbType;

        public SqlMapper.ITypeHandler? FallbackHandler => _fallbackHandler;

        public void SetValue(IDbDataParameter parameter, object? value)
        {
            var mapping = Resolve(mapping => mapping.CanSetValue(parameter), "parameter", parameter.GetType());
            if (mapping != null)
            {
                mapping.SetValue(parameter, value);
                return;
            }

            if (_fallbackHandler != null)
            {
                _fallbackHandler.SetValue(parameter, value!);
                return;
            }

            if (_defaultDbType.HasValue && (int)_defaultDbType.Value >= 0)
            {
                parameter.DbType = _defaultDbType.Value;
            }

            parameter.Value = SanitizeParameterValue(value);
        }

        public object? Parse(Type destinationType, object value)
        {
            var mapping = Resolve(
                candidate => candidate.CanParse(value),
                "value",
                value?.GetType() ?? typeof(DBNull));

            if (mapping != null)
            {
                return mapping.Parse(destinationType, value);
            }

            if (_fallbackHandler != null)
            {
                return _fallbackHandler.Parse(destinationType, value!);
            }

            return ParseDefault(destinationType, value);
        }

        private IDapperTypeMapping? Resolve(
            Func<IDapperTypeMapping, bool> predicate,
            string sourceKind,
            Type sourceType)
        {
            IDapperTypeMapping? match = null;
            foreach (var mapping in _mappings)
            {
                if (!predicate(mapping))
                {
                    continue;
                }

                if (match != null)
                {
                    throw new InvalidOperationException(
                        $"Multiple Dapper type mappings for '{_targetType.FullName}' can handle {sourceKind} " +
                        $"type '{sourceType.FullName}'. Mapping predicates must be mutually exclusive.");
                }

                match = mapping;
            }

            return match;
        }

        private static object? ParseDefault(Type destinationType, object? value)
        {
            if (value == null || value is DBNull)
            {
                return Nullable.GetUnderlyingType(destinationType) != null || !destinationType.IsValueType
                    ? null
                    : Activator.CreateInstance(destinationType);
            }

            var conversionType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            if (conversionType.IsInstanceOfType(value))
            {
                return value;
            }

            if (conversionType.IsEnum)
            {
                return value is string text
                    ? Enum.Parse(conversionType, text, true)
                    : Enum.ToObject(conversionType, value);
            }

            var conversionOperator = GetOperator(value.GetType(), conversionType);
            if (conversionOperator != null)
            {
                return conversionOperator.Invoke(null, new[] { value });
            }

            return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        private static MethodInfo? GetOperator(Type sourceType, Type destinationType)
        {
            var sourceMethods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var destinationMethods = destinationType.GetMethods(BindingFlags.Public | BindingFlags.Static);

            return FindOperator(sourceMethods, sourceType, destinationType, "op_Implicit") ??
                   FindOperator(destinationMethods, sourceType, destinationType, "op_Implicit") ??
                   FindOperator(sourceMethods, sourceType, destinationType, "op_Explicit") ??
                   FindOperator(destinationMethods, sourceType, destinationType, "op_Explicit");
        }

        private static MethodInfo? FindOperator(
            IEnumerable<MethodInfo> methods,
            Type sourceType,
            Type destinationType,
            string name)
        {
            return methods.FirstOrDefault(method =>
            {
                if (method.Name != name || method.ReturnType != destinationType)
                {
                    return false;
                }

                var parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == sourceType;
            });
        }

        private static object SanitizeParameterValue(object? value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value is Enum
                ? Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), CultureInfo.InvariantCulture)
                : value;
        }
    }
}
