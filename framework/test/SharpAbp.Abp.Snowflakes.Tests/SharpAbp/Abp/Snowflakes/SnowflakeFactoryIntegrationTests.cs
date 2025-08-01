using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.Snowflakes
{
    public class SnowflakeFactoryIntegrationTests : AbpSnowflakesTestBase
    {
        private readonly ISnowflakeFactory _snowflakeFactory;

        public SnowflakeFactoryIntegrationTests()
        {
            _snowflakeFactory = GetRequiredService<ISnowflakeFactory>();
        }

        [Fact]
        public void Should_Generate_Unique_Ids_From_Default_Factory_Instance()
        {
            var defaultSnowflake = _snowflakeFactory.GetDefault();
            var ids = new HashSet<long>();
            for (int i = 0; i < 1000; i++)
            {
                Assert.True(ids.Add(defaultSnowflake.NextId()));
            }
        }

        [Fact]
        public void Should_Generate_Unique_Ids_From_Named_Factory_Instance()
        {
            var namedSnowflake = _snowflakeFactory.Get("test_instance");
            var ids = new HashSet<long>();
            for (int i = 0; i < 1000; i++)
            {
                Assert.True(ids.Add(namedSnowflake.NextId()));
            }
        }

        [Fact]
        public void Should_Return_Same_Instance_For_Same_Name()
        {
            var instance1 = _snowflakeFactory.Get("shared_instance");
            var instance2 = _snowflakeFactory.Get("shared_instance");

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Should_Return_Different_Instances_For_Different_Names()
        {
            var instance1 = _snowflakeFactory.Get("instance_a");
            var instance2 = _snowflakeFactory.Get("instance_b");

            Assert.NotSame(instance1, instance2);
        }

        [Fact]
        public void Should_Generate_Increasing_Ids_From_Factory_Instance()
        {
            var snowflake = _snowflakeFactory.GetDefault();
            long lastId = 0;
            for (int i = 0; i < 1000; i++)
            {
                long currentId = snowflake.NextId();
                Assert.True(currentId > lastId);
                lastId = currentId;
            }
        }
    }
}
