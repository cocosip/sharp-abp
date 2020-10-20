using Xunit;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class LoadBalancerUtilTest
    {
        [Fact]
        public void ParseWeightHostAndPorts_Test()
        {
            var weights = "127.0.0.1:1000-3,192.168.0.100:10002-5,192.168.100.103:333-2";
            var weightServiceHostAndPorts = LoadBalancerUtil.ParseWeightHostAndPorts(weights);

            var v1 = weightServiceHostAndPorts[0];
            Assert.Equal("127.0.0.1", v1.HostAndPort.Host);
            Assert.Equal(1000, v1.HostAndPort.Port);
            Assert.Equal(3, v1.Weight);

            var v2 = weightServiceHostAndPorts[1];
            Assert.Equal("192.168.0.100", v2.HostAndPort.Host);
            Assert.Equal(10002, v2.HostAndPort.Port);
            Assert.Equal(5, v2.Weight);

            var v3 = weightServiceHostAndPorts[2];
            Assert.Equal("192.168.100.103", v3.HostAndPort.Host);
            Assert.Equal(333, v3.HostAndPort.Port);
            Assert.Equal(2, v3.Weight);
        }
    }
}
