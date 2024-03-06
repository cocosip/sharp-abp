using Volo.Abp.Reflection;

namespace SharpAbp.Abp.TransformSecurityManagement
{
    public class TransformSecurityManagementPermissions
    {
        public const string GroupName = "AbpTransformSecurityManagement";

        public static class SecurityCredentialInfos
        {
            public const string Default = GroupName + ".SecurityCredentialInfos";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }
        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(TransformSecurityManagementPermissions));
        }
    }
}
