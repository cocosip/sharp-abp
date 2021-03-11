using Volo.Abp.Data;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public static class MapTenancyManagementDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

        public const string ConnectionStringName = "AbpMapTenancyManagement";
    }
}
