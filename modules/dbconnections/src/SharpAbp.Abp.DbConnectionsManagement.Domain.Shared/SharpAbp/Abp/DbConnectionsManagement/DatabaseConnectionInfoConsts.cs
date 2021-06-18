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
        /// Default value: 256.
        /// </summary>
        public static int MaxConnectionStringLength { get; set; } = 256;
    }
}
