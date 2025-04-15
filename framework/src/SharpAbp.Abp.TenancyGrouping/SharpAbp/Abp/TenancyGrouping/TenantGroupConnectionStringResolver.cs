using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace SharpAbp.Abp.TenancyGrouping
{
    [Dependency(ReplaceServices = true)]
    public class TenantGroupConnectionStringResolver : MultiTenantConnectionStringResolver
    {
        protected ILogger Logger { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected ICurrentTenantGroup CurrentTenantGroup { get; }
        protected IServiceProvider ServiceProvider { get; }
        public TenantGroupConnectionStringResolver(
            ILogger<TenantGroupConnectionStringResolver> logger,
            IOptionsMonitor<AbpDbConnectionOptions> options,
            ICurrentTenant currentTenant,
            IServiceProvider serviceProvider,
            ICurrentTenantGroup currentTenantGroup)
            : base(options, currentTenant, serviceProvider)
        {
            Logger = logger;
            CurrentTenant = currentTenant;
            ServiceProvider = serviceProvider;
            CurrentTenantGroup = currentTenantGroup;
        }

        public override async Task<string> ResolveAsync(string? connectionStringName = null)
        {
            if (CurrentTenantGroup != null && CurrentTenantGroup.IsAvailable && CurrentTenant.Id.HasValue)
            {
                if (CurrentTenantGroup.Tenants != null && CurrentTenantGroup.Tenants.Contains(CurrentTenant.GetId()))
                {
                    try
                    {
                        var tenantGroup = await FindTenantGroupConfigurationAsync(CurrentTenantGroup.GetId());
                        var tenantGroupDefaultConnectionString = tenantGroup?.ConnectionStrings?.Default;

                        //Requesting default connection string...
                        if (connectionStringName == null ||
                            connectionStringName == ConnectionStrings.DefaultConnectionStringName)
                        {
                            //Return tenant's default or global default
                            return !tenantGroupDefaultConnectionString.IsNullOrWhiteSpace()
                                ? tenantGroupDefaultConnectionString!
                                : Options.ConnectionStrings.Default!;
                        }

                        //Requesting specific connection string...
                        var connString = tenantGroup?.ConnectionStrings?.GetOrDefault(connectionStringName);
                        if (!connString.IsNullOrWhiteSpace())
                        {
                            //Found for the tenant
                            return connString!;
                        }

                        //Fallback to the mapped database for the specific connection string
                        var database = Options.Databases.GetMappedDatabaseOrNull(connectionStringName);
                        if (database != null && database.IsUsedByTenants)
                        {
                            connString = tenantGroup?.ConnectionStrings?.GetOrDefault(database.DatabaseName);
                            if (!connString.IsNullOrWhiteSpace())
                            {
                                //Found for the tenant
                                return connString!;
                            }
                        }

                        //Fallback to tenant's default connection string if available
                        if (!tenantGroupDefaultConnectionString.IsNullOrWhiteSpace())
                        {
                            return tenantGroupDefaultConnectionString!;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Resolve connection by tenant failed. {Message}", ex.Message);
                    }
                }
            }
            return await base.ResolveAsync(connectionStringName);
        }


        protected virtual async Task<TenantGroupConfiguration?> FindTenantGroupConfigurationAsync(Guid groupId)
        {
            using var serviceScope = ServiceProvider.CreateScope();
            var tenantGroupStore = serviceScope
                .ServiceProvider
                .GetRequiredService<ITenantGroupStore>();

            return await tenantGroupStore.FindAsync(groupId);
        }


        //protected IServiceProvider ServiceProvider { get; }
        //public TenantGroupConnectionStringResolver(
        //    IOptionsMonitor<AbpDbConnectionOptions> options,
        //    ICurrentTenant currentTenant,
        //    IServiceProvider serviceProvider) : base(options, currentTenant, serviceProvider)
        //{
        //    ServiceProvider = serviceProvider;
        //}


        //protected override async Task<TenantConfiguration?> FindTenantConfigurationAsync(Guid tenantId)
        //{
        //    using var serviceScope = ServiceProvider.CreateScope();
        //    var currentTenantGroup = serviceScope.ServiceProvider.GetService<ICurrentTenantGroup>();
        //    if (currentTenantGroup != null && currentTenantGroup.IsAvailable)
        //    {

        //    }


        //    var tenantStore = serviceScope
        //        .ServiceProvider
        //        .GetRequiredService<ITenantStore>();

        //    var tenant = await tenantStore.FindAsync(tenantId);

        //    var tenantGroupStore = serviceScope.ServiceProvider.GetRequiredService<ITenantGroupStore>();
        //    var tenantGroup = await tenantGroupStore.FindByTenantIdAsync(tenantId);
        //    if (tenantGroup != null && tenantGroup.IsActive)
        //    {
        //        if (tenantGroup.ConnectionStrings != null)
        //        {
        //            var defaultConnectionString = tenant?.ConnectionStrings?.Default;

        //            if (tenant != null)
        //            {
        //                tenant.ConnectionStrings = tenantGroup.ConnectionStrings;
        //                if (tenant.ConnectionStrings.Default.IsNullOrWhiteSpace())
        //                {
        //                    tenant.ConnectionStrings.Default = defaultConnectionString;
        //                }
        //            }
        //        }
        //    }

        //    return tenant;
        //}

    }
}
