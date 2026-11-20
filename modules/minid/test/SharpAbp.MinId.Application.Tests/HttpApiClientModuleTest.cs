using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Xunit;

namespace SharpAbp.MinId;

public class HttpApiClientModuleTest
{
    [Fact]
    public void Application_Registers_A_Resolvable_Client_For_The_Module_Contract()
    {
        using var application = AbpApplicationFactory.Create<MinIdHttpApiClientModule>(options => options.UseAutofac());
        application.Initialize();

        Assert.NotNull(application.ServiceProvider.GetRequiredService<IMinIdInfoAppService>());
    }

    [Fact]
    public void HttpApi_Module_Registers_Its_Controller_Application_Part()
    {
        var services = new ServiceCollection();
        using var application = services.AddApplication<MinIdHttpApiModule>();
        using var provider = services.BuildServiceProvider();

        Assert.Contains(provider.GetRequiredService<Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPartManager>().ApplicationParts,
            part => part.Name == typeof(MinIdHttpApiModule).Assembly.GetName().Name);
    }
}