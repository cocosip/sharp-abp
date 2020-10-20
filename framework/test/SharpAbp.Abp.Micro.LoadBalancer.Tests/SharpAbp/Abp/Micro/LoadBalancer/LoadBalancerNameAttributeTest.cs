using SharpAbp.Abp.Micro.LoadBalancer.TestObjects;
using Xunit;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerNameAttributeTest
    {
        [Fact]
        public void Should_Get_Specified_Name()
        {
            var name = LoadBalancerNameAttribute
                  .GetLoadBalancerName<TestLoadBalancer2>();

            Assert.Equal("balancer2", name);
        }

        [Fact]
        public void Should_Get_Full_Class_Name_If_Not_Specified()
        {
            var expected = typeof(TestLoadBalancer1).FullName;

            var name = LoadBalancerNameAttribute
                  .GetLoadBalancerName<TestLoadBalancer1>();
            Assert.Equal(expected, name);
        }


        [Fact]
        public void GetName_By_Type()
        {
            var expected = typeof(TestLoadBalancer3).FullName;
            var name = LoadBalancerNameAttribute.GetLoadBalancerName(typeof(TestLoadBalancer3));
            Assert.Equal(expected, name);
        }

    }
}
