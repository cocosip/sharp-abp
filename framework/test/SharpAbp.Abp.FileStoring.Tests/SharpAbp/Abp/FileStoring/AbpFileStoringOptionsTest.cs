using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring.Fakes;
using SharpAbp.Abp.FileStoring.TestObjects;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Modularity;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringOptionsTest : AbpFileStoringTestBase
    {
        private readonly IFileContainerConfigurationProvider _configurationProvider;

        public AbpFileStoringOptionsTest()
        {
            _configurationProvider = GetRequiredService<IFileContainerConfigurationProvider>();
        }

        [Fact]
        public void Should_Property_Set_And_Get_Options_For_Different_Containers()
        {

            var testContainer1Config = _configurationProvider.Get<TestContainer1>();

            Assert.Equal(nameof(FakeFileProvider1), testContainer1Config.Provider);
            Assert.Equal("TestValue1", testContainer1Config.GetConfigurationOrDefault<string>("TestConfig1"));
            Assert.Equal("TestValueDefault", testContainer1Config.GetConfigurationOrDefault<string>("TestConfigDefault"));


            var testContainer2Config = _configurationProvider.Get<TestContainer2>();
            Assert.Equal(nameof(FakeFileProvider2), testContainer2Config.Provider);
            Assert.Equal("TestValue2", testContainer2Config.GetConfigurationOrDefault<string>("TestConfig2"));
            Assert.Equal("TestValueDefault", testContainer2Config.GetConfigurationOrDefault<string>("TestConfigDefault"));
        }

        [Fact]
        public void Should_Fallback_To_Default_Configuration_If_Not_Specialized()
        {
            var config = _configurationProvider.Get<TestContainer3>();
            Assert.Equal(nameof(FakeFileProvider1), config.Provider);
            Assert.Equal("TestValueDefault", config.GetConfigurationOrNull("TestConfigDefault"));
        }

        [Fact]
        public void Should_Keep_Container_Defaults_When_Json_Entry_Omits_Values()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["FileStoringOptions:test:Provider"] = nameof(FakeFileProvider1)
                })
                .Build();

            var services = new ServiceCollection();
            services.PreConfigure<AbpFileStoringAbstractionsOptions>(opts =>
            {
                opts.Providers.TryAdd(new FileProviderConfiguration(
                    nameof(FakeFileProvider1),
                    typeof(AbpFileStoringOptions)));
            });

            var options = new AbpFileStoringOptions();
            options.Configure(configuration, new ServiceConfigurationContext(services));

            var container = options.Containers.GetConfiguration("test");
            Assert.Equal(nameof(FakeFileProvider1), container.Provider);
            Assert.True(container.IsMultiTenant);
            Assert.False(container.EnableAutoMultiPartUpload);
            Assert.Equal(1024 * 1024 * 100, container.MultiPartUploadMinFileSize);
            Assert.Equal(1024 * 1024 * 5, container.MultiPartUploadShardingSize);
            Assert.True(container.HttpAccess);
        }
    }
}
