using Microsoft.Extensions.Options;
using Xunit;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class AbpMicroLoadBalancerOptionsTest : AbpMicroLoadBalancerOptionsTestBase
    {
        private readonly AbpMicroLoadBalancerOptions _options;
        public AbpMicroLoadBalancerOptionsTest()
        {
            _options = GetRequiredService<IOptions<AbpMicroLoadBalancerOptions>>().Value;
        }


        [Fact]
        public void AbpMicroLoadBalancer_Configure_Test()
        {
            var defaultConfiguration = _options.Configurations.GetConfiguration(DefaultLoadBalancer.Name);
            Assert.Equal("Random", defaultConfiguration.BalancerType);

            var service1Configuration = _options.Configurations.GetConfiguration("service1");
            Assert.Equal("NoLoadBalancer", service1Configuration.BalancerType);
            Assert.Equal(true, service1Configuration.GetConfiguration(NoBalancerConfigurationNames.FirstOne));

            var service2Configuration = _options.Configurations.GetConfiguration("service2");
            Assert.Equal("RoundRobin", service2Configuration.BalancerType);
            Assert.Equal(1, service2Configuration.GetConfiguration(RoundRobinLoadBalancerConfigurationNames.Step));

            var service3Configuration = _options.Configurations.GetConfiguration("service3");
            Assert.Equal("WeightRoundRobin", service3Configuration.BalancerType);
            Assert.Equal("127.0.0.1:100-3,127.0.0.1:101-4,127.0.0.102-5", service3Configuration.GetConfiguration(WeightRoundRobinLoadBalancerConfigurationNames.Weights));
        }

    }
}
