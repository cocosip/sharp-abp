using System;
using System.Data;

namespace SharpAbp.Abp.Dapper
{
    /// <summary>
    /// Reads a <see cref="Guid"/> from its 16 binary bytes. Dapper gives a type handler no connection context
    /// while parsing, so a value shape is claimed once for the whole process: Oracle <c>RAW(16)</c>,
    /// MySQL <c>BINARY(16)</c>, OceanBase Oracle-mode <c>RAW(16)</c> and similar binary columns all share
    /// this reader. Writing is delegated to the provider mapping.
    /// </summary>
    public sealed class BinaryGuidTypeMapping : DapperTypeMapping<Guid>
    {
        public override bool CanSetValue(IDbDataParameter parameter) => false;

        public override bool CanParse(object? value) => value is byte[] { Length: 16 };

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            throw new NotSupportedException(
                $"'{nameof(BinaryGuidTypeMapping)}' only parses binary GUID values; " +
                "the provider mapping writes them.");
        }

        public override Guid Parse(object value) => new((byte[])value);
    }
}
