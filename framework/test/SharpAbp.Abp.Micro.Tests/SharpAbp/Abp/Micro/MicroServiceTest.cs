using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.Micro
{
    public class MicroServiceTest
    {
        [Fact]
        public void MicroService_Equal_Test()
        {
            var service1 = new MicroService()
            {
                Id = "1",
                Service = "service",
                Address = "127.0.0.1",
                Port = 1000
            };
            var service1_1 = new MicroService()
            {
                Id = "1",
                Service = "service",
                Address = "127.0.0.1",
                Port = 1000,
                Tags = new List<string>()
                {
                    "http",
                    "grpc"
                }
            };

            var service2 = new MicroService()
            {
                Id = "2",
                Service = "service",
                Address = "127.0.0.1",
                Port = 1000
            };

            Assert.True(service1 == service1_1);
            Assert.True(service1.Equals(service1_1));

            Assert.False(service1 == service2);
            Assert.False(service1_1 == service2);
        }

    }
}
