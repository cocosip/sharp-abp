using System.Data;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Default implementation of IOceanBaseDatabaseModeDetector that returns Oracle mode as default
    /// </summary>
    public class DefaultOceanBaseDatabaseModeDetector : IOceanBaseDatabaseModeDetector, ITransientDependency
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        protected ILogger Logger { get; }

        public DefaultOceanBaseDatabaseModeDetector(ILogger<DefaultOceanBaseDatabaseModeDetector> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Detects the database mode for the given OceanBase database connection
        /// Currently returns Oracle mode as default
        /// </summary>
        /// <param name="dbConnection">The database connection to inspect</param>
        /// <returns>Oracle mode as default</returns>
        public virtual OceanBaseDatabaseMode DetectMode(IDbConnection dbConnection)
        {
            Logger.LogDebug("Using Oracle compatibility mode as default for OceanBase database");
            return OceanBaseDatabaseMode.Oracle;
        }
    }
}
