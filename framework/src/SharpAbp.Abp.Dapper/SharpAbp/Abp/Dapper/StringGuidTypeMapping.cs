using System;
using System.Data;
using System.Globalization;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Reads a <see cref="Guid"/> from its textual form. Dapper gives a type handler no connection context
    /// while parsing, so a value shape is claimed once for the whole process: MySQL and OceanBase MySQL-mode
    /// <c>CHAR(36)</c> columns, and any other text column that stores a GUID, share this reader.
    /// Writing is delegated to the provider mapping.
    /// </summary>
    public sealed class StringGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => false;

        public override bool CanParse(object? value) => value is string;

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            throw new NotSupportedException(
                $"'{nameof(StringGuidTypeMapping)}' only parses textual GUID values; " +
                "the provider mapping writes them.");
        }

        public override Guid Parse(object value) =>
            Guid.Parse(((string)value).Trim(), CultureInfo.InvariantCulture);
    }
}
