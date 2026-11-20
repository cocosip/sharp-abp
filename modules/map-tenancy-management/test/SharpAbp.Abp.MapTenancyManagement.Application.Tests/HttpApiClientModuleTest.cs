using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.MapTenancyManagement;

public class HttpApiClientModuleTest
{
    [Fact]
    public void Application_Registers_A_Resolvable_Client_For_The_Module_Contract()
    {
        using var application = AbpApplicationFactory.Create<MapTenancyManagementHttpApiClientModule>(options => options.UseAutofac());
        application.Initialize();

        Assert.NotNull(application.ServiceProvider.GetRequiredService<IMapTenantAppService>());
    }

    [Fact]
    public void HttpApi_Module_Registers_Its_Controller_Application_Part()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<MapTenancyManagementHttpApiModule>();
        using var provider = services.BuildServiceProvider();

        Assert.Contains(provider.GetRequiredService<Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPartManager>().ApplicationParts,
            part => part.Name == typeof(MapTenancyManagementHttpApiModule).Assembly.GetName().Name);
    }
}