using System;
using System.Data;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Defines a provider-specific mapping for one CLR type.
    /// </summary>
    public interface IDapperTypeMapping
    {
        Type TargetType { get; }

        /// <summary>
        /// Returns true only when the concrete parameter belongs to the provider handled by this mapping.
        /// </summary>
        bool CanSetValue(IDbDataParameter parameter);

        /// <summary>
        /// Returns true when this mapping can parse the runtime value shape. Dapper does not provide connection
        /// context while parsing, so parse predicates for the same target type must be mutually exclusive.
        /// </summary>
        bool CanParse(object? value);

        void SetValue(IDbDataParameter parameter, object? value);

        object? Parse(Type destinationType, object? value);
    }
}
