using Volo.Abp.Data;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class AbpTransformSecurityManagementDbProperties
    {
        public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

        public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

        public const string ConnectionStringName = "AbpTransformSecurityManagement";
    }
}
