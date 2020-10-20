using Xunit;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerHouseTest : AbpMicroLoadBalancerTestBase
    {
        private readonly ILoadBalancerHouse _loadBalancerHouse;
        public LoadBalancerHouseTest()
        {
            _loadBalancerHouse = GetRequiredService<ILoadBalancerHouse>();
        }

        [Fact]
        public void Get_LoadBalancer_Test()
        {
            var loadBalancer1 = _loadBalancerHouse.Get("micro.addresstable.service1");
            Assert.Equal(LoadBalancerConsts.NoLoadBalancer, loadBalancer1.BalancerType);

            var loadBalancer2 = _loadBalancerHouse.Get("micro.addresstable.service2");
            Assert.Equal(LoadBalancerConsts.Random, loadBalancer2.BalancerType);

            var loadBalancer3 = _loadBalancerHouse.Get("micro.addresstable.service3");
            Assert.Equal(LoadBalancerConsts.WeightRoundRobin, loadBalancer3.BalancerType);

            var loadBalancer4 = _loadBalancerHouse.Get("micro.addresstable.service4");
            Assert.Equal(LoadBalancerConsts.RoundRobin, loadBalancer4.BalancerType);
        }

    }
}
