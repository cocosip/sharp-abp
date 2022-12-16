using Volo.Abp.Reflection;

namespace SharpAbp.Abp.OpenIddict
{
    public class OpenIddictPermissions
    {
        public const string GroupName = "OpenIddict";
        public static class Applications
        {
            public const string Default = GroupName + ".Applications";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class Scopes
        {
            public const string Default = GroupName + ".Scopes";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(OpenIddictPermissions));
        }

    }
}
