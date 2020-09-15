using Volo.Abp.Data;

namespace SharpAbp.Abp.FileStoring.Database
{
    public static class FileStoringDatabaseDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;


        public const string ConnectionStringName = "AbpFileStoring";
    }
}
