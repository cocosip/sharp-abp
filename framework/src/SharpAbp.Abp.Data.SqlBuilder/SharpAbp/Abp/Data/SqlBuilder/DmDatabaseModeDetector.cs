using System.Data;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Default implementation of IDmDatabaseModeDetector that returns Oracle mode as default
    /// </summary>
    public class DefaultDmDatabaseModeDetector : IDmDatabaseModeDetector, ITransientDependency
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        protected ILogger Logger { get; }

        public DefaultDmDatabaseModeDetector(ILogger<DefaultDmDatabaseModeDetector> logger)
        {
            Logger = logger;
        }
   
        /// <summary>
        /// Detects the database mode for the given DM database connection
        /// Currently returns Oracle mode as default
        /// </summary>
        /// <param name="dbConnection">The database connection to inspect</param>
        /// <returns>Oracle mode as default</returns>
        public virtual DmDatabaseMode DetectMode(IDbConnection dbConnection)
        {
            Logger.LogDebug("Using Oracle compatibility mode as default for DM database");
            return DmDatabaseMode.Oracle;
        }
    }
}