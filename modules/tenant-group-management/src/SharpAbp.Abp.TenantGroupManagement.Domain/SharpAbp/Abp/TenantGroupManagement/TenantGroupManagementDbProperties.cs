using Volo.Abp.Data;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupManagementDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

        public const string ConnectionStringName = "AbpTenantGroupManagement";
    }
}
