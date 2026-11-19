using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Defines an interface for detecting the compatibility mode of an OceanBase database connection
    /// </summary>
    public interface IOceanBaseDatabaseModeDetector : ITransientDependency
    {
        /// <summary>
        /// Detects the database mode for the given OceanBase database connection
        /// </summary>
        /// <param name="dbConnection">The database connection to inspect</param>
        /// <returns>The detected OceanBaseDatabaseMode</returns>
        OceanBaseDatabaseMode DetectMode(IDbConnection dbConnection);
    }
}
