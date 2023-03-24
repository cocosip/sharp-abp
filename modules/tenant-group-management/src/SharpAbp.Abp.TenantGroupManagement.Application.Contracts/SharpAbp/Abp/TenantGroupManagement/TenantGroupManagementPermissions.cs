using Volo.Abp.Reflection;

namespace SharpAbp.Abp.TenantGroupManagement
{
    public class TenantGroupManagementPermissions
    {
        public const string GroupName = "TenantGroupManagement";

        public static class TenantGroups
        {
            public const string Default = GroupName + ".TenantGroups";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string ManageFeatures = Default + ".ManageFeatures";
            public const string ManageTenants = Default + ".ManageTenants";
            public const string ManageConnectionStrings = Default + ".ManageConnectionStrings";
        }



        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(TenantGroupManagementPermissions));
        }
    }
}
