using Volo.Abp.Reflection;

namespace SharpAbp.Abp.MapTenancyManagement
{
    public class MapTenancyManagementPermissions
    {
        public const string GroupName = "MapTenancyManagement";

        public const string AlwaysAllow = "MapTenancyAlwaysAllow";

        public static class MapTenants
        {
            public const string Default = GroupName + ".MapTenants";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(MapTenancyManagementPermissions));
        }
    }
}
