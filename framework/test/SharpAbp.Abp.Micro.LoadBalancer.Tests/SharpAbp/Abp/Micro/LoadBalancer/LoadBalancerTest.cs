using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerTest : AbpMicroLoadBalancerTestBase
    {
        private readonly ILoadBalancerHouse _loadBalancerHouse;
        public LoadBalancerTest()
        {
            _loadBalancerHouse = GetRequiredService<ILoadBalancerHouse>();
        }

        [Fact]
        public async Task NoLoadBalancer_First_Last_Test()
        {
            var loadBalancer1 = _loadBalancerHouse.Get("micro.addresstable.service1");
            Assert.Equal(LoadBalancerConsts.NoLoadBalancer, loadBalancer1.BalancerType);
            var service1 = await loadBalancer1.Lease();

            Assert.Equal("1-2", service1.Id);
            var service2 = await loadBalancer1.Lease();
            Assert.Equal("1-2", service2.Id);
            var loadBalancer2 = _loadBalancerHouse.Get("micro.addresstable.service1");
            Assert.Equal(loadBalancer1, loadBalancer2);
        }

        [Fact]
        public async Task Random_LoadBalancer_Random_Test()
        {
            var loadBalancer = _loadBalancerHouse.Get("micro.addresstable.service2");
            Assert.Equal(LoadBalancerConsts.Random, loadBalancer.BalancerType);

            var ids = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                var service = await loadBalancer.Lease();
                ids.Add(service.Id);
            }

            Assert.Contains("2-1", ids);
            Assert.Contains("2-2", ids);
        }

        [Fact]
        public async Task RoundRobin_LoadBalancer_Gain_Test()
        {
            var loadBalancer = _loadBalancerHouse.Get("micro.addresstable.service4");
            Assert.Equal(LoadBalancerConsts.RoundRobin, loadBalancer.BalancerType);

            var service1 = await loadBalancer.Lease();
            var service2 = await loadBalancer.Lease();
            var service3 = await loadBalancer.Lease();
            var service4 = await loadBalancer.Lease();

            Assert.Equal("4-1", service1.Id);
            Assert.Equal("4-2", service2.Id);
            Assert.Equal("4-1", service3.Id);
            Assert.Equal("4-2", service4.Id);
        }


        [Fact]
        public async Task WeightRoundRobin_LoadBalancer_Round_Test()
        {
            var loadBalancer = _loadBalancerHouse.Get("micro.addresstable.service3");
            Assert.Equal(LoadBalancerConsts.WeightRoundRobin, loadBalancer.BalancerType);

            var service1 = await loadBalancer.Lease();
            var service2 = await loadBalancer.Lease();
            var service3 = await loadBalancer.Lease();
            var service4 = await loadBalancer.Lease();
            var service5 = await loadBalancer.Lease();
            var service6 = await loadBalancer.Lease();
            var service7 = await loadBalancer.Lease();

            Assert.Equal(service1.Id, service7.Id);
            Assert.Equal(service1.Id, service2.Id);
            Assert.Equal("3-3", service1.Id);
        }



    }
}
