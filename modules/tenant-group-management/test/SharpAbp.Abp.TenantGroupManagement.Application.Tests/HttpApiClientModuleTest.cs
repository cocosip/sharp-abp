using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Xunit;

namespace SharpAbp.Abp.TenantGroupManagement;

public class HttpApiClientModuleTest
{

    [Fact]
    public void HttpApi_Module_Registers_Its_Controller_Application_Part()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<TenantGroupManagementHttpApiModule>();
        using var provider = services.BuildServiceProvider();

        Assert.Contains(provider.GetRequiredService<Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPartManager>().ApplicationParts,
            part => part.Name == typeof(TenantGroupManagementHttpApiModule).Assembly.GetName().Name);
    }
}
