using Volo.Abp.Reflection;

namespace SharpAbp.Abp.FileStoringManagement
{
    public static class FileStoringManagementPermissions
    {
        public const string GroupName = "FileStoringManagement";

        public const string AlwaysAllow = "FileStoringAlwaysAllow";

        public static class Containers
        {
            public const string Default = GroupName + ".Containers";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static class Providers
        {
            public const string Default = GroupName + ".Providers";
            public const string Options = Default + ".Options";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileStoringManagementPermissions));
        }
    }
}
