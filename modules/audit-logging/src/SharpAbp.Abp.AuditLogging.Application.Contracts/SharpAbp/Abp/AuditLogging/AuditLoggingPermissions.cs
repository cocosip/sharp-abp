using Volo.Abp.Reflection;

namespace SharpAbp.Abp.AuditLogging
{
    public class AuditLoggingPermissions
    {
        public const string GroupName = "SharpAbpAuditLogging";

        public static class AuditLogs
        {
            public const string Default = GroupName + ".AuditLogs";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(AuditLoggingPermissions));
        }
    }
}
