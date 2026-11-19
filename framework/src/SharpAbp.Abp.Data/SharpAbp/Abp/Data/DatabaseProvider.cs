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
        /// OceanBase. A tenant runs in an Oracle- or MySQL-compatible mode; Oracle mode is the default
        /// when the mode is not known. The MySQL-compatible mode through Pomelo is reported as <see cref="MySql"/>.
        /// </summary>
        OceanBase
    }
}
