using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.Snowflakes
{
    public class AbpSnowflakesOptionsTest : AbpSnowflakesTestBase
    {
        private readonly AbpSnowflakesOptions _options;
        public AbpSnowflakesOptionsTest()
        {
            _options = GetRequiredService<IOptions<AbpSnowflakesOptions>>().Value;
        }

        [Fact]
        public void AbpSnowflakesOptions_Value_Test()
        {
            var defaultSnowflakeConfiguration = _options.Snowflakes.GetConfiguration(DefaultSnowflake.Name);

            Assert.Equal(3, defaultSnowflakeConfiguration.WorkerId);
            Assert.Equal(3, defaultSnowflakeConfiguration.DatacenterId);
       

            var snowflakeConfiguration1 = _options.Snowflakes.GetConfiguration("snowflake1");
            Assert.Equal(2, snowflakeConfiguration1.WorkerId);
            Assert.Equal(3, snowflakeConfiguration1.DatacenterId);
            Assert.Equal(1420041600000L, snowflakeConfiguration1.Twepoch);
            Assert.Equal(3, snowflakeConfiguration1.WorkerIdBits);
            Assert.Equal(3, snowflakeConfiguration1.DatacenterIdBits);
            Assert.Equal(5, snowflakeConfiguration1.SequenceBits);

            var snowflakeConfiguration2 = _options.Snowflakes.GetConfiguration("snowflake2");
            Assert.Equal(3, snowflakeConfiguration2.WorkerId);
            Assert.Equal(3, snowflakeConfiguration2.DatacenterId);
            Assert.Equal(1430041600000L, snowflakeConfiguration2.Twepoch);
            Assert.Equal(4, snowflakeConfiguration2.WorkerIdBits);
            Assert.Equal(4, snowflakeConfiguration2.DatacenterIdBits);
            Assert.Equal(6, snowflakeConfiguration2.SequenceBits);

        }
    }
}
