using System.Data;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder
{
    /// <summary>
    /// Defines an interface for detecting the database mode of a DM (Dameng) database connection
    /// </summary>
    public interface IDmDatabaseModeDetector : ITransientDependency
    {
        /// <summary>
        /// Detects the database mode for the given DM database connection
        /// </summary>
        /// <param name="dbConnection">The database connection to inspect</param>
        /// <returns>The detected DmDatabaseMode</returns>
        DmDatabaseMode DetectMode(IDbConnection dbConnection);
    }
}