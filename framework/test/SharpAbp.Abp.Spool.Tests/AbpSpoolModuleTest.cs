using Microsoft.Extensions.DependencyInjection;
using Moq;
using Spool;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;
using Xunit;

namespace SharpAbp.Abp.Spool.Tests
{
    public class AbpSpoolModuleTest
    {

        [Fact]
        public void InitTest()
        {
            var mockServiceConfigurationContext = new Mock<ServiceConfigurationContext>();

            var module = new AbpSpoolModule();

            //module.ConfigureServices(mockServiceConfigurationContext.Object);

            //mockServiceConfigurationContext.Verify(x => x.Services.Add(It.IsAny<ServiceDescriptor>()), Times.Once);

        }
    }
}
