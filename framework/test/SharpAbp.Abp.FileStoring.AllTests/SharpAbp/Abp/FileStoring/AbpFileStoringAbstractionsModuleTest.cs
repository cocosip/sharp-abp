using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.FileStoring
{
    public class AbpFileStoringAbstractionsModuleTest : AbpIntegratedTest<AbpFileStoringAbstractionsTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }

        [Fact]
        public void Should_Register_Accessor_And_Resolver_From_Abstractions_Module()
        {
            var accessor = GetRequiredService<IFilePathContextAccessor>();
            var resolver = GetRequiredService<IFilePathContextResolver>();
            var options = GetRequiredService<IOptions<AbpFilePathContextResolveOptions>>().Value;

            Assert.Same(AsyncLocalFilePathContextAccessor.Instance, accessor);
            Assert.IsType<DefaultFilePathContextResolver>(resolver);
            Assert.Empty(options.Contributors);
        }

        [Fact]
        public async Task Default_Resolver_Should_Return_Null_When_No_Contributor_Configured()
        {
            var resolver = GetRequiredService<IFilePathContextResolver>();

            var result = await resolver.ResolveAsync();

            Assert.Null(result);
        }
    }

    [DependsOn(
        typeof(AbpFileStoringAbstractionsModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutofacModule)
    )]
    public class AbpFileStoringAbstractionsTestModule : AbpModule
    {
    }
}
