using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace SharpAbp.MinId;

public class HttpApiClientModuleTest
{
    [Fact]
    public async Task Existing_Contract_Implementations_Can_Use_The_Body_Lookup_Convenience_Method()
    {
        IMinIdTokenAppService service = new LegacyMinIdTokenService();

        var result = await service.FindAsync(new FindMinIdTokenDto { BizType = "orders", Token = "legacytoken" });

        Assert.Equal("orders", result.BizType);
        Assert.Equal("legacytoken", result.Token);
    }

    private sealed class LegacyMinIdTokenService : IMinIdTokenAppService
    {
        public Task<MinIdTokenDto> FindByTokenAsync(string bizType, string token) =>
            Task.FromResult(new MinIdTokenDto { BizType = bizType, Token = token });

        public Task<MinIdTokenDto> GetAsync(Guid id) => throw new NotSupportedException();
        public Task<PagedResultDto<MinIdTokenDto>> GetPagedListAsync(MinIdTokenPagedRequestDto input) => throw new NotSupportedException();
        public Task<MinIdTokenDto> CreateAsync(CreateMinIdTokenDto input) => throw new NotSupportedException();
        public Task<MinIdTokenDto> UpdateAsync(Guid id, UpdateMinIdTokenDto input) => throw new NotSupportedException();
        public Task DeleteAsync(Guid id) => throw new NotSupportedException();
    }

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
