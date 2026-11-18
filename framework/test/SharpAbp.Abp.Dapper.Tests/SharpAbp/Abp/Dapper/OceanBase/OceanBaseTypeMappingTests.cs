using System;
using System.Data;
using OceanBaseConnector;
using SharpAbp.Abp.Dapper.OceanBase;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    public sealed class OceanBaseTypeMappingTests
    {
        private static readonly Guid Value = Guid.Parse("1a9b9a6b-cd16-4b5a-98b2-2328ef4aecbf");

        [Fact]
        public void Oracle_Mode_Should_Write_Raw_16()
        {
            var mapping = new OceanBaseOracleGuidTypeMapping();
            var parameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.Oracle);

            mapping.SetValue(parameter, Value);

            Assert.Equal(DbType.Binary, parameter.DbType);
            Assert.Equal(16, parameter.Size);
            Assert.Equal(Value.ToByteArray(), parameter.Value);
        }

        [Fact]
        public void MySql_Mode_Should_Write_Char_36()
        {
            var mapping = new OceanBaseMySqlGuidTypeMapping();
            var parameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.MySql);

            mapping.SetValue(parameter, Value);

            Assert.Equal(DbType.String, parameter.DbType);
            Assert.Equal(36, parameter.Size);
            Assert.Equal(Value.ToString("D"), parameter.Value);
        }

        [Fact]
        public void Mappings_Should_Select_By_Parameter_Compatibility_Mode()
        {
            var oracleMapping = new OceanBaseOracleGuidTypeMapping();
            var mySqlMapping = new OceanBaseMySqlGuidTypeMapping();
            var oracleParameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.Oracle);
            var mySqlParameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.MySql);
            var unopenedParameter = new OceanBaseParameter();

            Assert.True(oracleMapping.CanSetValue(oracleParameter));
            Assert.False(oracleMapping.CanSetValue(mySqlParameter));

            Assert.True(mySqlMapping.CanSetValue(mySqlParameter));
            Assert.False(mySqlMapping.CanSetValue(oracleParameter));

            Assert.False(oracleMapping.CanSetValue(unopenedParameter));
            Assert.False(mySqlMapping.CanSetValue(unopenedParameter));

            Assert.False(oracleMapping.CanSetValue(new TestDbParameter()));
            Assert.False(mySqlMapping.CanSetValue(new TestDbParameter()));
        }

        [Fact]
        public void Mappings_Should_Write_Null_Values()
        {
            IDapperTypeMapping oracleMapping = new OceanBaseOracleGuidTypeMapping();
            IDapperTypeMapping mySqlMapping = new OceanBaseMySqlGuidTypeMapping();
            var oracleParameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.Oracle);
            var mySqlParameter = OceanBaseTestParameters.Create(OceanBaseCompatibilityMode.MySql);

            oracleMapping.SetValue(oracleParameter, null);
            mySqlMapping.SetValue(mySqlParameter, null);

            Assert.Equal(DbType.Binary, oracleParameter.DbType);
            Assert.Equal(16, oracleParameter.Size);
            Assert.Equal(DBNull.Value, oracleParameter.Value);

            Assert.Equal(DbType.String, mySqlParameter.DbType);
            Assert.Equal(36, mySqlParameter.Size);
            Assert.Equal(DBNull.Value, mySqlParameter.Value);
        }

        [Fact]
        public void Mappings_Should_Leave_Reading_To_The_Shared_Shape_Mappings()
        {
            Assert.False(new OceanBaseOracleGuidTypeMapping().CanParse(Value.ToByteArray()));
            Assert.False(new OceanBaseMySqlGuidTypeMapping().CanParse(Value.ToString("D")));

            Assert.True(new BinaryGuidTypeMapping().CanParse(Value.ToByteArray()));
            Assert.True(new StringGuidTypeMapping().CanParse(Value.ToString("D")));
            Assert.Equal(Value, new StringGuidTypeMapping().Parse(Value.ToString("D")));
        }
    }
}
