using System.Data;
using OceanBaseConnector;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder.OceanBase
{
    /// <summary>
    /// Detects the OceanBase tenant compatibility mode from the connection.
    /// The mode is available after the connection is opened; unopened connections and other
    /// connection types fall back to Oracle mode.
    /// </summary>
    public class OceanBaseDatabaseModeDetector : IOceanBaseDatabaseModeDetector, ITransientDependency
    {
        /// <summary>
        /// Detects the database mode for the given OceanBase database connection
        /// </summary>
        /// <param name="dbConnection">The database connection to inspect</param>
        /// <returns>The detected OceanBaseDatabaseMode</returns>
        public virtual OceanBaseDatabaseMode DetectMode(IDbConnection dbConnection)
        {
            if (dbConnection is OceanBaseConnection { CompatibilityMode: { } compatibilityMode })
            {
                return compatibilityMode == OceanBaseCompatibilityMode.MySql
                    ? OceanBaseDatabaseMode.MySql
                    : OceanBaseDatabaseMode.Oracle;
            }

            return OceanBaseDatabaseMode.Oracle;
        }
    }
}
