using Microsoft.Extensions.Configuration;
using SharpAbp.Abp.Micro.Discovery.AddressTable;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpAbp.Abp.Micro.Discovery
{
    public class AddressTableDiscoveryOptionsConfigurationTest
    {
        [Fact]
        public void Configure_From_ConfigurationFile_Test()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var section = configuration.GetSection("AddressTable");
            var addressTableServices = section.Get<List<AddressTableService>>();

            Assert.Equal(2, addressTableServices.Count);

            var service1 = addressTableServices.FirstOrDefault(x => x.Service == "service1");
            Assert.NotNull(service1);

            var entry = service1.Entries.FirstOrDefault();
            Assert.Single(entry.Tags);
            Assert.Equal("grpc", entry.Tags[0]);

            Assert.True(entry.Meta.Count == 2);
            Assert.Equal("AddressTable", entry.Meta["Type"]);
            Assert.Equal("true", entry.Meta["Health"]);
        }

        [Fact]
        public void Configure_By_List()
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var section = configuration.GetSection("AddressTable");
            var addressTableServices = section.Get<List<AddressTableService>>();

            AddressTableDiscoveryOptions options = new AddressTableDiscoveryOptions();
            options.Configure(addressTableServices);


            var service1 = options.GetService("service1");
            Assert.Equal("service1", service1.Service);
            Assert.Equal(2, service1.Entries.Count);

            Assert.Equal("grpc", service1.Entries[0].Tags[0]);
            Assert.Equal("true", service1.Entries[0].Meta["Health"]);
            Assert.Equal("AddressTable", service1.Entries[0].Meta["Type"]);
        }


    }

}
