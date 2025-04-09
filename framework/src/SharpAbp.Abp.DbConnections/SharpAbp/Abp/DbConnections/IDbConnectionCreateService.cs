using JetBrains.Annotations;
using System.Data;
using System.Threading.Tasks;

namespace SharpAbp.Abp.DbConnections
{
    public interface IDbConnectionCreateService
    {
        /// <summary>
        /// Create DbConnection
        /// </summary>
        /// <param name="dbConnectionName"></param>
        /// <returns></returns>
        [NotNull]
        Task<IDbConnection?> CreateAsync([NotNull] string dbConnectionName);

        /// <summary>
        /// Create DbConnection by DbConnectionInfo
        /// </summary>
        /// <param name="dbConnectionInfo"></param>
        /// <returns></returns>
        [NotNull]
        Task<IDbConnection?> CreateAsync([NotNull] DbConnectionInfo dbConnectionInfo);
    }
}
