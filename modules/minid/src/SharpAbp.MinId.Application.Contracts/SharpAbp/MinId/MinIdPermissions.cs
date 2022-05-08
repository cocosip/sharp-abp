using Volo.Abp.Reflection;

namespace SharpAbp.MinId
{
    public class MinIdPermissions
    {
        public const string GroupName = "MinId";

        public const string AlwaysAllow = "MinIdAlwaysAllow";

        public static class MinIdInfos
        {
            public const string Default = GroupName + ".MinIdInfos";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class MinIdTokens
        {
            public const string Default = GroupName + ".MinIdTokens";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(MinIdPermissions));
        }
    }
}
