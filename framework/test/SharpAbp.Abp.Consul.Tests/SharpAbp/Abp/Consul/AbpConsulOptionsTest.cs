using SharpAbp.Abp.Consul.TestObjects;
using Xunit;

namespace SharpAbp.Abp.Consul
{
    public class AbpConsulOptionsTest : AbpConsulTestBase
    {
        private readonly IConsulConfigurationProvider _configurationProvider;

        public AbpConsulOptionsTest()
        {
            _configurationProvider = GetRequiredService<IConsulConfigurationProvider>();
        }


        [Fact]
        public void Get_Configuration_Test()
        {
            var defaultConfiguration = _configurationProvider.Get("default");
            Assert.Equal("http://127.0.0.1/", defaultConfiguration.Address.ToString());
            Assert.Equal("DataCenter1", defaultConfiguration.DataCenter);
            Assert.Equal("123", defaultConfiguration.Token);
            Assert.Equal(5, defaultConfiguration.WaitTime.Value.TotalSeconds);

            var configuration2 = _configurationProvider.Get("Test2");
            Assert.Equal("http://192.168.0.100/api", configuration2.Address.ToString());
            Assert.Equal("DataCenter2", configuration2.DataCenter);
            Assert.Equal("456", configuration2.Token);
            Assert.Null(configuration2.WaitTime);

            var configuration3 = _configurationProvider.Get(typeof(TestConsul3).FullName);
            Assert.Equal("http://192.168.0.103/", configuration3.Address.ToString());
            Assert.Equal("DataCenter3", configuration3.DataCenter);
            Assert.Equal("111", configuration3.Token);
            Assert.Equal(3, configuration3.WaitTime.Value.TotalSeconds);

            Assert.NotNull(configuration3.ClientOverride);
            Assert.NotNull(configuration3.HandlerOverride);
        }

    }
}
