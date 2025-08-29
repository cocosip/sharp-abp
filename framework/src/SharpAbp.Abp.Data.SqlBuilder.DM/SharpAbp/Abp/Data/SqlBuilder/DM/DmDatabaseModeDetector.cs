using System.Data;
using Dm;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.Data.SqlBuilder.DM
{
    /// <summary>
    /// DM database mode detector
    /// </summary>
    public class DmDatabaseModeDetector : IDmDatabaseModeDetector, ITransientDependency
    {
        /// <summary>
        /// Detect DM database compatibility mode
        /// </summary>
        /// <param name="dbConnection">Database connection</param>
        /// <returns>Database mode</returns>
        public virtual DmDatabaseMode DetectMode(IDbConnection dbConnection)
        {
            var dmConnection = dbConnection as DmConnection;
            if (dmConnection != null)
            {
                if (dmConnection.compatibleOracle())
                {
                    return DmDatabaseMode.Oracle;
                }
                else if (dmConnection.compatibleMysql())
                {
                    return DmDatabaseMode.MySql;
                }
                else
                {
                    return DmDatabaseMode.PostgreSql;
                }
            }
            
            // Default mode is Oracle
            return DmDatabaseMode.Oracle;
        }
    }
}