using SharpAbp.Abp.FreeRedis.TestObjects;
using Xunit;

namespace SharpAbp.Abp.FreeRedis
{
    public class RedisClientNameAttributeTest
    {
        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name = RedisClientNameAttribute
                  .GetClientName<TestClient2>();

            Assert.Equal("Test2", name);
        }

        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestClient1).FullName;

            var name = RedisClientNameAttribute
                  .GetClientName<TestClient1>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var expected = typeof(TestClient1).FullName;
            var name = RedisClientNameAttribute
                .GetClientName(typeof(TestClient1));
            Assert.Equal(expected, name);
        }
    }
}
