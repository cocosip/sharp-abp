using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.Swashbuckle.Versioning;

public class SwashbuckleVersioningModuleTest : AbpIntegratedTest<SwashbuckleVersioningTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options) => options.UseAutofac();

    [Fact]
    public void Should_Apply_Defaults_And_Host_Preconfiguration()
    {
        var versioning = GetRequiredService<IOptions<ApiVersioningOptions>>().Value;
        Assert.True(versioning.AssumeDefaultVersionWhenUnspecified);
        Assert.True(versioning.ReportApiVersions);
        Assert.Equal(new ApiVersion(2, 0), versioning.DefaultApiVersion);
        var explorer = GetRequiredService<IOptions<ApiExplorerOptions>>().Value;
        Assert.Equal("'version-'VVV", explorer.GroupNameFormat);
        Assert.True(explorer.SubstituteApiVersionInUrl);
        Assert.Equal("SharpAbp", GetRequiredService<IOptions<AbpSwashbuckleVersioningOptions>>().Value.Title);
        Assert.Contains(GetRequiredService<System.Collections.Generic.IEnumerable<IConfigureOptions<SwaggerGenOptions>>>(),
            x => x is ConfigureSwaggerOptions);
    }
}

[DependsOn(typeof(AbpSwashbuckleVersioningModule), typeof(AbpAspNetCoreTestBaseModule), typeof(AbpAutofacModule))]
public class SwashbuckleVersioningTestModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<ApiVersioningOptions>(options => options.DefaultApiVersion = new ApiVersion(2, 0));
        context.Services.PreConfigure<ApiExplorerOptions>(options => options.GroupNameFormat = "'version-'VVV");
    }
}
