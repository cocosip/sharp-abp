using System.Data;
using Volo.Abp.Data;
using Xunit;

namespace SharpAbp.Abp.Dapper
{
    public sealed class ExtraPropertyDictionaryTypeMappingTests
    {
        [Fact]
        public void Extra_Properties_Mapping_Should_Participate_In_The_Mapping_Pipeline()
        {
            Assert.IsAssignableFrom<IDapperTypeMapping>(new ExtraPropertyDictionaryTypeMapping());

            var mapping = (IDapperTypeMapping)new ExtraPropertyDictionaryTypeMapping();
            var parameter = new TestDbParameter();
            mapping.SetValue(parameter, new ExtraPropertyDictionary
            {
                ["Code"] = "A001"
            });

            Assert.Equal(DbType.String, parameter.DbType);
            Assert.Contains("A001", Assert.IsType<string>(parameter.Value));
        }
    }
}
