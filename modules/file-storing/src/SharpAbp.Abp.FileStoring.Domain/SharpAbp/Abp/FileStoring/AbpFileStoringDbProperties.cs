using Volo.Abp.Data;

namespace SharpAbp.Abp.FileStoring
{
    public static class AbpFileStoringDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

        public const string ConnectionStringName = "AbpFileStoring";
    }
}
