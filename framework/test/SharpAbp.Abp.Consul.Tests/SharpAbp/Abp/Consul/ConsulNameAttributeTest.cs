using SharpAbp.Abp.Consul.TestObjects;
using Xunit;

namespace SharpAbp.Abp.Consul
{
    public class ConsulNameAttributeTest
    {
        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name1 = ConsulNameAttribute
                  .GetConsulName<TestConsul1>();

            var name2 = ConsulNameAttribute
                .GetConsulName<TestConsul2>();

            Assert.Equal("Test1", name1);
            Assert.Equal("Test2", name2);
        }


        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestConsul3).FullName;

            var name = ConsulNameAttribute
                  .GetConsulName<TestConsul3>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var name = ConsulNameAttribute.GetConsulName(typeof(TestConsul1));
            Assert.Equal("Test1", name);
        }

    }
}
