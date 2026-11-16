namespace SharpAbp.Abp.Data
{
    /// <summary>
    /// Represents the supported database providers
    /// </summary>
    public enum DatabaseProvider
    {
        SqlServer,
        MySql,
        Oracle,
        PostgreSql,
        Sqlite,
        InMemory,
        Cosmos,
        Firebird,
        Dm,
        OpenGauss,
        GaussDB,

        /// <summary>
        /// OceanBase, currently used for the Oracle-compatible mode. The MySQL-compatible mode is
        /// accessed through the Pomelo provider and reported as <see cref="MySql"/>.
        /// </summary>
        OceanBase
    }
}
