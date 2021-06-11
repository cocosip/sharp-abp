using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Snowflakes
{
    public class SnowflakeTest : AbpSnowflakesTestBase
    {
        private readonly ISnowflakeFactory _snowflakeFactory;
        public SnowflakeTest()
        {
            _snowflakeFactory = GetRequiredService<ISnowflakeFactory>();
        }

        [Fact]
        public void Snowflake_Id_Test()
        {
            var snowflake1 = _snowflakeFactory.Get("snowflake1");
            var id1_1 = snowflake1.NextId();
            var id1_2 = snowflake1.NextId();
            Assert.True(id1_1 != id1_2);

            var snowflake2 = _snowflakeFactory.Get("snowflake2");
            var id2_1 = snowflake2.NextId();
            var id2_2 = snowflake2.NextId();
            Assert.True(id2_1 != id2_2);
        }

    }
}
