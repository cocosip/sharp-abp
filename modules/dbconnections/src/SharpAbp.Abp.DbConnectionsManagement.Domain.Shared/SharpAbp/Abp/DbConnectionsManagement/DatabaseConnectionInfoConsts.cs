namespace SharpAbp.Abp.DbConnectionsManagement
{
    public static class DatabaseConnectionInfoConsts
    {
        /// <summary>
        /// Default value : 64
        /// </summary>
        public static int MaxNameLength { get; set; } = 64;

        /// <summary>
        /// Default value : 32
        /// </summary>
        public static int MaxDatabaseProviderLength { get; set; } = 32;

        /// <summary>
        /// Default value: 1024.
        /// Kept at or below 2000 so Oracle maps the column to NVARCHAR2(n) instead of a LOB.
        /// </summary>
        public static int MaxConnectionStringLength { get; set; } = 1024;
    }
}
