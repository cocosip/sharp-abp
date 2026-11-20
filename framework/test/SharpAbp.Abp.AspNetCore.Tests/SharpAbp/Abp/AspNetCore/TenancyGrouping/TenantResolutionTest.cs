using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharpAbp.Abp.MapTenancy;
using SharpAbp.Abp.TenancyGrouping;
using SharpAbp.Abp.TenancyGrouping.ConfigurationStore;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Testing;
using Xunit;

namespace SharpAbp.Abp.AspNetCore.TenancyGrouping;

[DependsOn(typeof(AbpAutofacModule), typeof(AbpTestBaseModule), typeof(AbpTenancyGroupingModule), typeof(AbpMapTenancyModule))]
public class TenantResolutionTestModule : AbpModule
{
    public static readonly Guid TenantId = Guid.Parse("5e5ee40f-d13b-43a9-bd74-e74781531b23");
    public static readonly Guid GroupId = Guid.Parse("e7ed80bf-a4ae-421f-8c47-51f0019ca5b6");

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDefaultTenantGroupStoreOptions>(options => options.Groups =
        [
            new TenantGroupConfiguration(GroupId, "Acme", "ACME") { Tenants = [TenantId] },
            new TenantGroupConfiguration(Guid.Parse("1da3cdc5-fc33-43aa-bc9e-213273316d99"), "Inactive", "INACTIVE") { IsActive = false }
        ]);
        Configure<AbpMapTenancyOptions>(options => options.Mappers.Configure("local", mapping =>
        {
            mapping.TenantId = TenantId;
            mapping.Code = "local";
            mapping.MapCode = "partner";
        }));
    }
}

public class TenantResolutionTest : AbpIntegratedTest<TenantResolutionTestModule>
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options) => options.UseAutofac();

    [Fact]
    public async Task Registered_Providers_Should_Resolve_Current_Tenant_From_Configured_Store()
    {
        var store = GetRequiredService<ITenantGroupStore>();
        var group = await store.FindAsync(TenantResolutionTestModule.GroupId);
        Assert.NotNull(group);
        Assert.Same(group, await store.FindAsync("ACME"));
        Assert.Same(group, await store.FindByTenantIdAsync(TenantResolutionTestModule.TenantId));
        Assert.Equal(2, (await store.GetListAsync()).Count);
        Assert.Null(await store.FindAsync(Guid.Empty));
        Assert.Null(await store.FindAsync("missing"));
        Assert.Null(await store.FindByTenantIdAsync(Guid.Empty));

        using (GetRequiredService<ICurrentTenant>().Change(TenantResolutionTestModule.TenantId))
        {
            var result = await GetRequiredService<ITenantGroupResolver>().ResolveGroupIdOrNameAsync();
            Assert.Equal(TenantResolutionTestModule.GroupId.ToString(), result.GroupIdOrName);
            Assert.Equal(new[] { "Default" }, result.AppliedResolvers);
            Assert.Same(group, await GetRequiredService<ITenantGroupConfigurationProvider>().GetAsync(true));
        }

        Assert.Null((await GetRequiredService<ITenantGroupResolver>().ResolveGroupIdOrNameAsync()).GroupIdOrName);
        Assert.Null(await GetRequiredService<ITenantGroupConfigurationProvider>().GetAsync());
        var mapping = await GetRequiredService<IMapTenancyConfigurationProvider>().GetByMapCodeAsync("partner");
        Assert.Equal(TenantResolutionTestModule.TenantId, mapping.TenantId);
        Assert.Equal("local", (await GetRequiredService<IMapTenantCodeProvider>().FindByTenantIdAsync(TenantResolutionTestModule.TenantId)).Code);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Resolver_Should_Stop_After_Resolved_Group_Or_Explicit_Host(bool host)
    {
        var options = new AbpTenantGroupResolveOptions();
        options.TenantResolvers.Add(new ActionTenantGroupResolveContributor(_ => { }));
        options.TenantResolvers.Add(new ActionTenantGroupResolveContributor(context =>
        {
            context.GroupIdOrName = host ? null : "ACME";
            context.Handled = host;
        }));
        options.TenantResolvers.Add(new ActionTenantGroupResolveContributor(_ => throw new InvalidOperationException("Must not run")));

        var result = await new TenantGroupResolver(Options.Create(options), ServiceProvider).ResolveGroupIdOrNameAsync();

        Assert.Equal(host ? null : "ACME", result.GroupIdOrName);
        Assert.Equal(new[] { "Action", "Action" }, result.AppliedResolvers);
    }

    [Theory]
    [InlineData("acme", null)]
    [InlineData("e7ed80bf-a4ae-421f-8c47-51f0019ca5b6", null)]
    [InlineData("missing", "Volo.AbpIo.TenancyGrouping:010001")]
    [InlineData("inactive", "Volo.AbpIo.TenancyGrouping:010002")]
    public async Task Configuration_Provider_Should_Normalize_Names_And_Validate_Resolved_Groups(string name, string errorCode)
    {
        var options = new AbpTenantGroupResolveOptions();
        options.TenantResolvers.Add(new ActionTenantGroupResolveContributor(context => context.GroupIdOrName = name));
        var accessor = new ResolveResultAccessor();
        var provider = ActivatorUtilities.CreateInstance<TenantGroupConfigurationProvider>(ServiceProvider,
            new TenantGroupResolver(Options.Create(options), ServiceProvider), accessor);

        if (errorCode == null)
        {
            var group = await provider.GetAsync(true);
            Assert.Equal(TenantResolutionTestModule.GroupId, group.Id);
        }
        else
        {
            var exception = await Assert.ThrowsAsync<BusinessException>(() => provider.GetAsync(true));
            Assert.Equal(errorCode, exception.Code);
        }

        Assert.Equal(name, accessor.Result.GroupIdOrName);
        Assert.Equal(new[] { "Action" }, accessor.Result.AppliedResolvers);
    }

    private sealed class ResolveResultAccessor : ITenantGroupResolveResultAccessor
    {
        public TenantGroupResolveResult Result { get; set; }
    }
}
