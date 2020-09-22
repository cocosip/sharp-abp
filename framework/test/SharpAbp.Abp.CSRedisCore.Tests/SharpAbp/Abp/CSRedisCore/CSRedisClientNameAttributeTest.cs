using SharpAbp.Abp.CSRedisCore.TestObjects;
using Xunit;

namespace SharpAbp.Abp.CSRedisCore
{
    public class CSRedisClientNameAttributeTest
    {
        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name = CSRedisClientNameAttribute
                  .GetClientName<TestClient2>();

            Assert.Equal("Test2", name);
        }

        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestClient1).FullName;

            var name = CSRedisClientNameAttribute
                  .GetClientName<TestClient1>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var expected = typeof(TestClient1).FullName;
            var name = CSRedisClientNameAttribute.GetClientName(typeof(TestClient1));
            Assert.Equal(expected, name);
        }
    }
}
