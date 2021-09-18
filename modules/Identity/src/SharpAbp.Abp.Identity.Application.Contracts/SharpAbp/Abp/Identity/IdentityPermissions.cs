using Volo.Abp.Reflection;

namespace SharpAbp.Abp.Identity
{
    public class IdentityPermissions
    {
        public const string GroupName = "SharpAbpIdentity";

        public static class IdentityClaimTypes
        {
            public const string Default = GroupName + ".IdentityClaimTypes";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class IdentitySecurityLogs
        {
            public const string Default = GroupName + ".IdentitySecurityLogs";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class OrganizationUnits
        {
            public const string Default = GroupName + ".OrganizationUnits";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

 
        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(IdentityPermissions));
        }
    }
}
