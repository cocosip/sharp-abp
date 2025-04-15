namespace SharpAbp.Abp.TenancyGrouping.ConfigurationStore
{
    public class AbpDefaultTenantGroupStoreOptions
    {
        public TenantGroupConfiguration[] Groups { get; set; }

        public AbpDefaultTenantGroupStoreOptions()
        {
            Groups = [];
        }
    }
}
