namespace SharpAbp.Abp.Data
{
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
        GaussDB
    }

    /// <summary>
    /// Represents the compatibility modes supported by DM (Dameng) database
    /// </summary>
    public enum DmDatabaseMode
    {
        /// <summary>
        /// Oracle compatibility mode
        /// </summary>
        Oracle,

        /// <summary>
        /// PostgreSQL compatibility mode
        /// </summary>
        PostgreSql,

        /// <summary>
        /// MySQL compatibility mode
        /// </summary>
        MySql
    }
}
