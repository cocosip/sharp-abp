using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using SharpAbp.Abp.Dapper.PostgreSql;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    public sealed class PostgreSqlTypeMappingTests
    {
        [Fact]
        public void Guid_Mapping_Should_Use_Uuid_For_Npgsql_Parameters()
        {
            var mapping = new PostgreSqlGuidTypeMapping();
            var parameter = new NpgsqlParameter();
            var value = Guid.Parse("92e11849-7534-4af6-98b2-e828da9d548e");

            mapping.SetValue(parameter, value);

            Assert.True(mapping.CanSetValue(parameter));
            Assert.False(mapping.CanSetValue(new TestDbParameter()));
            Assert.Equal(NpgsqlDbType.Uuid, parameter.NpgsqlDbType);
            Assert.Equal(value, parameter.Value);
        }

        [Fact]
        public void Guid_Mapping_Should_Write_Empty_Guid_As_Null_Uuid()
        {
            var mapping = new PostgreSqlGuidTypeMapping();
            var parameter = new NpgsqlParameter();

            mapping.SetValue(parameter, Guid.Empty);

            Assert.Equal(NpgsqlDbType.Uuid, parameter.NpgsqlDbType);
            Assert.Equal(DBNull.Value, parameter.Value);
        }

        [Fact]
        public void Nullable_Guid_Mapping_Should_Write_Null_As_Null_Uuid()
        {
            IDapperTypeMapping mapping = new PostgreSqlGuidTypeMapping();
            var parameter = new NpgsqlParameter();

            mapping.SetValue(parameter, null);

            Assert.Equal(NpgsqlDbType.Uuid, parameter.NpgsqlDbType);
            Assert.Equal(DBNull.Value, parameter.Value);
        }

        [Fact]
        public void Guid_Mapping_Should_Preserve_PostgreSql_Guid_Values()
        {
            var mapping = new PostgreSqlGuidTypeMapping();
            var value = Guid.Parse("4fa2852a-1d57-4884-8029-d54e09daa823");

            Assert.True(mapping.CanParse(value));
            Assert.False(mapping.CanParse(value.ToByteArray()));
            Assert.Equal(value, mapping.Parse(value));
        }
    }
}
