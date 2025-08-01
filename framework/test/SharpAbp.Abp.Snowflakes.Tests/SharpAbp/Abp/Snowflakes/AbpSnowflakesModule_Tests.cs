using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.Snowflakes
{
    public class AbpSnowflakesModule_Tests : AbpSnowflakesTestBase
    {
        private readonly AbpSnowflakesOptions _options;

        public AbpSnowflakesModule_Tests()
        {
            _options = GetRequiredService<IOptions<AbpSnowflakesOptions>>().Value;
        }

        [Fact]
        public void Should_Configure_Default_Snowflake_Options()
        {
            var defaultSnowflakeConfig = _options.Snowflakes.GetConfiguration(DefaultSnowflake.Name);

            Assert.NotNull(defaultSnowflakeConfig);
            Assert.Equal(1L, defaultSnowflakeConfig.WorkerId);
            Assert.Equal(1L, defaultSnowflakeConfig.DatacenterId);
        }
    }
}
