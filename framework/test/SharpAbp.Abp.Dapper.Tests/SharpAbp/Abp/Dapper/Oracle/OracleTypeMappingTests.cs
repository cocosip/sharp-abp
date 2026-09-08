using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using SharpAbp.Abp.Dapper.Oracle;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    public sealed class OracleTypeMappingTests
    {
        [Fact]
        public void Guid_Mapping_Should_Use_Raw_16_For_Oracle_Parameters()
        {
            var mapping = new OracleGuidTypeMapping();
            var parameter = new OracleParameter();
            var value = Guid.Parse("af4cce1b-bf35-4cfd-bd87-a4189f98f12f");

            mapping.SetValue(parameter, value);

            Assert.True(mapping.CanSetValue(parameter));
            Assert.False(mapping.CanSetValue(new TestDbParameter()));
            Assert.Equal(DbType.Binary, parameter.DbType);
            Assert.Equal(16, parameter.Size);
            Assert.Equal(value.ToByteArray(), parameter.Value);
            Assert.Equal(value, mapping.Parse(value.ToByteArray()));
        }

        [Fact]
        public void Guid_Mapping_Should_Configure_Null_As_Raw_16()
        {
            IDapperTypeMapping mapping = new OracleGuidTypeMapping();
            var parameter = new OracleParameter();

            mapping.SetValue(parameter, null);

            Assert.Equal(DbType.Binary, parameter.DbType);
            Assert.Equal(16, parameter.Size);
            Assert.Equal(DBNull.Value, parameter.Value);
        }

        [Fact]
        public void Guid_Mapping_Should_Write_Empty_Guid_As_Raw_16()
        {
            var mapping = new OracleGuidTypeMapping();
            var parameter = new OracleParameter();

            mapping.SetValue(parameter, Guid.Empty);

            Assert.Equal(DbType.Binary, parameter.DbType);
            Assert.Equal(16, parameter.Size);
            Assert.Equal(Guid.Empty.ToByteArray(), parameter.Value);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        [InlineData("1", true)]
        [InlineData("0", false)]
        public void Bool_Mapping_Should_Parse_Oracle_Number_Values(object value, bool expected)
        {
            var mapping = new OracleBoolTypeMapping();

            Assert.Equal(expected, mapping.Parse(value));
        }

        [Theory]
        [InlineData((byte)1)]
        [InlineData((short)1)]
        [InlineData(1)]
        [InlineData(1L)]
        [InlineData("1")]
        public void Bool_Mapping_Should_Not_Claim_Provider_Agnostic_Parse_Values(object value)
        {
            var mapping = new OracleBoolTypeMapping();

            Assert.False(mapping.CanParse(value));
        }

        [Fact]
        public void Bool_Mapping_Should_Write_Oracle_Number_Values()
        {
            var mapping = new OracleBoolTypeMapping();
            var parameter = new OracleParameter();

            mapping.SetValue(parameter, true);

            Assert.True(mapping.CanSetValue(parameter));
            Assert.False(mapping.CanSetValue(new TestDbParameter()));
            Assert.Equal((byte)1, parameter.Value);
        }

        [Fact]
        public void Nullable_Bool_Mapping_Should_Write_Null_As_Zero()
        {
            IDapperTypeMapping mapping = new OracleBoolTypeMapping();
            var parameter = new OracleParameter();

            mapping.SetValue(parameter, null);

            Assert.Equal((byte)0, parameter.Value);
        }
    }
}
