#nullable enable

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace SharpAbp.Abp.Faster
{
    public class AbpFasterOptionsTests
    {
        [Fact]
        public void Configure_ShouldBindForceCompleteGapTimeoutMillis()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["FasterOptions:RootPath"] = "D:\\faster-tests",
                    ["FasterOptions:Configurations:default:FileName"] = "default.log",
                    ["FasterOptions:Configurations:default:PageSizeBits"] = "24",
                    ["FasterOptions:Configurations:default:MemorySizeBits"] = "25",
                    ["FasterOptions:Configurations:default:SegmentSizeBits"] = "31",
                    ["FasterOptions:Configurations:default:GapTimeoutMillis"] = "600000",
                    ["FasterOptions:Configurations:default:ForceCompleteGapTimeoutMillis"] = "45000"
                })
                .Build();

            var options = new AbpFasterOptions().Configure(configuration);
            var loggerConfiguration = options.Configurations.GetConfiguration("default");

            Assert.Equal("D:\\faster-tests", options.RootPath);
            Assert.Equal("default.log", loggerConfiguration.FileName);
            Assert.Equal(24, loggerConfiguration.PageSizeBits);
            Assert.Equal(25, loggerConfiguration.MemorySizeBits);
            Assert.Equal(31, loggerConfiguration.SegmentSizeBits);
            Assert.Equal(600000, loggerConfiguration.GapTimeoutMillis);
            Assert.Equal(45000, loggerConfiguration.ForceCompleteGapTimeoutMillis);
        }
    }
}
