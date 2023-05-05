using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.FileStoring.Fakes;
using SharpAbp.Abp.FileStoring.TestObjects;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.FileStoring
{
    [DependsOn(
       typeof(AbpFileStoringModule),
       typeof(AbpTestBaseModule),
       typeof(AbpAutofacModule)
       )]
    public class AbpFileStoringTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IFileProvider, FakeFileProvider1>();
            context.Services.AddTransient<IFileProvider, FakeFileProvider2>();

            Configure<AbpFileStoringOptions>(options =>
            {
                options.Containers
                    .ConfigureDefault(container =>
                    {
                        container.SetConfiguration("TestConfigDefault", "TestValueDefault");
                        container.Provider = nameof(FakeFileProvider1);
                    })
                    .Configure<TestContainer1>(container =>
                    {
                        container.SetConfiguration("TestConfig1", "TestValue1");
                        container.Provider = nameof(FakeFileProvider1);
                    })
                    .Configure<TestContainer2>(container =>
                    {
                        container.SetConfiguration("TestConfig2", "TestValue2");
                        container.Provider = nameof(FakeFileProvider2);
                    });
            });
            return Task.CompletedTask;
        }
    }
}
