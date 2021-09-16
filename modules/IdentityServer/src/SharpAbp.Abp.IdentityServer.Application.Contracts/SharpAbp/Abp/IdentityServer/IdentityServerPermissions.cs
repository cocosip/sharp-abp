using Volo.Abp.Reflection;

namespace SharpAbp.Abp.IdentityServer
{
    public class IdentityServerPermissions
    {
        public const string GroupName = "SharpAbpIdentityServer";


        public static class ApiResources
        {
            public const string Default = GroupName + ".ApiResources";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class ApiScopes
        {
            public const string Default = GroupName + ".ApiScopes";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class Clients
        {
            public const string Default = GroupName + ".Clients";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(IdentityServerPermissions));
        }
    }
}