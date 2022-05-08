using Volo.Abp.Data;

namespace SharpAbp.MinId
{
    public static class MinIdDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

        public const string ConnectionStringName = "AbpMinId";
    }
}
