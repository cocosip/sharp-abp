using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Reflection;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DbConnectionsManagementPermissions
    {
        public const string GroupName = "DbConnectionsManagement";

        public const string AlwaysAllow = "DbConnectionsAlwaysAllow";

        public static class DatabaseConnectionInfos
        {
            public const string Default = GroupName + ".DatabaseConnectionInfos";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
        }

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(DbConnectionsManagementPermissions));
        }
    }
}
