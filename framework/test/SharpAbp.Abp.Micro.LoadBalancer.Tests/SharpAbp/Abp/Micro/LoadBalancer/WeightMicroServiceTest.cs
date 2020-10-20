using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.Micro.LoadBalancer
{
    public class WeightMicroServiceTest
    {
        [Fact]
        public void WeightMicroService_Compare_Test()
        {
            var weightMicroService1 = new WeightMicroService(1, new MicroService()
            {
                Id = "1",
                Address = "127.0.0.1",
                Port = 10001,
                Service = "service1"
            }, 2);

            var weightMicroService2 = new WeightMicroService(1, new MicroService()
            {
                Id = "2",
                Address = "127.0.0.1",
                Port = 10002,
                Service = "service1"
            }, 4);

            var weightMicroService3 = new WeightMicroService(1, new MicroService()
            {
                Id = "3",
                Address = "127.0.0.1",
                Port = 10003,
                Service = "service1"
            }, 5);

            Assert.True(weightMicroService1 < weightMicroService2);
            Assert.True(weightMicroService1 < weightMicroService3);
            Assert.True(weightMicroService2 < weightMicroService3);
        }


        [Fact]
        public void WeightMicroService_Max_CurrentWeight_Test()
        {
            var weightMicroService1 = new WeightMicroService(1, new MicroService()
            {
                Id = "1",
                Address = "127.0.0.1",
                Port = 10001,
                Service = "service1"
            }, 2);

            var weightMicroService2 = new WeightMicroService(1, new MicroService()
            {
                Id = "2",
                Address = "127.0.0.1",
                Port = 10002,
                Service = "service1"
            }, 4);

            var weightMicroService3 = new WeightMicroService(1, new MicroService()
            {
                Id = "3",
                Address = "127.0.0.1",
                Port = 10003,
                Service = "service1"
            }, 5);

            var services = new List<WeightMicroService>
            {
                weightMicroService1,
                weightMicroService3,
                weightMicroService2
            };

            var maxService = services.Max();

            Assert.Equal("3", maxService.Service.Id);
            Assert.Equal(5, maxService.CurrentWeight);
            Assert.Equal(1, maxService.Weight);
        }

    }
}
