namespace SharpAbp.Abp.TenancyGrouping.ConfigurationStore
{
    public class AbpDefaultTenantGroupStoreOptions
    {
        public TenantGroupConfiguration[] TenantGroups { get; set; }

        public AbpDefaultTenantGroupStoreOptions()
        {
            TenantGroups = new TenantGroupConfiguration[0];
        }
    }
}
